using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    private Transform coche;
    public float distanciaAtras = 5f;
    public float altura = 2f;
    public float suavizado = 2f;
    public float suavizadoRotacion = 2f;

    private Vector3 velocidadCamara = Vector3.zero;
    private float velocidadRotacion = 0f;

    void Start()
    {
        // Buscar el objeto llamado "Coche"
        coche = GameObject.Find("Coche").transform;
        
        if (coche == null)
        {
            Debug.LogError("No se encontró el objeto 'Coche' en la escena!");
        }
    }

    void LateUpdate()
    {
        if (coche != null)
        {
            SeguirCocheConRotacion();
        }
    }

    void SeguirCocheConRotacion()
    {
        // Calcular posición objetivo detrás del coche
        Vector3 direccionAtras = -coche.forward * distanciaAtras;
        Vector3 posicionObjetivo = coche.position + direccionAtras + Vector3.up * altura;
        
        // Suavizado de posición
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            posicionObjetivo, 
            ref velocidadCamara, 
            suavizado * Time.deltaTime
        );

        // Calcular rotación objetivo (mirar hacia el coche con un poco de adelanto)
        Vector3 direccionMirar = coche.position - transform.position;
        
        // Añadir un poco de adelanto en la dirección que mira el coche
        direccionMirar += coche.forward * 2f;
        
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMirar, Vector3.up);

        // Suavizado de rotación
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            rotacionObjetivo, 
            suavizadoRotacion * Time.deltaTime
        );
    }

    // Método alternativo más simple (elige uno)
    void SeguirCocheSimple()
    {
        // Posición: siempre detrás del coche en su dirección
        Vector3 posicionObjetivo = coche.position - coche.forward * distanciaAtras + Vector3.up * altura;
        
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            posicionObjetivo, 
            ref velocidadCamara, 
            suavizado * Time.deltaTime
        );

        // Rotación: siempre mirar al coche
        transform.LookAt(coche.position + coche.forward * 2f); // Mirar un poco adelante del coche
    }

    // Método para ajustar dinámicamente los parámetros
    public void ConfigurarSeguimiento(float nuevaDistancia, float nuevaAltura, float nuevoSuavizado)
    {
        distanciaAtras = nuevaDistancia;
        altura = nuevaAltura;
        suavizado = nuevoSuavizado;
    }

    // Método para resetear la cámara
    public void ResetearCamara()
    {
        if (coche != null)
        {
            Vector3 posicionInicial = coche.position - coche.forward * distanciaAtras + Vector3.up * altura;
            transform.position = posicionInicial;
            transform.LookAt(coche.position + coche.forward * 2f);
        }
    }
}