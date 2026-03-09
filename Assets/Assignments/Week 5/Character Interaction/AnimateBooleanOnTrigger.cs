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

    public void ToggleTrigger()
    {
        bool isTrigger = animator.GetBool(this.animationParameterName);
        animator.SetBool(this.animationParameterName, !isTrigger);
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
