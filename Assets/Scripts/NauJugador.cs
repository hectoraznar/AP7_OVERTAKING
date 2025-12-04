using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 500f; // Velocidad de movimiento lateral base
    public float forwardSpeed = 40f; // Velocidad base hacia adelante
    public float maxForwardSpeed = 160f; // Velocidad máxima hacia adelante
    public float acceleration = 10f; // Qué tan rápido acelera
    public float normalDeceleration = 4f; // Deceleración normal cuando sueltas W
    public float brakeDeceleration = 2000000f; // Deceleración cuando presionas S
    public float minLateralMultiplier = 0.3f; // Mínimo movimiento lateral (a velocidad 0)
    public float maxLateralSpeed = 20f; // Velocidad a la que se alcanza el giro completo

    [Header("Mapa Infinito")]
    public GameObject carreteraOriginal;
    public GameObject triggerObject;
    public float cooldownGeneracion = 0.6f;
    public int maxSegmentosActivos = 6;

    [Header("UI")]
    public TextMeshProUGUI textoVelocidad;
    public TextMeshProUGUI textoTemporizador;
    public TextMeshProUGUI textoPuntos;

    [Header("Puntos")]
    public int puntosPorSegundo = 5;
    public float intervaloPuntos = 1f;
    [Header("Sistema de Coins")]
    public int multiplicadorCoins = 2; // Multiplicador de coins (por defecto x2)
    public int coins = 0; // Coins totales
    public TextMeshProUGUI textoCoins; // UI para mostrar coins

    /*
    pasar el script del spawn; 

    falta implementar logica de cada x puntos llamo al metodo de reducir timepo del spawner y le dices cuanto tiempo tienes que reducir. ej= 0.2f, 0.5f, 1f;
    */
    private float currentForwardSpeed = 0f; // Velocidad actual hacia adelante
    private float tiempoTranscurrido = 0f;
    private int puntosTotales = 0;
    private float tiempoUltimoPunto = 0f;
    private float velocidadVisual = 0f;
    private float alturaFija; // Para mantener la altura constante

    // Mapa Infinito
    private Queue<GameObject> segmentosActivos = new Queue<GameObject>();
    private bool puedeRegenerar = true;

    // Input System
    private InputAction movimientoAction;
    private InputAction acelerarAction;
    private InputAction frenarAction;

    void Start()
    {
        ConfigurarInput();

        // Guardar la altura inicial para mantenerla fija
        alturaFija = transform.position.y;

        // Buscar referencias UI si no están asignadas
        if (textoVelocidad == null)
            textoVelocidad = GameObject.Find("textoVelocidad")?.GetComponent<TextMeshProUGUI>();
        if (textoTemporizador == null)
            textoTemporizador = GameObject.Find("textoTemporizador")?.GetComponent<TextMeshProUGUI>();
        if (textoPuntos == null)
            textoPuntos = GameObject.Find("textoPuntos")?.GetComponent<TextMeshProUGUI>();

        // Inicializar mapa infinito
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

        if (carreteraOriginal != null)
        {
            segmentosActivos.Enqueue(carreteraOriginal);
            Debug.Log("Mapa infinito inicializado con carretera original");
        }
        else
        {
            Debug.LogError("No se encontró carreteraOriginal!");
        }
    }

    void Update()
    {
        // MOVIMIENTO LATERAL (A y D) - SOLO si el coche tiene velocidad
        if (currentForwardSpeed > 0f)
        {
            Vector2 input = movimientoAction.ReadValue<Vector2>();
            float horizontalInput = input.x;

            // Calcular multiplicador de movimiento lateral basado en la velocidad
            float speedRatio = Mathf.Clamp01(currentForwardSpeed / maxLateralSpeed);
            float lateralMultiplier = Mathf.Lerp(minLateralMultiplier, 1.0f, speedRatio);

            Vector3 lateralMovement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * lateralMultiplier * Time.deltaTime;
            transform.Translate(lateralMovement);
        }

        // CONTROL DE ACELERACIÓN con Input System
        float acel = acelerarAction.ReadValue<float>();
        float fren = frenarAction.ReadValue<float>();

        if (acel > 0.1f)
        {
            // Aumentar velocidad progresivamente
            currentForwardSpeed += acceleration * Time.deltaTime;
            currentForwardSpeed = Mathf.Min(currentForwardSpeed, maxForwardSpeed);
        }

        // APLICAR DECELERACIÓN (normal o de frenado)
        float currentDeceleration = normalDeceleration;

        if (fren > 0.1f)
        {
            // Usar deceleración de frenado cuando se presiona S
            currentDeceleration = brakeDeceleration;
        }

        // Solo aplicar deceleración si no estamos acelerando
        if (acel <= 0.1f)
        {
            currentForwardSpeed -= currentDeceleration * Time.deltaTime;
            currentForwardSpeed = Mathf.Max(currentForwardSpeed, 0f);
        }

        // APLICAR MOVIMIENTO HACIA ADELANTE
        Vector3 forwardMovement = new Vector3(0f, 0f, currentForwardSpeed) * Time.deltaTime;
        transform.Translate(forwardMovement);

        // BLOQUEAR ALTURA - CORREGIR POSICIÓN Y
        BloquearAltura();

        // ACTUALIZAR UI
        ActualizarVelocimetro();
        ActualizarTemporizador();
        ActualizarPuntos();
    }

    void BloquearAltura()
    {
        // Forzar la posición Y a mantenerse constante
        Vector3 posicionActual = transform.position;
        if (Mathf.Abs(posicionActual.y - alturaFija) > 0.01f)
        {
            posicionActual.y = alturaFija;
            transform.position = posicionActual;
        }
    }

    // --------------------------
    // MAPA INFINITO - CORREGIDO
    // --------------------------
    void GenerarNuevoMapa()
    {
        if (!puedeRegenerar || carreteraOriginal == null)
        {
            Debug.Log("No se puede regenerar: puedeRegenerar=" + puedeRegenerar + ", carreteraOriginal=" + (carreteraOriginal != null));
            return;
        }

        // OBTENER EL ÚLTIMO SEGMENTO CORRECTAMENTE
        GameObject ultimoSegmento = segmentosActivos.ToArray()[segmentosActivos.Count - 1];
        Debug.Log("Generando nuevo segmento después de: " + ultimoSegmento.name);

        Bounds bBase;
        TryGetBounds(carreteraOriginal, out bBase);
        float largo = bBase.size.z;

        Vector3 posNuevo = new Vector3(
            ultimoSegmento.transform.position.x,
            ultimoSegmento.transform.position.y,
            ultimoSegmento.transform.position.z + largo
        );

        GameObject nuevo = Instantiate(carreteraOriginal, posNuevo, ultimoSegmento.transform.rotation);
        nuevo.name = "carretera_copia_" + System.DateTime.Now.Ticks;
        segmentosActivos.Enqueue(nuevo);

        // ACTUALIZAR POSICIÓN DEL TRIGGER
        if (triggerObject != null)
        {
            Bounds bNuevo;
            TryGetBounds(nuevo, out bNuevo);
            Vector3 newTriggerPos = triggerObject.transform.position;
            newTriggerPos.z = nuevo.transform.position.z + (bNuevo.size.z / 2f);
            triggerObject.transform.position = newTriggerPos;
            Debug.Log("Trigger movido a posición Z: " + newTriggerPos.z);
        }

        // LIMPIAR SEGMENTOS ANTIGUOS
        if (segmentosActivos.Count > maxSegmentosActivos)
        {
            GameObject viejo = segmentosActivos.Dequeue();
            if (viejo != carreteraOriginal)
            {
                Destroy(viejo);
                Debug.Log("Segmento viejo destruido: " + viejo.name);
            }
        }

        puedeRegenerar = false;
        Invoke(nameof(ReactivarGeneracion), cooldownGeneracion);

        Debug.Log("Nuevo segmento generado. Total activos: " + segmentosActivos.Count);
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

    void ReactivarGeneracion()
    {
        puedeRegenerar = true;
        Debug.Log("Generación reactivada");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered: " + other.gameObject.name + ", Tag: " + other.tag);

        if ((triggerObject != null && other.gameObject == triggerObject) || other.CompareTag("trigger"))
        {
            Debug.Log("Generando nuevo mapa por trigger...");
            GenerarNuevoMapa();
        }
    }

    // --------------------------
    // SISTEMA DE UI
    // --------------------------
    void ActualizarVelocimetro()
    {
        if (textoVelocidad == null) return;

        // Convertir velocidad a km/h
        float velocidadKmh = Mathf.Abs(currentForwardSpeed) * 3.6f;
        velocidadVisual = Mathf.Lerp(velocidadVisual, velocidadKmh, Time.deltaTime * 8f);

        int kmh = Mathf.RoundToInt(velocidadVisual);

        // Mostrar velocidad
        textoVelocidad.text = kmh + " km/h";

        // Cambiar color según velocidad
        if (kmh < 30)
            textoVelocidad.color = Color.green;
        else if (kmh < 60)
            textoVelocidad.color = Color.yellow;
        else
            textoVelocidad.color = Color.red;
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

    // SOLO sumar puntos si nos estamos moviendo (velocidad > 0.1)
    if (currentForwardSpeed > 0.1f)
    {
        if (Time.time - tiempoUltimoPunto >= intervaloPuntos)
        {
            puntosTotales += puntosPorSegundo;

            // Calcular coins (puntos multiplicados por el multiplicador)
            coins = puntosTotales * multiplicadorCoins;

            tiempoUltimoPunto = Time.time;
            textoPuntos.text = "Puntos: " + puntosTotales;

            // Actualizar texto de coins si está asignado
            if (textoCoins != null)
            {
                textoCoins.text = "Coins: " + coins;
            }
        }
    }
    else
    {
        // Si estamos parados, resetear el contador para que no sume
        // puntos inmediatamente al empezar a moverse
        tiempoUltimoPunto = Time.time;
    }
}

    // Añade este método para acceder a las coins desde otros scripts:
    public int ObtenerCoins()
    {
        return coins;
    }

    public int ObtenerPuntos()
    {
        return puntosTotales;
    }

    // --------------------------
    // INPUT SYSTEM
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