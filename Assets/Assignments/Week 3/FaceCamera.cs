using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        this.target = Camera.main.transform;
    }

    void Update()
    {
        this.transform.LookAt(target, this.transform.up);
    }
}
