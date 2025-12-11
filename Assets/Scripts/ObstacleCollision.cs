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
            GuardarDatosDelJugador(other.gameObject);
            SceneManager.LoadScene("Derrota");
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log("Colisión con jugador detectada (OnCollisionEnter)");
            GuardarDatosDelJugador(collision.gameObject);
            SceneManager.LoadScene("Derrota");
        }
    }
    
    private void GuardarDatosDelJugador(GameObject jugador)
    {
        try
        {
            PlayerMovement playerScript = jugador.GetComponent<PlayerMovement>();
            
            if (playerScript != null)
            {
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
                    
                    // CALCULAR COINS: 100 coins por cada 5 segundos
                    int coins = CalcularCoinsPorTiempo(tiempo);
                    
                    // Guardar datos de la partida
                    PlayerPrefs.SetInt("PUNTOS_FINALES", puntos);
                    PlayerPrefs.SetFloat("TIEMPO_FINAL", tiempo);
                    PlayerPrefs.SetInt("COINS_FINALES", coins);
                    PlayerPrefs.Save();
                    
                    Debug.Log($"Datos guardados: {puntos} puntos, {tiempo:F1} segundos, {coins} coins");
                    Debug.Log($"Cálculo: {Mathf.Floor(tiempo / 5f)} intervalos de 5s × 100 coins = {coins} coins");
                }
                else
                {
                    GuardarValoresPorDefecto();
                }
            }
        }
        catch
        {
            GuardarValoresPorDefecto();
        }
    }
    
    private int CalcularCoinsPorTiempo(float tiempoSegundos)
    {
        // Calcular cuántos intervalos completos de 5 segundos hubo
        int intervalos5Segundos = Mathf.FloorToInt(tiempoSegundos / 5f);
        
        // 100 coins por cada intervalo de 5 segundos
        int coins = intervalos5Segundos * 100;
        
        // Bonus mínimo: si jugó menos de 5 segundos, gana 20 coins por segundo
        if (intervalos5Segundos == 0 && tiempoSegundos > 0)
        {
            coins = Mathf.FloorToInt(tiempoSegundos) * 20;
        }
        
        return coins;
    }
    
    private void GuardarValoresPorDefecto()
    {
        float tiempo = Time.timeSinceLevelLoad;
        int coins = CalcularCoinsPorTiempo(tiempo);
        
        PlayerPrefs.SetInt("PUNTOS_FINALES", 150);
        PlayerPrefs.SetFloat("TIEMPO_FINAL", tiempo);
        PlayerPrefs.SetInt("COINS_FINALES", coins);
        PlayerPrefs.Save();
        
        Debug.Log($"Valores por defecto: 150 puntos, {tiempo:F1} segundos, {coins} coins");
    }
}