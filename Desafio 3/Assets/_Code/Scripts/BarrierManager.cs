using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    [SerializeField] float distanceToRunByWave = 20f;
    public bool waveHasChanged;
    float i = 0;

    // Update is called once per frame
    void Update()
    {
        if (waveHasChanged) UpBarrier();

    }
    void UpBarrier()
    {
        if (waveHasChanged)
        {
            i += Time.deltaTime;
            transform.position += (Vector3.up * i);

            if (i >= distanceToRunByWave)
            {
                i = 0;
                waveHasChanged = false;
            }

        }
    }
}
