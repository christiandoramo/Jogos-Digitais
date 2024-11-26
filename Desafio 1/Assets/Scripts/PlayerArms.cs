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
        // transform.rotation = Quaternion.Euler(0, 0, structMouseDirectionAndAngle.angle); // tratando angulo para onde a arma aponta no geral
        Debug.Log($"angle: {structMouseDirectionAndAngle.angle}");
        // 90 até -90 é lado direito

        //if (structMouseDirectionAndAngle.angle <= 90f && structMouseDirectionAndAngle.angle > -90f)
        //{
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
        //}
        //else if (structMouseDirectionAndAngle.angle > 90f)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, structMouseDirectionAndAngle.angle % 90f);
        //    //transform.lossyScale.Set(-1.5f, transform.localScale.y, transform.localScale.z);
        //    transform.localScale = invertedScale;

        //}
        //else if (structMouseDirectionAndAngle.angle <= -90f)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, (-1 * structMouseDirectionAndAngle.angle) % 90f);
        //    //transform.localScale.Set(1000f, 1000f, 1000f);
        //    transform.localScale = invertedScale;
        //}
    }
}
