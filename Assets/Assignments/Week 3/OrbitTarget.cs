using UnityEngine;

public class OrbitTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float angularSpeed; // deg/sec
    [SerializeField] private AnimationCurve animationCurve;
    private float lastTime;
    private float elapsed;
    private Vector3 startingScale;

    private void Start()
    {
        if (target == null)
            Debug.LogError($"{nameof(OrbitTarget)} on {name} has no target assigned.", this);

        startingScale = transform.localScale;

        if (animationCurve != null && animationCurve.length >= 0)
            lastTime = animationCurve.keys[animationCurve.length - 1].time;
        else
            lastTime = 1f;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.RotateAround(
            target.position,
            target.up,
            angularSpeed * Time.deltaTime
        );

        elapsed += Time.deltaTime;
        elapsed = Mathf.Repeat(elapsed, lastTime);
        float scale = animationCurve.Evaluate(elapsed);
        transform.localScale = startingScale * scale;
    }
}
