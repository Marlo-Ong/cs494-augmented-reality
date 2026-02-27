using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsARTracking : MonoBehaviour
{
    public Transform ARTarget;
    private new Rigidbody rigidbody;

    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    public void AttachToTarget()
    {
        this.rigidbody.isKinematic = true;
        this.transform.SetParent(this.ARTarget, worldPositionStays: false);
        this.transform.position = this.ARTarget.transform.position;
    }

    public void DetachFromTarget()
    {
        this.transform.SetParent(null);
        this.rigidbody.isKinematic = false;
    }
}
