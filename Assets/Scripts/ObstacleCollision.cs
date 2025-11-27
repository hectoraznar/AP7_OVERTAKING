using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Configuración")]
    public string playerTag = "player";
    
    private void Start()
    {
        Debug.Log($"ObstacleCollision listo en {gameObject.name}");
        
        // Verificar que tenemos collider de trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null && collider.isTrigger)
        {
            Debug.Log("✓ Configurado como TRIGGER correctamente");
        }
        else
        {
            Debug.LogError("❌ ERROR: El collider NO está configurado como trigger");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔹 Trigger activado con: {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (other.CompareTag(playerTag))
        {
            Debug.Log("🎯 ¡JUGADOR CHOCÓ! Cambiando a escena de derrota...");
            SceneManager.LoadScene("Derrota");
        }
    }

    // Para debug visual
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}