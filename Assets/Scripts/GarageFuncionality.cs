using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography.X509Certificates;

public class GarageFuncionality : MonoBehaviour
{
    [Header("Referencias")]
    public Renderer cuboRenderer; // Arrastra el cubo aquí desde el Inspector

    
    [Header("Array de Colores (Orden: 0-Red, 1-Blue, 2-Green, 3-Yellow, 4-Pink, 5-Grey, 6-Black)")]
    public Color[] colores = new Color[7];
    
    [Header("Botones (Opcional - Asignar en orden)")]
    public Button[] botones = new Button[7];
    public Button botonReset;

    [Header("Material del Cubo")]
    public Material materialOriginalCubo;
    private Color colorOriginal;
    public MeshRenderer myMeshRender;
    void Start()
    {
        // Inicializar array de colores con valores por defecto si está vacío
        InicializarColores();
       
        
        // Configurar material del cubo
        if (cuboRenderer != null && materialOriginalCubo != null)
        {
            cuboRenderer.material = new Material(materialOriginalCubo);
            colorOriginal = materialOriginalCubo.color;
        }
        
        // Configurar botones automáticamente si están asignados
        ConfigurarBotonesDesdeArray();
    }

    void InicializarColores()
    {
        if (colores.Length < 7)
        {
            Array.Resize(ref colores, 7);
        }

        // Asignar colores por defecto si no están configurados
        if (colores[0] == default(Color)) colores[0] = Color.red;
        // Red
        if (colores[1] == default(Color)) colores[1] = Color.blue;     // Blue
        if (colores[2] == default(Color)) colores[2] = Color.green;    // Green
        if (colores[3] == default(Color)) colores[3] = Color.yellow;   // Yellow
        if (colores[4] == default(Color)) colores[4] = MagentaColor(); // Pink
        if (colores[5] == default(Color)) colores[5] = Color.gray;     // Grey
        if (colores[6] == default(Color)) colores[6] = Color.black;    // Black
    }

    Color MagentaColor()
    {
        return new Color(1f, 0f, 1f); // Color magenta para Pink
    }

    void ConfigurarBotonesDesdeArray()
    {
        // Configurar botones de colores (0-6)
        for (int i = 0; i < Mathf.Min(botones.Length, colores.Length); i++)
        {
            int index = i; // Importante: capturar el índice actual
            if (botones[i] != null)
            {
                botones[i].onClick.AddListener(() => CambiarColorPorIndice(index));
            }
        }

        // Configurar botón reset
        if (botonReset != null)
        {
            botonReset.onClick.AddListener(ResetearColor);
        }
    }

    // Función principal para cambiar color por índice
    public void CambiarColorPorIndice(int indiceColor)
    {
        if (indiceColor >= 0 && indiceColor < colores.Length)
        {
            CambiarColor(colores[indiceColor]);
            Debug.Log($"Color cambiado a: {ObtenerNombreColor(indiceColor)} (Índice: {indiceColor})");
        }
        else
        {
            Debug.LogError($"Índice de color inválido: {indiceColor}");
        }
    }

    // Función genérica para cambiar color
    public void CambiarColor(Color nuevoColor)
    {
        if (cuboRenderer != null)
        {
            cuboRenderer.material.color = nuevoColor;
        }
        else
        {
            Debug.LogError("No se ha asignado el cuboRenderer en el Inspector");
        }
    }

    public void ResetearColor()
    {
        if (cuboRenderer != null)
        {
            cuboRenderer.material.color = colorOriginal;
            Debug.Log("Color reseteado al original");
        }
    }

    string ObtenerNombreColor(int indice)
    {
        string[] nombres = { "Red", "Blue", "Green", "Yellow", "Pink", "Grey", "Black" };
        return indice >= 0 && indice < nombres.Length ? nombres[indice] : "Desconocido";
    }

    // Funciones específicas para asignar manualmente en el Inspector si se prefiere
    public void CambiarColorRed() => CambiarColorPorIndice(0);
    public void CambiarColorBlue() => CambiarColorPorIndice(1);
    public void CambiarColorGreen() => CambiarColorPorIndice(2);
    public void CambiarColorYellow() => CambiarColorPorIndice(3);
    public void CambiarColorPink() => CambiarColorPorIndice(4);
    public void CambiarColorGrey() => CambiarColorPorIndice(5);
    public void CambiarColorBlack() => CambiarColorPorIndice(6);
}