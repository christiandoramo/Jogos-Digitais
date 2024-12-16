using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] float floatSpeed = 1f; // Velocidade da flutuação
    [SerializeField] float floatAmplitude = 0.5f; // Amplitude da flutuação


    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Calcula a nova posição usando uma função senoidal para flutuação
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
