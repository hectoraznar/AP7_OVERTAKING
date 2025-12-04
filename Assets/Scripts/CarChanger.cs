using UnityEngine;
using UnityEngine.UI;

public class CarChanger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject carObject; // Arrastra tu cubo aquí desde el Inspector
    public Button btnCar1; // Arrastra tu btnCar1 aquí desde el Inspector
    public Button btnCar2; // Arrastra tu btnCar2 aquí desde el Inspector
    
    [Header("Prefabs (Opcional)")]
    public GameObject spherePrefab; // Si quieres usar un prefab de esfera
    
    // Variable para guardar el mesh original
    private Mesh originalMesh;
    private bool hasOriginalMesh = false;
    private Mesh currentMesh; // Cambiado a Mesh en lugar de MeshFilter
    
    // Cambiado a array de Mesh en lugar de MeshFilter
    public Mesh[] carMeshes; 
    

    void Start()
    {
        
        // Buscar objetos si no están asignados
        if (carObject == null)
            carObject = GameObject.Find("Car");
        
        if (btnCar1 == null)
            btnCar1 = GameObject.Find("btnCar1")?.GetComponent<Button>();
            
        if (btnCar2 == null)
            btnCar2 = GameObject.Find("btnCar2")?.GetComponent<Button>();

        // Guardar el mesh original al inicio
        if (carObject != null)
        {
            MeshFilter meshFilter = carObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                originalMesh = meshFilter.sharedMesh;
                hasOriginalMesh = true;
                
                // Inicializar con el primer mesh del array
                if (carMeshes != null && carMeshes.Length > 0)
                {
                    currentMesh = originalMesh;
                }
            }
        }
        
        // Asignar los eventos de los botones
        if (btnCar1 != null)
        {
            btnCar1.onClick.AddListener(RestoreOriginalShape);
        }
        
        if (btnCar2 != null)
        {
            btnCar2.onClick.AddListener(ChangeToSphere);
        }
    }
    
    void ChangeToSphere()
    {
        if (carObject == null || carMeshes == null || carMeshes.Length < 2)
        {
            Debug.LogWarning("Objeto car no asignado o carMeshes no tiene suficientes elementos");
            return;
        }
        
        // Cambiar al segundo mesh (esfera)
        ApplyMesh(carMeshes[1]);
    }
    
    void RestoreOriginalShape()
    {
        if (carObject == null)
        {
            Debug.LogWarning("Objeto car no asignado");
            return;
        }
        
        if (hasOriginalMesh)
        {
            // Restaurar mesh original
            ApplyMesh(originalMesh);
        }
        else if (carMeshes != null && carMeshes.Length > 0)
        {
            // Usar el primer mesh del array
            ApplyMesh(carMeshes[0]);
            currentMesh = carMeshes[0];
        }
        else
        {
            Debug.LogWarning("No hay mesh original guardado ni carMeshes configurado");
        }
    }
    
    // Método para aplicar un mesh al objeto
    private void ApplyMesh(Mesh meshToApply)
    {
        if (carObject == null || meshToApply == null)
            return;
            
        MeshFilter meshFilter = carObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogWarning("El objeto no tiene MeshFilter");
            return;
        }
        
        // Aplicar el mesh
        meshFilter.sharedMesh = meshToApply;
        currentMesh = meshToApply;
        
        Debug.Log("Mesh cambiado a: " + meshToApply.name);
    }
    
    // Método para aplicar mesh por índice desde el array
    public void ApplyMeshByIndex(int index)
    {
        if (carMeshes == null || index < 0 || index >= carMeshes.Length)
        {
            Debug.LogWarning($"Índice {index} fuera de rango o carMeshes no configurado");
            return;
        }
        
        ApplyMesh(carMeshes[index]);
    }
    
    // Método para aplicar un mesh desde otro MeshFilter
    public void ApplyMeshFromMeshFilter(MeshFilter sourceMeshFilter)
    {
        if (sourceMeshFilter == null || sourceMeshFilter.sharedMesh == null)
        {
            Debug.LogWarning("MeshFilter de origen no válido");
            return;
        }
        
        ApplyMesh(sourceMeshFilter.sharedMesh);
    }
    
    // Getters y Setters
    public Mesh GetCurrentMesh()
    {
        return currentMesh;
    }
    
    public void SetCurrentMesh(Mesh newMesh)
    {
        currentMesh = newMesh;
        ApplyMesh(newMesh);
    }
    public Mesh getCarMesh()
    {
        Debug.Log(currentMesh.name.ToString());
        return currentMesh;
        
    }
}