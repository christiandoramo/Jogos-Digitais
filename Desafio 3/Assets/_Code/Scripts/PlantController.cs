using System.Collections;
using UnityEngine;
using static CollectableManager;


public class PlantController : MonoBehaviour
{
    public CultivableType cultivableType;
    public CollectableManager collectableManager;

    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public GameObject powerUpPrefab;


    // Use this for initialization
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.dayTimer == 0)
        {
            collectableManager.BornPlantPowerUp(transform.position, powerUpPrefab ,Quaternion.identity);
            Destroy(gameObject, 1f);
        }
    }
}

