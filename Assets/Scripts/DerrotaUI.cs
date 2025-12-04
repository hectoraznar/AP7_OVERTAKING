using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DerrotaUI : MonoBehaviour
{
    [Header("Textos UI")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoCoins;
    public TextMeshProUGUI textoTiempo;
    
    [Header("Botones")]
    public Button botonReiniciar;
    public Button botonMenu;
    
    void Start()
    {
        // Configurar botones
        if (botonReiniciar != null)
            botonReiniciar.onClick.AddListener(ReiniciarJuego);
        
        if (botonMenu != null)
            botonMenu.onClick.AddListener(IrAlMenu);
        
        // Mostrar datos
        MostrarDatos();
    }
    
    void MostrarDatos()
    {
        // Obtener datos guardados
        int puntos = PlayerPrefs.GetInt("PUNTOS_FINALES", 0);
        int coins = PlayerPrefs.GetInt("COINS_FINALES", 0);
        float tiempo = PlayerPrefs.GetFloat("TIEMPO_FINAL", 0f);
        
        // Mostrar puntos
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos;
        
        // Mostrar coins
        if (textoCoins != null)
            textoCoins.text = "Coins: " + coins;
        
        // Mostrar tiempo formateado
        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempo / 60f);
            int segundos = Mathf.FloorToInt(tiempo % 60f);
            textoTiempo.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);
        }
    }
    
    void ReiniciarJuego()
    {
        // Cargar la escena del juego
        // Cambia "Juego" por el nombre de tu escena principal
        SceneManager.LoadScene("Juego");
    }
    
    void IrAlMenu()
    {
        // Cargar menú principal (índice 0 normalmente)
        SceneManager.LoadScene(0);
    }
}