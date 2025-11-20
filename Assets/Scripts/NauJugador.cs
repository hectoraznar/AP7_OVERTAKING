using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 200f; // Velocidad de movimiento lateral base
    public float forwardSpeed = 20f; // Velocidad base hacia adelante
    public float maxForwardSpeed = 80f; // Velocidad máxima hacia adelante
    public float acceleration = 10f; // Qué tan rápido acelera
    public float normalDeceleration = 4f; // Deceleración normal cuando sueltas W
    public float brakeDeceleration = 2000000f; // Deceleración cuando presionas S
    public float minLateralMultiplier = 0.3f; // Mínimo movimiento lateral (a velocidad 0)
    public float maxLateralSpeed = 20f; // Velocidad a la que se alcanza el giro completo
    
    private float currentForwardSpeed = 0f; // Velocidad actual hacia adelante
    
    void Update()
    {
        // Movimiento lateral (A y D) - SOLO si el coche tiene velocidad
        if (currentForwardSpeed > 0f)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            
            // Calcular multiplicador de movimiento lateral basado en la velocidad
            float speedRatio = Mathf.Clamp01(currentForwardSpeed / maxLateralSpeed);
            float lateralMultiplier = Mathf.Lerp(minLateralMultiplier, 1.0f, speedRatio);
            
            Vector3 lateralMovement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * lateralMultiplier * Time.deltaTime;
            transform.Translate(lateralMovement);
        }
        
        // Control de aceleración con tecla W
        if (Input.GetKey(KeyCode.W))
        {
            // Aumentar velocidad progresivamente
            currentForwardSpeed += acceleration * Time.deltaTime;
            currentForwardSpeed = Mathf.Min(currentForwardSpeed, maxForwardSpeed);
        }
        
        // Aplicar deceleración (normal o de frenado)
        float currentDeceleration = normalDeceleration;
        
        if (Input.GetKey(KeyCode.S))
        {
            // Usar deceleración de frenado cuando se presiona S
            currentDeceleration = brakeDeceleration;
        }
        
        // Solo aplicar deceleración si no estamos acelerando con W
        if (!Input.GetKey(KeyCode.W))
        {
            currentForwardSpeed -= currentDeceleration * Time.deltaTime;
            currentForwardSpeed = Mathf.Max(currentForwardSpeed, 0f);
        }
        
        // Aplicar movimiento hacia adelante
        Vector3 forwardMovement = new Vector3(0f, 0f, currentForwardSpeed) * Time.deltaTime;
        transform.Translate(forwardMovement);
    }
}