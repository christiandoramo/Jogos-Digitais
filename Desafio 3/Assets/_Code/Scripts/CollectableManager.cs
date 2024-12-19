using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static CollectableManager;

[CreateAssetMenu(fileName = "CollectableManager", menuName = "ScriptableObjects/CollectableManager", order = 2)]
public class CollectableManager : ScriptableObject
{

    [SerializeField] PlayerController pc;
    [SerializeField] GameObject pcObj;
    [SerializeField]
    RuntimeAnimatorController[] animatorControllers;
    [SerializeField] Sprite[] sprites;
    private int rollete;

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

    public void Initialize()
    {
        seedPrefab.GetComponent<DropController>().collectableType = CollectableType.SEED;
        starPrefab.GetComponent<DropController>().collectableType = CollectableType.STAR;

        pcObj = GameObject.FindWithTag("Player");
        pc = pcObj.GetComponentInParent<PlayerController>();
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
        if (pc.collectables.seeds <= 0) return;
        if (!pc.isGrounded) return; // deve estar no chão
        if (pc.floorTag != "Ground") return; //deve estar colidindo com um chão plantável
        PlantInstance hasInSameGround = plantInstances.Where((p) => p.objInstance.transform.position.x == ground.transform.position.x).FirstOrDefault();
        if (hasInSameGround != null) return; // se ja tem no mesmo chão não gunciona
        pc.collectables.seeds--;

        // Vector.up para ajustar para nascer em cima
        GameObject instance = Instantiate(plantPrefab, ground.transform.position + Vector3.up * .5f, Quaternion.identity);

        CultivableType[] enumValues = (CultivableType[])System.Enum.GetValues(typeof(CultivableType));
        CultivableType cultivableType = enumValues[Random.Range(0, enumValues.Length)];

        PlantController plantController = instance.GetComponent<PlantController>();

        plantController.cultivableType = cultivableType;

        plantController.spriteRenderer.sprite = sprites.Where(spr => spr.name == $"{CultivableTypeName(cultivableType)}_0").FirstOrDefault(); // nomes devem ser iguais
        plantController.animator.runtimeAnimatorController = animatorControllers.Where(anim => anim.name == CultivableTypeName(cultivableType)).FirstOrDefault();

        //powerUpPrefabs.ForEach(p => Debug.Log("p.name: " + p.name + "Plant")); // teste de nome dos powerUps

        plantController.powerUpPrefab = powerUpPrefabs.Where(p => p.name + "Plant" == "PowerUp" + CultivableTypeName(cultivableType)).FirstOrDefault();

        plantInstances.Add(new PlantInstance(instance, plantController));
    }
    public void BornPlantPowerUp(CultivableType ct, Vector3 t, GameObject p, Quaternion r) // posição de spawn, prefab de powerUp, quaternion
    {
        GameObject instance = Instantiate(p, t, r);
        PowerUpController puc = instance.GetComponent<PowerUpController>();

        puc.powerUpType = ConvertCultivateTypeToPowerUpType(ct);
        PowerUpInstance pui = new PowerUpInstance(instance, puc);
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
        if (rollete < 15) // star drop
        {
            Instantiate(starPrefab, transf.position, Quaternion.identity);
        }
        else
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
    public PowerUpType ConvertCultivateTypeToPowerUpType(CultivableType type)
    {
        PowerUpType returnType = PowerUpType.HPREGEN;
        switch (type)
        {
            case CultivableType.HPREGEN:
                returnType = PowerUpType.HPREGEN;
                break;
            case CultivableType.JUMP:
                returnType = PowerUpType.JUMP;
                break;
            case CultivableType.DMG:
                returnType = PowerUpType.DMG;
                break;
            default:
                break;
        }
        return returnType;
    }

    public CollectableType ConvertPowerUpTypeToCollectableType(PowerUpType type)
    {
        CollectableType returnType = CollectableType.HPREGEN;
        switch (type)
        {
            case PowerUpType.HPREGEN:
                returnType = CollectableType.HPREGEN;
                break;
            case PowerUpType.JUMP:
                returnType = CollectableType.JUMP;
                break;
            case PowerUpType.DMG:
                returnType = CollectableType.DMG;
                break;
            default:
                break;
        }
        return returnType;
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
        Debug.Log("Coletou: " + type.HumanName());
        Debug.Log("pc: " + pc.ToString());
        Debug.Log("pc.collectables: " + pc.collectables.ToString());
        Debug.Log("pc.collectables.seeds: " + pc.collectables.seeds.ToString());

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
        Debug.Log("DMG novo: " + pc.bulletDmg);
    }
    private void ActivateJumpPowerUp()
    {
        if (pc.collectables.jumps <= 0) return;
        pc.jumpForce *= 1 + jumpAugment;
        pc.superJump = true;
        pc.collectables.jumps--;
        Debug.Log("Jump novo: " + pc.jumpForce);

    }
    private void ActivateHpRegenPowerUp()
    {
        Debug.Log("hp Entrou");
        if (pc.collectables.hpregens <= 0) return;
        pc.maxHp += hpRegenAugment; // aumento de hp
        pc.hp = pc.maxHp; // cura total
        Debug.Log("hp Passou");
        Debug.Log("HP novo: " + pc.hp);
        pc.collectables.hpregens--;
    }
    private void ActivateStarBoost()
    {
        if (pc.collectables.stars <= 0) return;
        if (pc.isStarBoostActivated) return;
        pc.collectables.stars--;
        pc.isStarBoostActivated = true;
        pc.HandleStarBoost();
    }
}
