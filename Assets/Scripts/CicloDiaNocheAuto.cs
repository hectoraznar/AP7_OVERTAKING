using UnityEngine;

public class CicloDiaNocheAuto : MonoBehaviour
{
    public Light luzSol;
    private float tiempoCiclo = 0.8f;
    private const float DURACION_CICLO = 80f; // 30 segundos para día + noche
    
    void Start()
    {
        // Buscar automáticamente la luz direccional
        BuscarLuzSolar();
        
        // Configurar automáticamente el ambiente
       // ConfigurarAmbienteInicial();
    }
    
    void BuscarLuzSolar()
    {
        // Intentar encontrar la luz direccional principal
        //luzSol = GameObject.FindObjectOfType<Light>();
        
        // Si no hay ninguna luz, crear una
       /* if (luzSol == null)
        {
            GameObject nuevaLuz = new GameObject("Sol_Luna");
            luzSol = nuevaLuz.AddComponent<Light>();
            luzSol.type = LightType.Directional;
            nuevaLuz.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }*/
        
        // Asegurar que es una luz direccional
      //  luzSol.type = LightType.Directional;
    }
    
    void ConfigurarAmbienteInicial()
    {
        // Configurar niebla para mejor efecto
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.01f;
    }
    
    void Update()
    {
        // Actualizar tiempo del ciclo
        tiempoCiclo += Time.deltaTime;
        
        // Reiniciar ciclo cada 30 segundos
        if (tiempoCiclo >= DURACION_CICLO)
        {
            tiempoCiclo = 0f;
        }
        
        // Calcular progreso del ciclo (0 a 1)
        float progreso = tiempoCiclo / DURACION_CICLO;
        
        // Actualizar rotación del sol/luna (simula movimiento circular)
        ActualizarRotacionLuz(progreso);
        
        // Actualizar intensidad y color de la luz
        ActualizarPropiedadesLuz(progreso);
        
        // Actualizar ambiente y cielo
        ActualizarAmbiente(progreso);
    }
    
    void ActualizarRotacionLuz(float progreso)
    {
        // Calcular ángulo (0° = noche, 90° = amanecer, 180° = día, 270° = atardecer)
        float angulo = progreso * 360f;
        
        // Convertir a rotación 3D
        float altura = Mathf.Sin(angulo * Mathf.Deg2Rad) * 180f;
        luzSol.transform.rotation = Quaternion.Euler(altura, -30f, 0f);
    }
    
    void ActualizarPropiedadesLuz(float progreso)
    {
        // Calcular intensidad basada en la altura del sol
        float alturaNormalizada = Mathf.Sin(progreso * Mathf.PI);
        float intensidad = Mathf.Clamp(alturaNormalizada, 0.1f, 1f);
        luzSol.intensity = intensidad;
        
        // Cambiar color gradualmente
        // Día: amarillo cálido → Atardecer: naranja → Noche: azul oscuro
        Color colorLuz;
        if (progreso < 0.25f) // Noche a amanecer
        {
            colorLuz = Color.Lerp(
                new Color(0.1f, 0.1f, 0.3f), // Azul noche
                new Color(1f, 0.6f, 0.4f),   // Naranja amanecer
                progreso * 4f
            );
        }
        else if (progreso < 0.5f) // Amanecer a día
        {
            colorLuz = Color.Lerp(
                new Color(1f, 0.6f, 0.4f),   // Naranja amanecer
                new Color(1f, 0.95f, 0.9f),  // Amarillo día
                (progreso - 0.25f) * 4f
            );
        }
        else if (progreso < 0.75f) // Día a atardecer
        {
            colorLuz = Color.Lerp(
                new Color(1f, 0.95f, 0.9f),  // Amarillo día
                new Color(1f, 0.4f, 0.2f),   // Rojo atardecer
                (progreso - 0.5f) * 4f
            );
        }
        else // Atardecer a noche
        {
            colorLuz = Color.Lerp(
                new Color(1f, 0.4f, 0.2f),   // Rojo atardecer
                new Color(0.1f, 0.1f, 0.3f), // Azul noche
                (progreso - 0.75f) * 4f
            );
        }
        
        luzSol.color = colorLuz;
    }
    
    void ActualizarAmbiente(float progreso)
    {
        // Calcular brillo del cielo basado en la posición del sol
        float brilloCielo = Mathf.Clamp(Mathf.Sin(progreso * Mathf.PI), 0.05f, 0.8f);
        
        // Actualizar color del ambiente
        Color colorCielo;
        if (progreso < 0.5f) // Noche a día
        {
            colorCielo = Color.Lerp(
                new Color(0.05f, 0.05f, 0.1f), // Noche oscura
                new Color(0.5f, 0.7f, 1f),     // Día azul
                progreso * 2f
            );
        }
        else // Día a noche
        {
            colorCielo = Color.Lerp(
                new Color(0.5f, 0.7f, 1f),     // Día azul
                new Color(0.05f, 0.05f, 0.1f), // Noche oscura
                (progreso - 0.5f) * 2f
            );
        }
        
        // Aplicar configuraciones
        RenderSettings.ambientSkyColor = colorCielo;
        RenderSettings.ambientIntensity = brilloCielo;
        RenderSettings.fogColor = colorCielo;
        
        // Ajustar niebla para más realismo
        RenderSettings.fogDensity = Mathf.Lerp(0.015f, 0.005f, brilloCielo);
    }
    
    // Métodos públicos útiles (opcionales)
    public float ObtenerHoraNormalizada()
    {
        return tiempoCiclo / DURACION_CICLO;
    }
    
    public bool EsDeDia()
    {
        float progreso = tiempoCiclo / DURACION_CICLO;
        return progreso > 0.25f && progreso < 0.75f;
    }
    
    public void ReiniciarCiclo()
    {
        tiempoCiclo = 0f;
    }
}