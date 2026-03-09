using UnityEngine;

public class ObjectScaler : MonoBehaviour
{
    public void SetScaleX(float value)
    {
        var scale = this.gameObject.transform.localScale;
        this.gameObject.transform.localScale = new Vector3(value, scale.y, scale.z);
    }

    public void SetScaleY(float value)
    {
        var scale = this.gameObject.transform.localScale;
        this.gameObject.transform.localScale = new Vector3(scale.x, value, scale.z);
    }

    public void SetScaleZ(float value)
    {
        var scale = this.gameObject.transform.localScale;
        this.gameObject.transform.localScale = new Vector3(scale.x, scale.y, value);
    }
}
