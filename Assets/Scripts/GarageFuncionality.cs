using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography.X509Certificates;

public class GarageFuncionality : MonoBehaviour
{
    [Header("Referencias")]
    public Renderer cuboRenderer; // Arrastra el cubo aquí desde el Inspector

       [Header("Referencias de Modelos de Coche")]
    public GameObject[] carModels; // Array de modelos de coche disponibles
    
    [Header("Referencias de Renderizado")]
    public Renderer[] carRenderers; // Arrastra los renderers de los coches aquí desde el Inspector
    public Material originalMaterial; // Material original de los coches
    
    [Header("Array de Colores (Orden: 0-Red, 1-Blue, 2-Green, 3-Yellow, 4-Pink, 5-Grey, 6-Black)")]
    public Color[] colors = new Color[7];
    
    [Header("Botones de UI (Opcional - Asignar en orden)")]
    public Button[] colorButtons = new Button[7];
    public Button resetColorButton;
    public Button[] carModelButtons; // Botones para cambiar de modelo (ej: 0, 1, 2, etc.)
    
    private Color originalColor;
    private Color currentColor;
    private int currentCarIndex = 0;
    private Material[] carMaterials; // Materiales instanciados para cada coche

    GameObject currentCarModel;
    void Start()
    {
        InitializeSystem();
    }

    void InitializeSystem()
    {
        // Inicializar colores por defecto
        InitializeColors();
        
        // Inicializar materiales de los coches
        InitializeCarMaterials();
        
        // Activar el primer coche y desactivar los demás
        SetActiveCar(0);
        
        // Configurar botones de UI
        ConfigureColorButtons();
        ConfigureCarModelButtons();
    }

    void InitializeColors()
    {
        if (colors.Length < 7)
        {
            Array.Resize(ref colors, 7);
        }

        // Asignar colores por defecto si no están configurados
        if (colors[0] == default(Color)) colors[0] = Color.red;
        if (colors[1] == default(Color)) colors[1] = Color.blue;
        if (colors[2] == default(Color)) colors[2] = Color.green;
        if (colors[3] == default(Color)) colors[3] = Color.yellow;
        if (colors[4] == default(Color)) colors[4] = MagentaColor(); // Pink
        if (colors[5] == default(Color)) colors[5] = Color.gray;
        if (colors[6] == default(Color)) colors[6] = Color.black;
    }

    Color MagentaColor()
    {
        return new Color(1f, 0f, 1f); // Color magenta para Pink
    }

    void InitializeCarMaterials()
    {
        if (originalMaterial != null && carRenderers != null && carRenderers.Length > 0)
        {
            carMaterials = new Material[carRenderers.Length];
            originalColor = originalMaterial.color;
            currentColor = originalColor;
            
            for (int i = 0; i < carRenderers.Length; i++)
            {
                if (carRenderers[i] != null)
                {
                    // Crear una nueva instancia del material para cada coche
                    carMaterials[i] = new Material(originalMaterial);
                    carRenderers[i].material = carMaterials[i];
                }
            }
        }
        else
        {
            Debug.LogWarning("Faltan referencias de materiales o renderers en el Inspector");
        }
    }

    void ConfigureColorButtons()
    {
        // Configurar botones de colores (0-6)
        for (int i = 0; i < Mathf.Min(colorButtons.Length, colors.Length); i++)
        {
            int index = i; // Importante: capturar el índice actual
            if (colorButtons[i] != null)
            {
                colorButtons[i].onClick.RemoveAllListeners();
                colorButtons[i].onClick.AddListener(() => ChangeColorByIndex(index));
            }
        }

        // Configurar botón reset
        if (resetColorButton != null)
        {
            resetColorButton.onClick.RemoveAllListeners();
            resetColorButton.onClick.AddListener(ResetColor);
        }
    }

    void ConfigureCarModelButtons()
    {
        if (carModelButtons != null)
        {
            for (int i = 0; i < carModelButtons.Length; i++)
            {
                int carIndex = i;
                if (carModelButtons[i] != null)
                {
                    carModelButtons[i].onClick.RemoveAllListeners();
                    carModelButtons[i].onClick.AddListener(() => ChangeCarModel(carIndex));
                }
            }
        }
    }

    // ===== FUNCIONALIDAD DE CAMBIO DE MODELO =====
    public void ChangeCarModel(int carIndex)
    {    currentCarModel = carModels[carIndex];
        if (carModels == null || carIndex < 0 || carIndex >= carModels.Length)
        {
            Debug.LogError($"Índice de coche inválido: {carIndex}");
            return;
        }

        // Desactivar todos los coches
        for (int i = 0; i < carModels.Length; i++)
        {
            if (carModels[i] != null)
            {
                carModels[i].SetActive(false);
            }
        }

        // Activar el coche seleccionado
        carModels[carIndex].SetActive(true);
        currentCarIndex = carIndex;
        
        // Aplicar el color actual al nuevo coche activo
        if (carRenderers != null && carIndex < carRenderers.Length && carRenderers[carIndex] != null)
        {
            carRenderers[carIndex].material.color = currentColor;
        }
        
        Debug.Log($"Modelo cambiado a: Coches[{carIndex}]");
    }

    // ===== FUNCIONALIDAD DE CAMBIO DE COLOR =====
    public void ChangeColorByIndex(int colorIndex)
    {
        if (colorIndex >= 0 && colorIndex < colors.Length)
        {
            currentColor = colors[colorIndex];
            ApplyColorToCurrentCar(currentColor);
            Debug.Log($"Color cambiado a: {GetColorName(colorIndex)} (Índice: {colorIndex})");
        }
        else
        {
            Debug.LogError($"Índice de color inválido: {colorIndex}");
        }
    }

    void ApplyColorToCurrentCar(Color newColor)
    {
        if (carRenderers != null && currentCarIndex < carRenderers.Length && carRenderers[currentCarIndex] != null)
        {
            carRenderers[currentCarIndex].material.color = newColor;
        }
        else
        {
            Debug.LogWarning("No se pudo aplicar el color al coche actual. Verifica las referencias.");
        }
    }

    public void ResetColor()
    {
        currentColor = originalColor;
        ApplyColorToCurrentCar(originalColor);
        Debug.Log("Color reseteado al original");
    }

    // ===== FUNCIONES AUXILIARES =====
    string GetColorName(int index)
    {
        string[] names = { "Red", "Blue", "Green", "Yellow", "Pink", "Grey", "Black" };
        return index >= 0 && index < names.Length ? names[index] : "Unknown";
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }

    public int GetCurrentCarIndex()
    {
        return currentCarIndex;
    }

    public GameObject GetCar()
    {
        return currentCarModel;
    }

    // ===== FUNCIONES ESPECÍFICAS PARA ASIGNAR MANUALMENTE EN EL INSPECTOR =====
    public void SetActiveCar(int index)
    {
        
        ChangeCarModel(index);
    }
    
    public void ChangeColorRed() => ChangeColorByIndex(0);
    public void ChangeColorBlue() => ChangeColorByIndex(1);
    public void ChangeColorGreen() => ChangeColorByIndex(2);
    public void ChangeColorYellow() => ChangeColorByIndex(3);
    public void ChangeColorPink() => ChangeColorByIndex(4);
    public void ChangeColorGrey() => ChangeColorByIndex(5);
    public void ChangeColorBlack() => ChangeColorByIndex(6);

}