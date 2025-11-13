using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class movimientoCoche : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camara;
    public GameObject carreteraOriginal;
    public GameObject triggerObject;

    [Header("Movimiento - Con CharacterController")]
    public float velocidadLateral = 15f;
    public float velocidadAdelante = 25f;
    public float velocidadAtras = 12f;

    [Header("UI")]
    public TextMeshProUGUI textoVelocidad;
    public TextMeshProUGUI textoTemporizador;
    public TextMeshProUGUI textoPuntos;

    [Header("Puntos y tiempo")]
    public int puntosPorSegundo = 5;
    public float intervaloPuntos = 1f;

    [Header("Mapa Infinito")]
    public float cooldownGeneracion = 0.6f;
    public int maxSegmentosActivos = 6;

    private InputAction movimientoAction;
    private InputAction acelerarAction;
    private InputAction frenarAction;

    private CharacterController controller;
    private float velocidadVisual;
    private float tiempoTranscurrido;
    private int puntosTotales;
    private float tiempoUltimoPunto;

    private Queue<GameObject> segmentosActivos = new Queue<GameObject>();
    private bool puedeRegenerar = true;
    private Vector3 posicionInicialCoche;

    void Awake()
    {
        ConfigurarInput();
        controller = GetComponent<CharacterController>();
        posicionInicialCoche = transform.position;
    }

    void Start()
    {
        if (camara == null) camara = Camera.main;

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
        
        if (carreteraOriginal != null)
            segmentosActivos.Enqueue(carreteraOriginal);
    }

    void Update()
    {
        MovimientoConColisiones();
        ActualizarTemporizador();
        ActualizarPuntos();
        ActualizarVelocimetro();
    }

    void MovimientoConColisiones()
    {
        Vector2 input = movimientoAction.ReadValue<Vector2>();
        float acel = acelerarAction.ReadValue<float>();
        float fren = frenarAction.ReadValue<float>();

        Vector3 movimiento = Vector3.zero;

        // MOVIMIENTO ADELANTE/ATRÁS
        if (acel > 0.1f)
        {
            movimiento.z = velocidadAdelante * Time.deltaTime;
        }
        else if (fren > 0.1f)
        {
            movimiento.z = -velocidadAtras * Time.deltaTime;
        }

        // MOVIMIENTO LATERAL SOLO SI VA HACIA ADELANTE
        if (Mathf.Abs(input.x) > 0.1f && acel > 0.1f)
        {
            movimiento.x = input.x * velocidadLateral * Time.deltaTime;
        }

        // APLICAR MOVIMIENTO CON DETECCIÓN DE COLISIONES
        if (movimiento != Vector3.zero)
        {
            // CharacterController.Move() detecta colisiones automáticamente
            controller.Move(movimiento);
        }

        // Mantener altura constante
        Vector3 pos = transform.position;
        pos.y = posicionInicialCoche.y;
        transform.position = pos;
    }

    // --------------------------
    // Mapa Infinito (igual)
    // --------------------------
    void GenerarNuevoMapa()
    {
        if (!puedeRegenerar || carreteraOriginal == null) return;

        GameObject ultimo = segmentosActivos.Last();

        Bounds bBase;
        TryGetBounds(carreteraOriginal, out bBase);
        float largo = bBase.size.z;

        Vector3 posNuevo = new Vector3(
            ultimo.transform.position.x,
            ultimo.transform.position.y,
            ultimo.transform.position.z + largo
        );

        GameObject nuevo = Instantiate(carreteraOriginal, posNuevo, ultimo.transform.rotation);
        nuevo.name = "carretera_copia_" + System.DateTime.Now.Ticks;
        segmentosActivos.Enqueue(nuevo);

        if (triggerObject != null)
        {
            Bounds bNuevo;
            TryGetBounds(nuevo, out bNuevo);
            Vector3 newTriggerPos = triggerObject.transform.position;
            newTriggerPos.z = nuevo.transform.position.z + (bNuevo.size.z / 2f);
            triggerObject.transform.position = newTriggerPos;
        }

        if (segmentosActivos.Count > maxSegmentosActivos)
        {
            GameObject viejo = segmentosActivos.Dequeue();
            if (viejo != carreteraOriginal)
                Destroy(viejo);
        }

        puedeRegenerar = false;
        Invoke(nameof(ReactivarGeneracion), cooldownGeneracion);
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
        if ((triggerObject != null && other.gameObject == triggerObject) || other.CompareTag("trigger"))
        {
            GenerarNuevoMapa();
        }
    }

    // --------------------------
    // UI
    // --------------------------
    void ActualizarVelocimetro()
    {
        if (textoVelocidad == null) return;

        float acel = acelerarAction.ReadValue<float>();
        float fren = frenarAction.ReadValue<float>();

        float velocidadActual = 0f;
        if (acel > 0.1f) velocidadActual = velocidadAdelante;
        else if (fren > 0.1f) velocidadActual = -velocidadAtras;

        float velocidadKmh = Mathf.Abs(velocidadActual) * 3.6f;
        velocidadVisual = Mathf.Lerp(velocidadVisual, velocidadKmh, Time.deltaTime * 8f);

        int kmh = Mathf.RoundToInt(velocidadVisual);
        
        if (velocidadActual < 0)
        {
            textoVelocidad.text = "R " + kmh + " km/h";
            textoVelocidad.color = Color.cyan;
        }
        else
        {
            textoVelocidad.text = kmh + " km/h";
            textoVelocidad.color = kmh < 80 ? Color.green : 
                                 kmh < 160 ? Color.yellow : 
                                 Color.red;
        }
    }

    void ActualizarTemporizador()
    {
        if (textoTemporizador == null) return;
        tiempoTranscurrido += Time.deltaTime;
        int min = Mathf.FloorToInt(tiempoTranscurrido / 60f);
        int seg = Mathf.FloorToInt(tiempoTranscurrido % 60f);
        textoTemporizador.text = $"{min:00}:{seg:00}";
    }

    void ActualizarPuntos()
    {
        if (textoPuntos == null) return;
        if (Time.time - tiempoUltimoPunto >= intervaloPuntos)
        {
            puntosTotales += puntosPorSegundo;
            tiempoUltimoPunto = Time.time;
            textoPuntos.text = "Puntos: " + puntosTotales;
        }
    }

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