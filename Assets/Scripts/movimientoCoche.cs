using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class movimientoCoche : MonoBehaviour
{
    [Header("Configuración del coche - MÁS VELOCIDAD PERO SUAVE")]
    public float velocidadBase = 30f;
    public float aceleracion = 50f;
    public float frenado = 12f;
    public float velocidadActual = 0f;
    public float velocidadGiro = 7f;
    public float aceleracionSuave = 1.5f;

    [Header("UI - Velocímetro")]
    public TextMeshProUGUI textoVelocidad;

    [Header("Temporizador")]
    public TextMeshProUGUI textoTemporizador;
    private float tiempoTranscurrido = 0f;
    private bool temporizadorActivo = true;

    [Header("Sistema de Puntos")]
    public TextMeshProUGUI textoPuntos;
    private int puntosTotales = 0;
    private float tiempoUltimoPunto = 0f;
    public int puntosPorSegundo = 5;
    public float intervaloPuntos = 1f; // Cada 1 segundo

    [Header("Giro Realista")]
    public float giroMaximo = 6f;
    public float giroMinimo = 2f;
    public float velocidadReferencia = 40f;

    [Header("Límites de carretera")]
    private float limiteIzquierdo;
    private float limiteDerecho;
    private Camera camara;

    // Variables del nuevo Input System
    private InputAction movimientoAction;
    private InputAction acelerarAction;
    private InputAction frenarAction;

    void Start()
    {
        camara = Camera.main;
        velocidadActual = 0f;

        // Configurar límites de la cámara
        float distanciaZ = Mathf.Abs(transform.position.z - camara.transform.position.z);
        Vector3 limiteInferior = camara.ViewportToWorldPoint(new Vector3(0, 0, distanciaZ));
        Vector3 limiteSuperior = camara.ViewportToWorldPoint(new Vector3(1, 1, distanciaZ));

        limiteIzquierdo = limiteInferior.x + 1f;
        limiteDerecho = limiteSuperior.x - 1f;

        // Configurar el nuevo Input System
        ConfigurarInput();
    }

    void Update()
    {
        ActualizarTemporizador();
        ActualizarSistemaPuntos(); // ← NUEVO: Sistema de puntos
        ActualizarVelocimetro();
        Debug.Log("Velocidad: " + velocidadActual.ToString("F1"));
        MoverCoche();
        LimitarPosicion();
    }

    void ActualizarTemporizador()
    {
        if (temporizadorActivo && textoTemporizador != null)
        {
            tiempoTranscurrido += Time.deltaTime;
            
            int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60f);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60f);
            
            textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    void ActualizarSistemaPuntos()
    {
        if (temporizadorActivo && textoPuntos != null)
        {
            // Sumar puntos cada X segundos
            if (Time.time - tiempoUltimoPunto >= intervaloPuntos)
            {
                puntosTotales += puntosPorSegundo;
                tiempoUltimoPunto = Time.time;
                
                // Actualizar texto de puntos
                textoPuntos.text = "Puntos: " + puntosTotales;
                
                Debug.Log("+ " + puntosPorSegundo + " puntos! Total: " + puntosTotales);
            }
        }
    }

    void ActualizarVelocimetro()
    {
        if (textoVelocidad != null)
        {
            // CONVERSIÓN: 30 unidades = 250 km/h
            float velocidadNormalizada = Mathf.Abs(velocidadActual) / 30f;
            int velocidadKMH = Mathf.RoundToInt(velocidadNormalizada * 250f);
            velocidadKMH = Mathf.Min(velocidadKMH, 250);
            
            textoVelocidad.text = velocidadKMH + " km/h";
            
            if (velocidadKMH < 80)
                textoVelocidad.color = Color.green;
            else if (velocidadKMH < 160)
                textoVelocidad.color = Color.yellow;
            else
                textoVelocidad.color = Color.red;
        }
    }

    // MÉTODOS PARA CONTROLAR PUNTOS DESDE OTROS SCRIPTS
    public void SumarPuntos(int cantidad)
    {
        puntosTotales += cantidad;
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntosTotales;
    }

    public void RestarPuntos(int cantidad)
    {
        puntosTotales = Mathf.Max(0, puntosTotales - cantidad);
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntosTotales;
    }

    public void ReiniciarPuntos()
    {
        puntosTotales = 0;
        tiempoUltimoPunto = Time.time;
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntosTotales;
    }

    public int ObtenerPuntos()
    {
        return puntosTotales;
    }

    // El resto de tu código se mantiene igual...
    void ConfigurarInput()
    {
        // Crear acciones de input manualmente
        movimientoAction = new InputAction("Movimiento", InputActionType.Value);
        movimientoAction.AddCompositeBinding("2DVector(mode=1)")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        acelerarAction = new InputAction("Acelerar", InputActionType.Button);
        acelerarAction.AddBinding("<Keyboard>/upArrow");
        acelerarAction.AddBinding("<Keyboard>/w");

        frenarAction = new InputAction("Frenar", InputActionType.Button);
        frenarAction.AddBinding("<Keyboard>/downArrow");
        frenarAction.AddBinding("<Keyboard>/s");

        // Activar las acciones
        movimientoAction.Enable();
        acelerarAction.Enable();
        frenarAction.Enable();
    }

    void MoverCoche()
    {
        // Giro lateral con nuevo Input System
        Vector2 inputVector = movimientoAction.ReadValue<Vector2>();
        float direccionHorizontal = inputVector.x;

        // Control de velocidad
        ControlarVelocidad(inputVector);

        // SOLO girar si el coche se está moviendo
        if (Mathf.Abs(velocidadActual) > 0.1f)
        {
            // Calcular giro según la velocidad
            float giroEfectivo = CalcularGiroEfectivo();
            float movimientoX = direccionHorizontal * giroEfectivo * Time.deltaTime;
            transform.Translate(movimientoX, 0, 0);

            // Inclinación visual al girar (solo si hay movimiento)
            if (direccionHorizontal != 0)
            {
                float inclinacionEfectiva = CalcularInclinacionEfectiva();
                transform.rotation = Quaternion.Euler(0, direccionHorizontal * inclinacionEfectiva, 0);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 5f * Time.deltaTime);
            }
        }
        else
        {
            // SI ESTÁ PARADO: No girar y mantener rotación neutral
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 5f * Time.deltaTime);
        }

        // Aplicar movimiento hacia adelante/atrás (solo si hay velocidad)
        if (velocidadActual != 0)
        {
            float movimientoZ = velocidadActual * Time.deltaTime;
            transform.Translate(0, 0, movimientoZ);
        }
    }

    float CalcularGiroEfectivo()
    {
        float velocidadAbsoluta = Mathf.Abs(velocidadActual);
        float factorReduccion = Mathf.Clamp01(velocidadAbsoluta / velocidadReferencia);
        float giroEfectivo = Mathf.Lerp(giroMaximo, giroMinimo, factorReduccion);
        return giroEfectivo;
    }

    float CalcularInclinacionEfectiva()
    {
        float velocidadAbsoluta = Mathf.Abs(velocidadActual);
        float factorReduccion = Mathf.Clamp01(velocidadAbsoluta / velocidadReferencia);
        float inclinacionEfectiva = Mathf.Lerp(12f, 4f, factorReduccion);
        return inclinacionEfectiva;
    }

    void ControlarVelocidad(Vector2 input)
    {
        // Acelerar - MÁS LENTO PARA LLEGAR A LA MÁXIMA
        if (acelerarAction.ReadValue<float>() > 0 || input.y > 0)
        {
            velocidadActual = Mathf.Lerp(velocidadActual, aceleracion, aceleracionSuave * Time.deltaTime);
        }
        // Frenar/marcha atrás - MÁS SUAVE
        else if (frenarAction.ReadValue<float>() > 0 || input.y < 0)
        {
            if (velocidadActual > 0)
            {
                velocidadActual = Mathf.Lerp(velocidadActual, 0f, 2f * Time.deltaTime);
            }
            else
            {
                velocidadActual = Mathf.Lerp(velocidadActual, -frenado, 1f * Time.deltaTime);
            }
        }
        // Frenado natural - MÁS SUAVE
        else
        {
            velocidadActual = Mathf.Lerp(velocidadActual, 0f, 1.5f * Time.deltaTime);
            
            if (Mathf.Abs(velocidadActual) < 0.05f)
            {
                velocidadActual = 0f;
            }
        }
    }

    void LimitarPosicion()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limiteIzquierdo, limiteDerecho);
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("obstaculo"))
        {
            Debug.Log("Colisión con obstáculo");
            velocidadActual = 0f;
        }
    }

    void OnDestroy()
    {
        movimientoAction?.Disable();
        acelerarAction?.Disable();
        frenarAction?.Disable();
    }

    // Métodos opcionales para controlar el temporizador
    public void IniciarTemporizador()
    {
        temporizadorActivo = true;
    }

    public void PausarTemporizador()
    {
        temporizadorActivo = false;
    }

    public void ReiniciarTemporizador()
    {
        tiempoTranscurrido = 0f;
        temporizadorActivo = true;
    }

    public void DetenerTemporizador()
    {
        temporizadorActivo = false;
        tiempoTranscurrido = 0f;
    }
}