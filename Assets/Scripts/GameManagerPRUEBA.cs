using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    
    private int puntosFinales = 0;
    private float tiempoFinal = 0f;
    
    void Awake()
    {
        // Singleton pattern - solo una instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Método para guardar datos cuando el jugador pierde
    public void GuardarDatosFinales(int puntos, float tiempo)
    {
        puntosFinales = puntos;
        tiempoFinal = tiempo;
        Debug.Log($"Datos guardados: {puntos} puntos, {tiempo} segundos");
    }
    
    // Métodos para obtener los datos en la pantalla de derrota
    public int GetPuntosFinales() => puntosFinales;
    public float GetTiempoFinal() => tiempoFinal;
    public string GetTiempoFormateado()
    {
        int minutos = Mathf.FloorToInt(tiempoFinal / 60f);
        int segundos = Mathf.FloorToInt(tiempoFinal % 60f);
        return $"{minutos:00}:{segundos:00}";
    }
}