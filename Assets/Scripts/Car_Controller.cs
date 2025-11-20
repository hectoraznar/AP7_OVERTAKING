using UnityEngine;

public class Car_Controller : MonoBehaviour
{
        [Header("Configuración de Velocidades")]
    public float velocidadAdelante = 10f;
    public float velocidadAtras = 5f;
    public float velocidadLateral = 3f;
    
    [Header("Configuración de Ejes")]
    public string ejeHorizontal = "Horizontal";
    public string ejeVertical = "Vertical";
    public string ejeAcelerar = "Fire1"; // Click izquierdo o Ctrl
    public string ejeFrenar = "Fire2";   // Click derecho o Alt

    private Rigidbody rb;
    private Vector3 posicionInicialCoche;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Si no hay Rigidbody, agregamos uno
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configurar Rigidbody para coche
        rb.constraints = RigidbodyConstraints.FreezeRotation;
       // rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Guardar posición inicial para mantener altura
        posicionInicialCoche = transform.position;
    }

    void FixedUpdate()
    {
        // Leer inputs usando GetAxis
        float inputHorizontal = Input.GetAxis(ejeHorizontal);
        float inputVertical = Input.GetAxis(ejeVertical);
        //float acel = Input.GetAxis(ejeAcelerar);
        float fren = Input.GetAxis(ejeFrenar);

        Vector3 movimiento = Vector3.zero;

        // MOVIMIENTO ADELANTE/ATRÁS
        if (inputVertical > 0.1f)
        {
            movimiento.z = velocidadAdelante * Time.fixedDeltaTime;
        }
        else if (fren > 0.1f)
        {
            movimiento.z = -velocidadAtras * Time.fixedDeltaTime;
        }

        // MOVIMIENTO LATERAL SOLO SI VA HACIA ADELANTE
        if (Mathf.Abs(inputHorizontal) > 0.1f && inputVertical > 0.1f)
        {
            movimiento.x = inputHorizontal * velocidadLateral * Time.fixedDeltaTime;
        }

        // USAR MovePosition PARA RESPETAR COLISIONES
        if (movimiento != Vector3.zero)
        {
            Vector3 nuevaPosicion = rb.position + movimiento;
            rb.MovePosition(nuevaPosicion);
        }

        // Mantener altura constante
        Vector3 pos = rb.position;
        pos.y = posicionInicialCoche.y;
        rb.position = pos;
    }

}
