using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Configuración")]
    public string playerTag = "player";
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Colisión con jugador detectada");
            GuardarPuntosDelJugador(other.gameObject);
            SceneManager.LoadScene("Derrota");
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log("Colisión con jugador detectada (OnCollisionEnter)");
            GuardarPuntosDelJugador(collision.gameObject);
            SceneManager.LoadScene("Derrota");
        }
    }
    
    private void GuardarPuntosDelJugador(GameObject jugador)
    {
        try
        {
            PlayerMovement playerScript = jugador.GetComponent<PlayerMovement>();
            
            if (playerScript != null)
            {
                // Intentar acceder a los campos con reflexión
                System.Type tipo = playerScript.GetType();
                
                System.Reflection.FieldInfo campoPuntos = tipo.GetField(
                    "puntosTotales", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance
                );
                
                System.Reflection.FieldInfo campoTiempo = tipo.GetField(
                    "tiempoTranscurrido",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance
                );
                
                if (campoPuntos != null && campoTiempo != null)
                {
                    int puntos = (int)campoPuntos.GetValue(playerScript);
                    float tiempo = (float)campoTiempo.GetValue(playerScript);
                    int coins = puntos * 2;
                    
                    PlayerPrefs.SetInt("PUNTOS_FINALES", puntos);
                    PlayerPrefs.SetFloat("TIEMPO_FINAL", tiempo);
                    PlayerPrefs.SetInt("COINS_FINALES", coins);
                    PlayerPrefs.Save();
                    
                    Debug.Log("Datos guardados: " + puntos + " puntos, " + coins + " coins");
                }
                else
                {
                    // Valores por defecto si no encuentra los campos
                    PlayerPrefs.SetInt("PUNTOS_FINALES", 150);
                    PlayerPrefs.SetFloat("TIEMPO_FINAL", Time.timeSinceLevelLoad);
                    PlayerPrefs.SetInt("COINS_FINALES", 300);
                    PlayerPrefs.Save();
                }
            }
        }
        catch
        {
            // Si hay error, guardar valores básicos
            PlayerPrefs.SetInt("PUNTOS_FINALES", 100);
            PlayerPrefs.SetFloat("TIEMPO_FINAL", Time.timeSinceLevelLoad);
            PlayerPrefs.SetInt("COINS_FINALES", 200);
            PlayerPrefs.Save();
        }
    }
}