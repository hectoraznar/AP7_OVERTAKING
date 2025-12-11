using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    
    private const string COINS_TOTAL_KEY = "COINS_TOTALES";
    private const string COINS_PARTIDA_KEY = "COINS_FINALES";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Inicializar si no existe
        if (!PlayerPrefs.HasKey(COINS_TOTAL_KEY))
        {
            PlayerPrefs.SetInt(COINS_TOTAL_KEY, 0);
        }
    }
    
    // Obtener coins totales
    public int GetCoinsTotales()
    {
        return PlayerPrefs.GetInt(COINS_TOTAL_KEY, 0);
    }
    
    // Agregar coins de una partida
    public void AgregarCoinsPartida()
    {
        int coinsPartida = PlayerPrefs.GetInt(COINS_PARTIDA_KEY, 0);
        int coinsTotales = GetCoinsTotales() + coinsPartida;
        
        PlayerPrefs.SetInt(COINS_TOTAL_KEY, coinsTotales);
        PlayerPrefs.Save();
        
        Debug.Log($"Coins agregados: {coinsPartida}. Total: {coinsTotales}");
    }
    
    // Gastar coins (para compras)
    public bool GastarCoins(int cantidad)
    {
        int coinsTotales = GetCoinsTotales();
        
        if (coinsTotales >= cantidad)
        {
            coinsTotales -= cantidad;
            PlayerPrefs.SetInt(COINS_TOTAL_KEY, coinsTotales);
            PlayerPrefs.Save();
            
            Debug.Log($"Coins gastados: {cantidad}. Restantes: {coinsTotales}");
            return true;
        }
        
        Debug.Log($"No hay suficientes coins. Necesitas: {cantidad}, Tienes: {coinsTotales}");
        return false;
    }
    
    // Resetear coins (para debug)
    public void ResetearCoins()
    {
        PlayerPrefs.SetInt(COINS_TOTAL_KEY, 0);
        PlayerPrefs.Save();
    }
}