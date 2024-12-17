using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    [SerializeField] float distanceToRunByWave = 10f;
    public bool waveHasChanged;

    // Update is called once per frame
    void Update()
    {
        if (waveHasChanged) UpBarrier();

    }
    void UpBarrier()
    {
        waveHasChanged = false;
        for (float i = 0; i < distanceToRunByWave; i += Time.deltaTime)
        {
            transform.position += (Vector3.up * Time.deltaTime);
        }
    }
}
