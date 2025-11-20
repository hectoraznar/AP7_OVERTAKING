using UnityEngine;

public class carSpawner : MonoBehaviour
{
    public Transform [] spawnPoints;
    public GameObject carPrefab;
    float spawnInterval = 3f;
    private float currentSpawnTime;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpawnTime = spawnInterval;        
    }

    // Update is called once per frame
    void Update()
    {
        spawnInterval-=Time.deltaTime;
        if(spawnInterval <= 0f)
        {
            invokeCar();
            spawnInterval = 3f;
        }
    }
    private void invokeCar()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(carPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }

    public void decreaseTimeByPlayerScore(float amount)
    {
        currentSpawnTime -= amount;
        spawnInterval = currentSpawnTime;
        if(spawnInterval < 1f)
        {
            spawnInterval = 1f;
        }
    }
}
