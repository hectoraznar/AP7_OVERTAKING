using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    public Transform coche;
    public Vector3 offset = new Vector3(-1f, 4.3f, -15f); // Posición fija relativa al coche
    public float suavizado = 3f;

    private Vector3 velocidad = Vector3.zero;

    void Start()
    {
        // Buscar automáticamente el coche si no está asignado
        if (coche == null)
        {
            coche = GameObject.Find("Coche").transform;
            if (coche == null)
            {
                Debug.LogError("No se encontró el objeto 'Coche' en la escena!");
            }
        }
        
        // Posicionar la cámara correctamente al inicio
        PosicionarCamara();
    }

    void LateUpdate()
    {
        if (coche != null)
        {
            SeguirCoche();
        }
    }

    void SeguirCoche()
    {
        // Calcular la posición objetivo manteniendo el offset relativo al coche
        Vector3 posicionObjetivo = coche.position + offset;
        
        // Aplicar suavizado solo al movimiento horizontal (X y Z)
        Vector3 posicionSuavizada = Vector3.SmoothDamp(
            transform.position, 
            posicionObjetivo, 
            ref velocidad, 
            suavizado * Time.deltaTime
        );

        // Mantener la altura fija (Y siempre en 4.3)
        posicionSuavizada.y = offset.y;

        transform.position = posicionSuavizada;

        // Mantener la rotación fija (como en el Inspector)
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void PosicionarCamara()
    {
        if (coche != null)
        {
            // Posicionar inmediatamente en la posición correcta
            transform.position = coche.position + offset;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Resetear a la posición inicial
    public void ResetearCamara()
    {
        PosicionarCamara();
    }
}