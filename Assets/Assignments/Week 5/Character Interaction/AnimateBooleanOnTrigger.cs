using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(Animator))]
public class AnimateBooleanOnTrigger : MonoBehaviour
{
    public string animationParameterName;
    private Animator animator;

    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        animator.SetBool(this.animationParameterName, true);
    }

    void OnTriggerExit(Collider other)
    {
        animator.SetBool(this.animationParameterName, false);
    }
}
