using Unity.VisualScripting;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    [SerializeField] float distanceToRunByWave = 20f;
    public bool waveHasChanged;
    int i = 0;

    // Update is called once per frame
    void Update()
    {
        if (waveHasChanged) UpBarrier();

    }
    void UpBarrier()
    {
        if (waveHasChanged)
        {
            transform.position += (Vector3.up);
            i++;
            if (i == distanceToRunByWave)
            {
                i = 0;
                waveHasChanged = false;
            }

        }
    }
}
