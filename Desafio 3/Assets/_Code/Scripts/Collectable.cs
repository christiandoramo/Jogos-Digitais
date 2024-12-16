using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] float floatSpeed = 1f; // Velocidade da flutua��o
    [SerializeField] float floatAmplitude = 0.5f; // Amplitude da flutua��o


    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Calcula a nova posi��o usando uma fun��o senoidal para flutua��o
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
