using Unity.PlasticSCM.Editor.WebApi;
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
    private Mesh currentCarModel; 
    public SaveChangeCarSkin mSCS;
    void Start()
    {
        carObject = GameObject.Find("Car");
        btnCar1 = GameObject.Find("btnCar1").GetComponent<Button>();
        btnCar2 = GameObject.Find("btnCar2").GetComponent<Button>();

        // Guardar el mesh original al inicio
        if (carObject != null)
        {
            MeshFilter meshFilter = carObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.mesh != null)
            {
                originalMesh = meshFilter.mesh;
                hasOriginalMesh = true;
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
        if (carObject != null)
        {
            // Método 1: Cambiar el MeshFilter a una esfera
            ChangeMeshToSphere();
            
            // Método 2: O reemplazar por un prefab (descomenta la línea de abajo)
            // ReplaceWithSpherePrefab();
        }
        else
        {
            Debug.LogWarning("No hay objeto car asignado");
        }
    }
    
    void RestoreOriginalShape()
    {
        if (carObject != null && hasOriginalMesh)
        {
            MeshFilter meshFilter = carObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = originalMesh;
                Debug.Log("Forma original restaurada");
            }
        }
        else if (!hasOriginalMesh && carObject != null)
        {
            // Si no se guardó el mesh original, restaurar a cubo
            RestoreToCube();
        }
        else
        {
            Debug.LogWarning("No hay objeto car asignado o no se guardó la forma original");
        }
    }
    
    public void ChangeMeshToSphere()
    {
        // Obtener o agregar el componente MeshFilter
        MeshFilter meshFilter = carObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = carObject.AddComponent<MeshFilter>();
        }
        
        // Cambiar el mesh a una esfera
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sphereMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
        currentCarModel = sphereMesh;

        meshFilter.mesh = sphereMesh;
        // PASAR LA ESFERA 


        
        // Destruir la esfera temporal
        Destroy(sphere);
        
        Debug.Log("Cubo convertido a esfera");
    }
    
    public void RestoreToCube()
    {
        MeshFilter meshFilter = carObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            meshFilter.mesh = cube.GetComponent<MeshFilter>().sharedMesh;
            currentCarModel = cube.GetComponent<MeshFilter>().sharedMesh;
           
            Destroy(cube);
            Debug.Log("Restaurado a cubo");
        }
    }
    
    void ReplaceWithSpherePrefab()
    {
        // Método alternativo usando prefab
        if (spherePrefab != null)
        {
            Vector3 position = carObject.transform.position;
            Quaternion rotation = carObject.transform.rotation;
            Transform parent = carObject.transform.parent;
            
            // Destruir el cubo actual
            Destroy(carObject);
            
            // Instanciar la esfera
            carObject = Instantiate(spherePrefab, position, rotation, parent);
            carObject.name = "Car";
        }
        else
        {
            Debug.LogWarning("No hay spherePrefab asignado");
        }
    }

    public Mesh getCarMesh()
    {
        return currentCarModel;
    }
}