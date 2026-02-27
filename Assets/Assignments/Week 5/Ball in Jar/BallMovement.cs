using UnityEngine;

public class JarFollower : MonoBehaviour
{
    public Rigidbody jarRb;   // isKinematic = true
    public Transform target;

    void FixedUpdate()
    {
        Physics.gravity = -Camera.main.transform.up;
        jarRb.MovePosition(target.position);
        jarRb.MoveRotation(target.rotation);
    }
}
