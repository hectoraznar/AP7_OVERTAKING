using UnityEngine;
using System.Collections;

public class CarSpawnerWithStages : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public Transform[] spawnPoints;
    public GameObject carPrefab;
    
    [Header("Etapas de Dificultad")]
    public DifficultyStage[] difficultyStages = new DifficultyStage[]
    {
        new DifficultyStage(0f, 3f),    // 0-30s: cada 3s
        new DifficultyStage(30f, 2f),   // 30-60s: cada 2s  
        new DifficultyStage(60f, 1.2f), // 60-90s: cada 1.2s
        new DifficultyStage(90f, 0.6f), // 90-120s: cada 0.6s
        new DifficultyStage(120f, 0.4f) // 120s+: cada 0.4s
    };
    
    private float currentSpawnTime;
    private float currentSpawnInterval;
    private Transform playerCar;
    private float gameTime;

    [System.Serializable]
    public class DifficultyStage
    {
        public float startTime;
        public float spawnInterval;
        
        public DifficultyStage(float time, float interval)
        {
            startTime = time;
            spawnInterval = interval;
        }
    }

    void Start()
    {
        currentSpawnInterval = difficultyStages[0].spawnInterval;
        currentSpawnTime = currentSpawnInterval;
        FindPlayerCar();
        gameTime = 0f;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        UpdateDifficultyStage();
        
        currentSpawnTime -= Time.deltaTime;
        
        if(currentSpawnTime <= 0f)
        {
            SpawnCar();
            currentSpawnTime = currentSpawnInterval;
        }
        
        FollowPlayer();
    }

    private void UpdateDifficultyStage()
    {
        for (int i = difficultyStages.Length - 1; i >= 0; i--)
        {
            if (gameTime >= difficultyStages[i].startTime)
            {
                if (currentSpawnInterval != difficultyStages[i].spawnInterval)
                {
                    currentSpawnInterval = difficultyStages[i].spawnInterval;
                    Debug.Log($"Nueva etapa: {difficultyStages[i].spawnInterval}s - Tiempo: {gameTime:F0}s");
                }
                break;
            }
        }
    }

    private void FindPlayerCar()
    {
        playerCar = GameObject.FindGameObjectWithTag("player").transform;
    }

    private void FollowPlayer()
    {
        if (playerCar == null) return;
        transform.position = playerCar.position + playerCar.forward * 15f;
    }

    private void SpawnCar()
    {
        if (spawnPoints.Length == 0) return;
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(carPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }
}