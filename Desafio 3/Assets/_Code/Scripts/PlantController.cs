using System.Collections;
using UnityEngine;
using static CollectableManager;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class PlantController : MonoBehaviour
{
    public CultivableType cultivableType;
    public CollectableManager collectableManager;

    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public GameObject powerUpPrefab;
    [SerializeField] float counter = 15f;


    // Use this for initialization
    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (animator == null) animator = gameObject.GetComponent<Animator>();
        collectableManager = GameManager.instance.collectableManager;
    }

    // Update is called once per frame
    void Update()
    {
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            collectableManager.BornPlantPowerUp(cultivableType, transform.position + (Vector3.up * 1f), powerUpPrefab, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

