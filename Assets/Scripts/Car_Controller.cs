using UnityEngine;

public class Car_Controller : MonoBehaviour
{
 [Header("Configuración del Coche")]
    public float potenciaAceleracion = 800f;     // Fuerza al acelerar
    public float potenciaFrenado = 1000f;        // Fuerza al frenar
    public float velocidadMaxima = 30f;          // m/s (~108 km/h)
    public float velocidadMaximaReversa = 15f;   // m/s (~54 km/h)
    public float friccionDesaceleracion = 0.95f; // Desaceleración natural (0.9 = más rápido)
    public float sensibilidadGiro = 2f;         // Giro por segundo

    private Rigidbody rb;
    private float inputAceleracion;
    private float inputGiro;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Más estable
    }

    void Update()
    {
        // Capturar input
        inputAceleracion = Input.GetAxis("Vertical");   // W/S o ↑/↓
        inputGiro = Input.GetAxis("Horizontal");        // A/D o ←/→
    

    }
    void FixedUpdate(){
        AplicarAceleracionYFrenado();
       AplicarGiro();
        AplicarDesaceleracionNatural();
        LimitarVelocidad();
    }
    
    void AplicarAceleracionYFrenado()
    {
        Vector3 fuerza = transform.forward * inputAceleracion * potenciaAceleracion * Time.fixedDeltaTime;
        rb.AddForce(fuerza, ForceMode.Acceleration);
    }

    void AplicarGiro()
    {
        if (inputAceleracion != 0 || rb.linearVelocity.magnitude > 1f)
        {
            float giro = inputGiro * sensibilidadGiro * Time.fixedDeltaTime * 60f;
            transform.Rotate(0, giro, 0);
        }
    }

    void AplicarDesaceleracionNatural()
    {
        if (Mathf.Abs(inputAceleracion) < 0.1f && Mathf.Abs(inputGiro) < 0.1f)
        {
            Vector3 velocidad = rb.linearVelocity;
            velocidad *= friccionDesaceleracion;
            rb.linearVelocity = velocidad;
        }
    }

    void LimitarVelocidad()
    {
        Vector3 velocidad = rb.linearVelocity;
        float velocidadActual = velocidad.magnitude;

        if (inputAceleracion > 0 && velocidadActual > velocidadMaxima)
        {
            rb.linearVelocity = velocidad.normalized * velocidadMaxima;
        }
        else if (inputAceleracion < 0 && velocidadActual > velocidadMaximaReversa)
        {
            rb.linearVelocity = velocidad.normalized * velocidadMaximaReversa;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("obstaculo"))
        {
            // Lógica de colisión con un obstáculo
            Debug.Log("¡Colisión con un obstáculo!");
        }
    }
}
