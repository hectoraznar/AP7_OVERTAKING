using UnityEngine;

public class CarSpawnerWithStages : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public Transform[] spawnPoints;
    public GameObject [] carPrefabs;
    
    [Header("Etapas de Dificultad")]
    public DifficultyStage[] difficultyStages = new DifficultyStage[]
    {
        new DifficultyStage(0f, 3f),
        new DifficultyStage(30f, 2f),
        new DifficultyStage(60f, 1.2f),
        new DifficultyStage(90f, 0.6f),
        new DifficultyStage(120f, 0.4f)
    };
    
    [Header("Control por Velocidad")]
    public float velocidadMinimaParaSpawn = 5f;
    public bool debugVelocidad = false;
    
    private float currentSpawnTime;
    private float currentSpawnInterval;
    private Transform playerCar;
    private float gameTime;
    private float velocidadActual;
    private Vector3 posicionAnterior;
    private float tiempoUltimaMedicion;

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
        BuscarJugador();
        gameTime = 0f;
        tiempoUltimaMedicion = Time.time;
    }

    void Update()
    {
        // 1. Actualizar velocidad del jugador
        CalcularVelocidadJugador();
        
        // 2. Solo sumar tiempo al juego si nos movemos
        if (velocidadActual > velocidadMinimaParaSpawn)
        {
            gameTime += Time.deltaTime;
            UpdateDifficultyStage();
            
            // 3. Solo spawnear si nos movemos
            currentSpawnTime -= Time.deltaTime;
            
            if (currentSpawnTime <= 0f)
            {
                SpawnCar();
                currentSpawnTime = currentSpawnInterval;
            }
        }
        else
        {
            // Si estamos parados, resetear el timer de spawn
            // pero NO resetear el tiempo de juego
            currentSpawnTime = currentSpawnInterval;
        }
        
        // 4. Seguir al jugador siempre
        FollowPlayer();
        
        // 5. Debug opcional
        if (debugVelocidad && Time.frameCount % 60 == 0)
        {
            Debug.Log($"Velocidad: {velocidadActual:F1} | Spawn: {(velocidadActual > velocidadMinimaParaSpawn ? "ACTIVO" : "PAUSADO")}");
        }
    }

    void CalcularVelocidadJugador()
    {
        if (playerCar == null)
        {
            BuscarJugador();
            return;
        }
        
        // Calcular velocidad basada en cambio de posición
        float tiempoActual = Time.time;
        float deltaTiempo = tiempoActual - tiempoUltimaMedicion;
        
        if (deltaTiempo > 0.1f) // Medir cada 0.1 segundos
        {
            float distancia = Vector3.Distance(posicionAnterior, playerCar.position);
            velocidadActual = distancia / deltaTiempo * 3.6f; // Convertir a km/h
            
            posicionAnterior = playerCar.position;
            tiempoUltimaMedicion = tiempoActual;
        }
    }

    void UpdateDifficultyStage()
    {
        for (int i = difficultyStages.Length - 1; i >= 0; i--)
        {
            if (gameTime >= difficultyStages[i].startTime)
            {
                if (currentSpawnInterval != difficultyStages[i].spawnInterval)
                {
                    currentSpawnInterval = difficultyStages[i].spawnInterval;
                }
                break;
            }
        }
    }

    void BuscarJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("player");
        if (jugador != null)
        {
            playerCar = jugador.transform;
            posicionAnterior = playerCar.position;
        }
    }

    void FollowPlayer()
    {
        if (playerCar == null) return;
        transform.position = playerCar.position + playerCar.forward * 15f;
    }

    void SpawnCar()
    {
        if (spawnPoints.Length == 0) return;
        
        // Verificar una última vez que estamos moviéndonos
        if (velocidadActual > velocidadMinimaParaSpawn)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            
           
             int randomCarIndex = Random.Range(0, carPrefabs.Length);
            
            // Instanciar el coche aleatorio
            Instantiate(carPrefabs[0], spawnPoints[randomSpawnIndex].position, Quaternion.identity);
            
            if (debugVelocidad)
            {
                Debug.Log($"Coche spawnedo a {velocidadActual:F1} km/h");
            }
        }
    }
    
    // Método para ver estado actual
    public string GetEstado()
    {
        string estado = $"Velocidad: {velocidadActual:F1} km/h\n";
        estado += $"Mínimo para spawn: {velocidadMinimaParaSpawn} km/h\n";
        estado += $"Spawn activo: {velocidadActual > velocidadMinimaParaSpawn}\n";
        estado += $"Tiempo juego: {gameTime:F1}s\n";
        estado += $"Siguiente spawn en: {currentSpawnTime:F1}s\n";
        estado += $"Intervalo: {currentSpawnInterval:F1}s";
        
        return estado;
    }
}