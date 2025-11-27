using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public Transform[] spawnPoints;
    public GameObject carPrefab;
    
    [Header("Tiempos de Spawn")]
    public float initialSpawnInterval = 3f;
    public float minSpawnInterval = 1f;
    
    private float currentSpawnTime;
    private float currentSpawnInterval;
    private Transform playerCar;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentSpawnTime = currentSpawnInterval;
        FindPlayerCar();
    }

    void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        
        if(currentSpawnTime <= 0f)
        {
            SpawnCar();
            currentSpawnTime = currentSpawnInterval;
        }
        
        FollowPlayer();
    }

    private void FindPlayerCar()
    {
        playerCar = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FollowPlayer()
    {
        if (playerCar == null) return;
        
        // Mover TODOS los spawn points delante del jugador
        transform.position = playerCar.position + playerCar.forward * 15f;
    }

    private void SpawnCar()
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawnPoint = spawnPoints[randomIndex];
        
        Instantiate(carPrefab, selectedSpawnPoint.position, Quaternion.identity);
    }

    public void DecreaseSpawnTime(float amount)
    {
        currentSpawnInterval -= amount;
        if(currentSpawnInterval < minSpawnInterval)
            currentSpawnInterval = minSpawnInterval;
    }
}