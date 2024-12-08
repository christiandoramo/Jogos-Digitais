using UnityEngine;
using UnityEngine.UIElements;
using static PlayerController;

public class PlayerArms : MonoBehaviour
{
    public SpriteRenderer sr;
    private Vector3 invertedScale;
    private Vector3 normalScale;

    private void Start()
    {
        normalScale = transform.localScale;
        invertedScale = normalScale;
        invertedScale.y *= -1;

        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {

        StructMouseDirectionAndAngle structMouseDirectionAndAngle = GetStructMouseDirectionAndAngle(this.transform);

        if (structMouseDirectionAndAngle.direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, structMouseDirectionAndAngle.angle);
            transform.localScale = invertedScale;
        }
        else if (structMouseDirectionAndAngle.direction.x >= 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, structMouseDirectionAndAngle.angle);
            transform.localScale = normalScale;
        }
    }
}
