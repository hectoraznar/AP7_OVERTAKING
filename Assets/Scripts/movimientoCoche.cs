using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class movimientoCoche : MonoBehaviour

{
    [Header("Referencias")]
    public Camera camara;
    public GameObject carreteraOriginal;      // Asigna 'carretera_original' (o lo buscará por nombre)
    public GameObject triggerObject;          // El objeto trigger (con Collider y IsTrigger = true)

    [Header("Movimiento")]
    public float velocidadBase = 30f;         // Velocidad objetivo hacia adelante
    public float aceleracion = 50f;           // no usado para física directa, sirve en lerp si quieres
    public float frenado = 12f;
    public float aceleracionSuave = 1.5f;     // para suavizar velocímetro
    public float velocidadMaximaAtras = 15f;  // límite marcha atrás
    public float velocidadActual = 0f;        // valor real (positivo hacia adelante, negativo atrás)

    [Header("UI")]
    public TextMeshProUGUI textoVelocidad;    // Nombre en escena: "textoVelocidad"
    public TextMeshProUGUI textoTemporizador; // "Time" o similar
    public TextMeshProUGUI textoPuntos;       // "Texto Puntos"
    private bool primerTramoGenerado = false;

    [Header("Puntos y tiempo")]
    public int puntosPorSegundo = 5;
    public float intervaloPuntos = 1f;

    [Header("Mapa Infinito")]
    [Tooltip("Time de bloqueo para evitar múltiples generados consecutivos")]
    public float cooldownGeneracion = 0.6f;
    [Tooltip("Separación segura entre tramos (en unidades Z)")]
    public float separacionSegura = 0f;
    public int maxSegmentosActivos = 6;
    private bool puedeRegenerar = true;

    [Header("Giro / física visual")]
    public float giroMaximo = 6f;
    public float giroMinimo = 2f;
    public float velocidadReferencia = 40f;

    // internos
    private InputAction movimientoAction;
    private InputAction acelerarAction;
    private InputAction frenarAction;

    private float velocidadVisual = 0f;      // para suavizar lectura del velocímetro
    private float tiempoTranscurrido = 0f;
    private bool temporizadorActivo = true;
    private int puntosTotales = 0;
    private float tiempoUltimoPunto = 0f;

    private Queue<GameObject> segmentosActivos = new Queue<GameObject>();
    private bool puedeGenerar = true;

    private float limiteIzquierdo;
    private float limiteDerecho;

    void Awake()
    {
        ConfigurarInput();
    }

    void Start()
    {
        if (camara == null) camara = Camera.main;

        // Buscar referencias por nombre si no están asignadas
        if (carreteraOriginal == null)
            carreteraOriginal = GameObject.Find("carretera_original");

        if (triggerObject == null)
        {
            GameObject t = GameObject.Find("trigger");
            if (t != null) triggerObject = t;
            else
            {
                var byTag = GameObject.FindGameObjectWithTag("trigger");
                if (byTag != null) triggerObject = byTag;
            }
        }

        if (textoVelocidad == null)
        {
            var tv = GameObject.Find("textoVelocidad") ?? GameObject.Find("TextoVelocidad");
            if (tv != null) textoVelocidad = tv.GetComponent<TextMeshProUGUI>();
        }

        if (textoTemporizador == null)
        {
            var tt = GameObject.Find("Time") ?? GameObject.Find("textoTemporizador");
            if (tt != null) textoTemporizador = tt.GetComponent<TextMeshProUGUI>();
        }

        if (textoPuntos == null)
        {
            var tp = GameObject.Find("Texto Puntos") ?? GameObject.Find("TextoPuntos");
            if (tp != null) textoPuntos = tp.GetComponent<TextMeshProUGUI>();
        }

        // Inicializar límites laterales
        CalcularLimitesPantalla();

        // Colocar primer tramo en la cola
        if (carreteraOriginal != null)
            segmentosActivos.Enqueue(carreteraOriginal);

        Debug.Log("movimientoCoche inicializado.");
    }

    void Update()
    {
        ActualizarTemporizador();
        ActualizarPuntos();
        ActualizarVelocimetro();
        MoverCoche();
        LimitarPosicion();

        // tecla R para forzar generación (debug)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            GenerarNuevoMapa();
        }
    }

    // --------------------------
    // Movimiento y velocidad
    // --------------------------
    void MoverCoche()
    {
        if (movimientoAction == null) return;
        Vector2 input = movimientoAction.ReadValue<Vector2>();
        float dirX = input.x;

        ControlarVelocidad(input);

        // movimiento lateral solo si velocidad != 0 (si quieres permitir girar en parado quita la condición)
        if (Mathf.Abs(velocidadActual) > 0.01f)
        {
            float giroEfectivo = CalcularGiroEfectivo();
            float movimientoX = dirX * giroEfectivo * Time.deltaTime;
            transform.Translate(movimientoX, 0f, 0f);
        }

        // movimiento adelante/atrás
        if (Mathf.Abs(velocidadActual) > 0.001f)
        {
            float movimientoZ = velocidadActual * Time.deltaTime;
            transform.Translate(0f, 0f, movimientoZ);
        }
    }

    void ControlarVelocidad(Vector2 input)
    {
        if (acelerarAction == null || frenarAction == null) return;

        float acel = acelerarAction.ReadValue<float>();
        float fren = frenarAction.ReadValue<float>();

        // Acelerar (hacia velocidadBase con suavizado)
        if (acel > 0f || input.y > 0f)
        {
            velocidadActual = Mathf.Lerp(velocidadActual, velocidadBase, aceleracionSuave * Time.deltaTime);
        }
        // Frenar / marcha atrás
        else if (fren > 0f || input.y < 0f)
        {
            if (velocidadActual > 0f)
            {
                // frenar a 0
                velocidadActual = Mathf.Lerp(velocidadActual, 0f, 2f * Time.deltaTime);
            }
            else
            {
                // objetivo marcha atrás limitado
                velocidadActual = Mathf.Lerp(velocidadActual, -velocidadMaximaAtras, 1f * Time.deltaTime);
            }
        }
        // Frenado natural
        else
        {
            velocidadActual = Mathf.Lerp(velocidadActual, 0f, 1.5f * Time.deltaTime);
            if (Mathf.Abs(velocidadActual) < 0.02f) velocidadActual = 0f;
        }
    }

    float CalcularGiroEfectivo()
    {
        float vAbs = Mathf.Abs(velocidadActual);
        float factor = Mathf.Clamp01(vAbs / velocidadReferencia);
        return Mathf.Lerp(giroMaximo, giroMinimo, factor);
    }

    float CalcularInclinacionEfectiva()
    {
        float vAbs = Mathf.Abs(velocidadActual);
        float factor = Mathf.Clamp01(vAbs / velocidadReferencia);
        return Mathf.Lerp(12f, 4f, factor);
    }

    void LimitarPosicion()
    {
        if (camara == null) return;
        CalcularLimitesPantalla(); // mantén límites actualizados por si la cámara cambia

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limiteIzquierdo, limiteDerecho);
        transform.position = pos;
    }

    void CalcularLimitesPantalla()
    {
        if (camara == null) return;
        float distanciaZ = Mathf.Abs(transform.position.z - camara.transform.position.z);
        Vector3 inferior = camara.ViewportToWorldPoint(new Vector3(0f, 0f, distanciaZ));
        Vector3 superior = camara.ViewportToWorldPoint(new Vector3(1f, 1f, distanciaZ));
        limiteIzquierdo = inferior.x + 1f;
        limiteDerecho = superior.x - 1f;
    }

    // --------------------------
    // Mapa infinito: generar nuevo tramo al final del último
    // --------------------------
