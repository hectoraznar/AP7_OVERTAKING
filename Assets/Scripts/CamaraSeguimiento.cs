using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    private Transform coche;
    public float distanciaAtras = 5f;
    public float altura = 2f;
    public float suavizado = 2f;

    private Vector3 velocidadCamara = Vector3.zero;
    private float posicionXInicial;

    void Start()
    {
        // Buscar el objeto llamado "Coche"
        coche = GameObject.Find("Coche").transform;
        
        // Guardar la posición X inicial de la cámara
        posicionXInicial = transform.position.x;
    }

    void LateUpdate()
    {
        if (coche != null)
        {
            // Solo seguir en eje Z (hacia adelante/atrás) y mantener altura
            // Mantener siempre la misma posición X inicial
            Vector3 posicionObjetivo = new Vector3(
                posicionXInicial,                    // Misma X siempre
                coche.position.y + altura,           // Seguir altura del coche
                coche.position.z - distanciaAtras    // Seguir detrás en Z
            );
            
            // Aplicar suavizado solo en los ejes que seguimos
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                posicionObjetivo, 
                ref velocidadCamara, 
                suavizado * Time.deltaTime
            );

            // Mantener la rotación fija (mirando hacia adelante)
            transform.rotation = Quaternion.Euler(15f, 0f, 0f); // Ajusta el ángulo si quieres
        }
    }
}