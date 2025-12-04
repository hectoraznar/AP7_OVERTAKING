using UnityEngine;

public class CarManager : MonoBehaviour
{
    public static CarManager Instance;
    // Variables privadas
    public Color _colorCoche = Color.red;
    private int _indiceModelo = 0;
    public Mesh carMesh;
    
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
    }
    
    // ===== SETTERS =====
    
    public void SetColor(Color nuevoColor)
    {
        _colorCoche = nuevoColor;
        Debug.Log($"Color guardado: {nuevoColor}");
    }
    
    public void SetModelo(Mesh indiceModelo)
    {
        /*if (indiceModelo >= 0 && indiceModelo <= 1)
        {
            _indiceModelo = indiceModelo;
            Debug.Log($"Modelo guardado: {indiceModelo}");
        }
        else
        {
            Debug.LogError("Índice debe ser 0 o 1");
        }*/
        carMesh = indiceModelo;

    }
    
    // ===== GETTERS =====
    
    public Color GetColor()
    {
        return _colorCoche;
    }
    
    public Mesh GetModelo()
    {
        //return _indiceModelo;
        return carMesh;
    }
}