using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class FaceObjectOnTrigger : MonoBehaviour
{
    public float rotationSpeed; // degrees per second
    private Transform target;
    private Quaternion initialRotation;

    void Start()
    {
        this.initialRotation = this.transform.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        this.target = other.transform;
    }

    void OnTriggerExit(Collider other)
    {
        this.target = null;
    }

    void Update()
    {
        Quaternion targetRotation = this.target == null
            ? Quaternion.Euler(Vector3.zero)
            : Quaternion.LookRotation(target.transform.position - transform.position);

        float step = rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            targetRotation,
            step
        );
    }
}
