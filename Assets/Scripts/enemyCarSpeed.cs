using UnityEngine;

public class enemyCarSpeed : MonoBehaviour
{
    [SerializeField] private float minSpeed = 150f;
    [SerializeField] private float maxSpeed = 200f;
    private float speed = 0f;

    void Start()
    {
        // Asignar velocidad aleatoria al iniciar
        speed = Random.Range(minSpeed, maxSpeed);
        Debug.Log($"Velocidad del enemigo: {speed}");
    }

    void Update()
    {
        // Mover en dirección Z positiva (forward/adelante)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        // Otra forma equivalente:
        // transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}
