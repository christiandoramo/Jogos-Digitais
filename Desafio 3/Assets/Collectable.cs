using UnityEngine;
using static PlayerController;

public class CollectableGeneric : ScriptableObject
{

    [SerializeField] PlayerController pc;
    [SerializeField] GameObject pcObj;
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
    public void InitializeSeed()
    {
        pc.collectables.seeds--;

        CultivableType[] enumValues = (CultivableType[])System.Enum.GetValues(typeof(CultivableType));

        CultivableType randomType = enumValues[Random.Range(0, enumValues.Length)];

        switch (randomType)
        {
            case CultivableType.HPREGEN:

                break;
            case CultivableType.JUMP:

                break;
            case CultivableType.DMG:

                break;
            default:
                break;
        }
    }

    public void ExeecuteEffect(CollectableType type) // (inicia em 0 na vdd)
    // 1 - semente, 2 - regen +hp (usa ao pegar), 3 - estrela, 4 - +jump (usa ao pegar), 5 - DMG (usa ao pegar)
    {
        switch (type)
        {
            case CollectableType.SEED:
                pc.collectables.seeds--;
                break;
            case CollectableType.HPREGEN:
                pc.collectables.hpregens--;
                break;
            case CollectableType.STAR:
                pc.collectables.stars--;
                break;
            case CollectableType.JUMP:
                pc.collectables.jumps--;
                pc.jumpForce *= 1 + jumpAugment;
                break;
            case CollectableType.DMG:
                pc.bulletDmg *= 1 + bulletDmgAugment;
                pc.collectables.dmgs--;
                break;
            default:
                break;
        }
    }

    public void NewCollectableSpawn() // (inicia em 0 na vdd)
    // 1 - semente, 2 - regen +hp (usa ao pegar), 3 - estrela, 4 - +jump (usa ao pegar), 5 - DMG (usa ao pegar)
    {

        CollectableType[] enumValues = (CollectableType[])System.Enum.GetValues(typeof(CollectableType));

        CollectableType randomType = enumValues[Random.Range(0, enumValues.Length)];

        switch (randomType)
        {
            case CollectableType.HPREGEN:
                pc.collectables.hpregens++;
                break;
            case CollectableType.JUMP:
                pc.collectables.jumps++;
                break;
            case CollectableType.DMG:
                pc.collectables.dmgs++;
                break;
            case CollectableType.SEED:
                pc.collectables.seeds++;
                break;
            case CollectableType.STAR:
                pc.collectables.stars++;
                break;
            default:
                break;
        }
    }
}
