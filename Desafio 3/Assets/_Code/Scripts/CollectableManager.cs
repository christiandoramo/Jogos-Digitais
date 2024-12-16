using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableManager", menuName = "ScriptableObjects/CollectableManager", order = 2)]
public class CollectableManager : ScriptableObject
{

    [SerializeField] PlayerController pc;
    [SerializeField] GameObject pcObj;
    [SerializeField]
    RuntimeAnimatorController[] animatorControllers;
    [SerializeField] Sprite[] sprites;
    [SerializeField] int rollete;

    [SerializeField]
    [Tooltip("Aumento do pulo")]
    [Range(0.01f, 1f)]
    float jumpAugment = .1f;// inicia em 8
    [SerializeField]
    [Tooltip("Aumento do pulo")]
    [Range(0.01f, 1f)]
    float bulletDmgAugment = .1f; // inicia em 25
                                  // seed (vira qualquer coletável menos estrela) - 33% para hp,dmg ou jump
                                  // seeds e estrelas só são adquiridas com drops
                                  // hp pode ser adquirido com seed e drop
                                  // dmg, jump são adquiridos apenas com seed

    [SerializeField]
    [Tooltip("Aumento do pulo")]
    [Range(10, 500)]
    int hpRegenAugment = 10;

    [SerializeField] GameObject seedPrefab;
    [SerializeField] GameObject starPrefab;


    [SerializeField] GameObject plantPrefab;
    List<PlantInstance> plantInstances = new();

    [SerializeField]
    List<GameObject> powerUpPrefabs;
    public List<PowerUpInstance> powerUpInstances = new();

    public void Initialize()
    {
        seedPrefab.GetComponent<DropController>().collectableType = CollectableType.SEED;
        starPrefab.GetComponent<DropController>().collectableType = CollectableType.STAR;

        pcObj = GameObject.FindWithTag("Player");
        pc = pcObj.GetComponent<PlayerController>();
    }
    public enum CollectableType
    {
        SEED,
        HPREGEN,
        STAR,
        JUMP,
        DMG
    };
    public enum CultivableType
    {
        HPREGEN,
        JUMP,
        DMG
    };
    public enum PowerUpType
    {
        HPREGEN,
        JUMP,
        DMG
    };

    public void CultiveSeed(Transform ground) // plantar
    {
        if (!pc.isGrounded) return; // deve estar no chão
        if (pc.floorTag != "Ground") return; //deve estar colidindo com um chão plantável
        PlantInstance hasInSameGround = plantInstances.Where((p) => p.objInstance.transform.position.x == ground.transform.position.x).FirstOrDefault();
        if (hasInSameGround != null) return; // se ja tem no mesmo chão não gunciona
        pc.collectables.seeds--;

        GameObject instance = Instantiate(plantPrefab, ground.transform.position, Quaternion.identity);

        CultivableType[] enumValues = (CultivableType[])System.Enum.GetValues(typeof(CultivableType));
        CultivableType cultivableType = enumValues[Random.Range(0, enumValues.Length)];

        PlantController plantController = instance.GetComponent<PlantController>();
        plantController.cultivableType = cultivableType;

        plantController.spriteRenderer.sprite = sprites.Where(spr => spr.name == $"{CultivableTypeName(cultivableType)}_0").FirstOrDefault(); // nomes devem ser iguais

        plantController.animator.runtimeAnimatorController = animatorControllers.Where(anim => anim.name == CultivableTypeName(cultivableType)).FirstOrDefault();
        plantController.collectableManager = this;

        plantController.powerUpPrefab = powerUpPrefabs.Where(p => p.name + "Plant" == CultivableTypeName(cultivableType)).FirstOrDefault();

        plantInstances.Add(new PlantInstance(instance, plantController));
    }
    public void BornPlantPowerUp(Vector3 t, GameObject p, Quaternion r)
    {
        GameObject instance = Instantiate(p, t, r);
        PowerUpController puc = instance.GetComponent<PowerUpController>();
        PowerUpInstance pui = new PowerUpInstance(instance, puc);
        powerUpInstances.Add(pui);
        plantInstances = plantInstances
        .Where(plant => plant.objInstance.transform.position.x != t.x)
        .ToList();

    }

    public void SpawnDrop(float prob, Transform transf) // prob de drop e posição
    {
        //seed ou star:
        bool drop = Random.Range(1, 101) <= prob;
        if (!drop) return;

        rollete = Random.Range(0, 100);
        if (rollete < 5) // star drop
        {
            Instantiate(starPrefab, transf.position, Quaternion.identity);
        }
        else if (rollete < 34)
        {
            Instantiate(seedPrefab, transf.position, Quaternion.identity);
        }  // seed
           // ou não drop nada
    }