void GenerarNuevoMapa()
{
    if (!puedeRegenerar) return;
    if (carreteraOriginal == null) return;

    GameObject ultimo = segmentosActivos.Last();

    // Calculamos el tamaño del prefab base
    Bounds bBase;
    TryGetBounds(carreteraOriginal, out bBase);
    float largo = bBase.size.z;

    Vector3 posNuevo;

    // 🟢 Si solo hay el original, el siguiente va en Z = 880 exacto
    if (segmentosActivos.Count == 1)
    {
        posNuevo = new Vector3(
            ultimo.transform.position.x,
            ultimo.transform.position.y,
            880f
        );
    }
    else
    {
        // 🟡 Los siguientes se generan pegados al anterior
        posNuevo = new Vector3(
            ultimo.transform.position.x,
            ultimo.transform.position.y,
            ultimo.transform.position.z + largo
        );
    }

    // Instanciamos nuevo tramo
    GameObject nuevo = Instantiate(carreteraOriginal, posNuevo, ultimo.transform.rotation);
    nuevo.name = "carretera_copia_" + System.DateTime.Now.Ticks;
    segmentosActivos.Enqueue(nuevo);

    // 🔹 Fija la altura y orientación igual al anterior
    Vector3 posFija = nuevo.transform.position;
    posFija.y = ultimo.transform.position.y;
    nuevo.transform.position = posFija;
    nuevo.transform.rotation = ultimo.transform.rotation;

    // 🔹 Actualiza el trigger
    if (triggerObject != null)
    {
        Bounds bNuevo;
        TryGetBounds(nuevo, out bNuevo);
        float zFinalNuevo = bNuevo.center.z + bNuevo.extents.z;
        Vector3 newTriggerPos = triggerObject.transform.position;
        newTriggerPos.z = zFinalNuevo - bNuevo.size.z * 0.25f;
        triggerObject.transform.position = newTriggerPos;
    }

    // 🔹 Controla el número máximo de tramos activos
    if (segmentosActivos.Count > maxSegmentosActivos)
    {
        GameObject viejo = segmentosActivos.Dequeue();
        if (viejo != carreteraOriginal)
            Destroy(viejo);
    }

    puedeRegenerar = false;
    Invoke(nameof(ReactivarGeneracion), cooldownGeneracion);

    Debug.Log($"✅ Nuevo tramo generado en Z={posNuevo.z}");
}

    bool TryGetBounds(GameObject go, out Bounds bounds)
    {
        var rends = go.GetComponentsInChildren<Renderer>();
        if (rends == null || rends.Length == 0)
        {
            bounds = new Bounds(go.transform.position, Vector3.zero);
            return false;
        }

        bounds = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
            bounds.Encapsulate(rends[i].bounds);

        return true;
    }

    void ReactivarGeneracion() => puedeRegenerar = true;

    private void OnTriggerEnter(Collider other)
    {
        // Si el trigger que colisiona es el triggerObject (lo más fiable),
        // o si el collider entrante tiene tag 'trigger', generamos.
        if (triggerObject != null && other.gameObject == triggerObject) 
        {
            GenerarNuevoMapa();
            return;
        }

        if (other.CompareTag("trigger"))
        {
            GenerarNuevoMapa();
            return;
        }

        // obstáculo
        if (other.CompareTag("obstaculo"))
        {
            velocidadActual = 0f;
        }
    }

    // --------------------------
    // UI: velocímetro, tiempo y puntos
    // --------------------------
    void ActualizarVelocimetro()
    {
        if (textoVelocidad == null) return;

        // suavizado visual (usa aceleracionSuave como factor)
        velocidadVisual = Mathf.Lerp(velocidadVisual, Mathf.Abs(velocidadActual), aceleracionSuave * Time.deltaTime);

        // conversión (si quieres 30 unidades => 250 km/h sustituye la fórmula)
        // aquí convertimos m/s -> km/h para mostrar (3.6)
        int kmh = Mathf.RoundToInt(velocidadVisual * 3.6f);
        kmh = Mathf.Clamp(kmh, 0, 250);

        if (velocidadActual < -0.1f)
        {
            textoVelocidad.text = "R " + kmh + " km/h";
            textoVelocidad.color = Color.cyan;
        }
        else
        {
            textoVelocidad.text = kmh + " km/h";
            if (kmh < 80) textoVelocidad.color = Color.green;
            else if (kmh < 160) textoVelocidad.color = Color.yellow;
            else textoVelocidad.color = Color.red;
        }
    }

    void ActualizarTemporizador()
    {
        if (!temporizadorActivo || textoTemporizador == null) return;
        tiempoTranscurrido += Time.deltaTime;
        int min = Mathf.FloorToInt(tiempoTranscurrido / 60f);
        int seg = Mathf.FloorToInt(tiempoTranscurrido % 60f);
        textoTemporizador.text = $"{min:00}:{seg:00}";
    }

    void ActualizarPuntos()
    {
        if (!temporizadorActivo || textoPuntos == null) return;
        if (Time.time - tiempoUltimoPunto >= intervaloPuntos)
        {
            puntosTotales += puntosPorSegundo;
            tiempoUltimoPunto = Time.time;
            textoPuntos.text = "Puntos: " + puntosTotales;
        }
    }

    // --------------------------
    // Input System config
    // --------------------------
    void ConfigurarInput()
    {
        movimientoAction = new InputAction("Movimiento", InputActionType.Value);
        movimientoAction.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        acelerarAction = new InputAction("Acelerar", InputActionType.Button);
        acelerarAction.AddBinding("<Keyboard>/w");
        acelerarAction.AddBinding("<Keyboard>/upArrow");

        frenarAction = new InputAction("Frenar", InputActionType.Button);
        frenarAction.AddBinding("<Keyboard>/s");
        frenarAction.AddBinding("<Keyboard>/downArrow");

        movimientoAction.Enable();
        acelerarAction.Enable();
        frenarAction.Enable();
    }

    void OnDestroy()
    {
        movimientoAction?.Disable();
        acelerarAction?.Disable();
        frenarAction?.Disable();
    }
}
