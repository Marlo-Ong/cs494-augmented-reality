#if UNITY_EDITOR
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class PositionTween
{
    /// <summary>
    /// Tweens a Transform's position in the editor using async/await.
    /// Fire-and-forget by default; pass a CancellationToken to stop early.
    /// </summary>
    public static async Task TweenPositionAsync(
        Transform target,
        Vector3 to,
        float duration,
        CancellationToken cancellationToken = default,
        Action onComplete = null)
    {
        if (!target)
            return;

        if (duration <= 0f)
        {
            target.localPosition = to;
            onComplete?.Invoke();
            SceneView.RepaintAll();
            return;
        }

        Vector3 from = target.localPosition;
        double startTime = EditorApplication.timeSinceStartup;

        while (true)
        {
            if (cancellationToken.IsCancellationRequested || !target)
                return;

            double elapsed = EditorApplication.timeSinceStartup - startTime;
            float t = (float)(elapsed / duration);

            if (t >= 1f)
                break;

            target.localPosition = Vector3.LerpUnclamped(from, to, t);

            SceneView.RepaintAll();
            await Task.Yield();
        }

        if (!cancellationToken.IsCancellationRequested && target)
        {
            target.localPosition = to;
            onComplete?.Invoke();
            SceneView.RepaintAll();
        }
    }
}

public static class RingTween
{
    /// <summary>
    /// Tweens localPosition along a ring, always moving clockwise when viewed along upAxisLocal.
    /// Angles are degrees. "Clockwise" here means the angle decreases (right-hand rule around upAxisLocal).
    /// </summary>
    public static async Task TweenLocalPositionAlongRingClockwiseAsync(
        Transform target,
        Vector3 centerLocal,
        Vector3 upAxisLocal,
        float radius,
        float fromAngleDeg,
        float toAngleDeg,
        float duration,
        AnimationCurve ease = null,
        CancellationToken cancellationToken = default,
        Action onComplete = null)
    {
        if (!target) return;

        radius = Mathf.Abs(radius);

        if (radius == 0f)
        {
            target.localPosition = centerLocal;
            onComplete?.Invoke();
            SceneView.RepaintAll();
            return;
        }

        // Adjust destination to the equivalent angle reachable by decreasing angle (clockwise).
        float cwTo = MakeClockwiseDestination(fromAngleDeg, toAngleDeg);

        if (duration <= 0f)
        {
            target.localPosition = PointOnRing(centerLocal, upAxisLocal, radius, cwTo);
            onComplete?.Invoke();
            SceneView.RepaintAll();
            return;
        }

        ease ??= AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        var up = upAxisLocal.sqrMagnitude > 1e-8f ? upAxisLocal.normalized : Vector3.up;
        BuildRingBasis(up, out var right, out var forward);

        double startTime = EditorApplication.timeSinceStartup;

        while (true)
        {
            if (cancellationToken.IsCancellationRequested || !target)
                return;

            double elapsed = EditorApplication.timeSinceStartup - startTime;
            float t = (float)(elapsed / duration);

            if (t >= 1f) break;

            float u = ease.Evaluate(Mathf.Clamp01(t));
            float ang = Mathf.LerpUnclamped(fromAngleDeg, cwTo, u);

            target.localPosition = centerLocal + RingOffset(right, forward, radius, ang);

            SceneView.RepaintAll();
            await Task.Yield();
        }

        if (!cancellationToken.IsCancellationRequested && target)
        {
            target.localPosition = centerLocal + RingOffset(right, forward, radius, cwTo);
            onComplete?.Invoke();
            SceneView.RepaintAll();
        }
    }

    /// <summary>
    /// Returns an equivalent of toAngleDeg (toAngleDeg - 360*k) such that it is <= fromAngleDeg
    /// and is the "nearest" clockwise destination (smallest magnitude negative delta).
    /// </summary>
    private static float MakeClockwiseDestination(float fromAngleDeg, float toAngleDeg)
    {
        // Normalize both to [0, 360)
        float fromN = Mathf.Repeat(fromAngleDeg, 360f);
        float toN = Mathf.Repeat(toAngleDeg, 360f);

        // Clockwise delta is negative (or zero).
        float delta = toN - fromN;     // in (-360, 360)
        if (delta > 0f) delta -= 360f; // force non-positive

        // Apply that delta to the original (unwrapped) from angle to preserve continuity.
        return fromAngleDeg + delta;
    }

    // --- Math helpers ---

    private static Vector3 PointOnRing(Vector3 center, Vector3 upAxis, float radius, float angleDeg)
    {
        var up = upAxis.sqrMagnitude > 1e-8f ? upAxis.normalized : Vector3.up;
        BuildRingBasis(up, out var right, out var forward);
        return center + RingOffset(right, forward, radius, angleDeg);
    }

    private static Vector3 RingOffset(Vector3 right, Vector3 forward, float radius, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return (right * c + forward * s) * radius;
    }

    private static void BuildRingBasis(Vector3 up, out Vector3 right, out Vector3 forward)
    {
        Vector3 refAxis = Mathf.Abs(Vector3.Dot(up, Vector3.up)) < 0.99f ? Vector3.up : Vector3.right;
        right = Vector3.Cross(up, refAxis).normalized;
        forward = Vector3.Cross(right, up).normalized;
    }
}

#endif