    private string CollectableTypeName(CollectableType type)
    {
        string returnString = "";
        switch (type)
        {
            case CollectableType.SEED:
                returnString = "Seed";
                break;
            case CollectableType.HPREGEN:
                returnString = "Hp";
                break;
            case CollectableType.STAR:
                returnString = "Star";
                break;
            case CollectableType.JUMP:
                returnString = "Jump";
                break;
            case CollectableType.DMG:
                returnString = "Dmg";
                break;
            default:
                break;
        }
        return returnString;
    }

    private string PowerUpTypeName(PowerUpType type)
    {
        string returnString = "";
        switch (type)
        {
            case PowerUpType.HPREGEN:
                returnString = "Hp";
                break;
            case PowerUpType.JUMP:
                returnString = "Jump";
                break;
            case PowerUpType.DMG:
                returnString = "Dmg";
                break;
            default:
                break;
        }
        return returnString;
    }

    private string CultivableTypeName(CultivableType type)
    {
        string returnString = "";
        switch (type)
        {
            case CultivableType.HPREGEN:
                returnString = "HpPlant";
                break;
            case CultivableType.JUMP:
                returnString = "JumpPlant";
                break;
            case CultivableType.DMG:
                returnString = "DmgPlant";
                break;
            default:
                break;
        }
        return returnString;
    }
    public class PlantInstance
    {
        public GameObject objInstance { get; set; }
        public PlantController plantController { get; set; }

        public PlantInstance(GameObject gameObject, PlantController plantController)
        {
            this.objInstance = gameObject;
            this.plantController = plantController;
        }
    }
    public class PowerUpInstance
    {
        public GameObject objInstance { get; set; }
        public PowerUpController powerUpController { get; set; }

        public PowerUpInstance(GameObject objInstance, PowerUpController powerUpController)
        {
            this.objInstance = objInstance;
            this.powerUpController = powerUpController;
        }
    }

    public void PowerUpCollect(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.HPREGEN:
                pc.collectables.hpregens++;
                break;
            case PowerUpType.JUMP:
                pc.collectables.jumps++;
                break;
            case PowerUpType.DMG:
                pc.collectables.dmgs++;
                break;
            default:
                break;
        }
    }

    // 0 - seed, 1 - hp, 2 - star, 3 - jump, 4 - dmg (deve subtrair 1 do botao apaertado: 1,2,3,4,5)
    public void UseCollectable(Transform gound, CollectableType type) // utilizável direto do inventário ou coletável usável
    {
        switch (type)
        {
            case CollectableType.SEED:
                CultiveSeed(gound); // ground é o transform do chão, se não existir ground usar transform do player
                break;
            case CollectableType.HPREGEN:
                ActivateHpRegenPowerUp();
                break;
            case CollectableType.STAR:
                ActivateStarBoost();
                break;
            case CollectableType.JUMP:
                ActivateJumpPowerUp();
                break;
            case CollectableType.DMG:
                ActivateDMGPowerUp();
                break;
            default:
                break;
        }
    }

    public void Collect(CollectableType type) // utilizável direto do inventário ou coletável usável
    {
        switch (type)
        {
            case CollectableType.SEED:
                pc.collectables.seeds++; // ground é o transform do chão, se não existir ground usar transform do player
                break;
            case CollectableType.HPREGEN:
                pc.collectables.hpregens++;
                break;
            case CollectableType.STAR:
                pc.collectables.stars++;
                break;
            case CollectableType.JUMP:
                pc.collectables.jumps++;
                break;
            case CollectableType.DMG:
                pc.collectables.dmgs++;
                break;
            default:
                break;
        }
    }

    private void ActivateDMGPowerUp()
    {
        if (pc.collectables.dmgs <= 0) return;
        pc.bulletDmg *= 1 + bulletDmgAugment;
        pc.collectables.dmgs--;
    }
    private void ActivateJumpPowerUp()
    {
        if (pc.collectables.jumps <= 0) return;
        pc.jumpForce *= 1 + jumpAugment;
        pc.collectables.jumps--;
    }
    private void ActivateHpRegenPowerUp()
    {
        if (pc.collectables.hpregens <= 0) return;
        pc.hp += hpRegenAugment;
        pc.collectables.hpregens--;
    }
    private void ActivateStarBoost()
    {
        if (pc.collectables.stars <= 0) return;
        if (pc.collectables.isStarBoostActivated) return;
        pc.collectables.stars--;
        pc.collectables.isStarBoostActivated = true;
    }
}
