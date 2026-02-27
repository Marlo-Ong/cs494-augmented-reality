using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class DistributeChildrenOnSphere : MonoBehaviour
{
    public GameObject childPrefab;
    [Min(0f)] public float radius;
    public Material[] materials;

    static readonly float GoldenAngleRAD = Mathf.PI * (3f - Mathf.Sqrt(5f));

    [ContextMenu("Visualize")]
    public void Visualize()
    {
        if (childPrefab == null) return;
        int n = (materials == null) ? 0 : materials.Length;

        SyncChildrenCount(n);
        StartCoroutine(Visualize(n));
        AssignMaterials(n);
    }

    [ContextMenu("Apply")]
    public void Apply()
    {
        if (childPrefab == null) return;
        int n = (materials == null) ? 0 : materials.Length;

        SyncChildrenCount(n);
        PlaceChildren(n);
        AssignMaterials(n);
    }



    void SyncChildrenCount(int targetCount)
    {
        // Destroy extra children
        for (int i = transform.childCount - 1; i >= targetCount; i--)
        {
            Transform extra = transform.GetChild(i);
            if (Application.isPlaying)
                Destroy(extra.gameObject);
            else
                DestroyImmediate(extra.gameObject);
        }

        // Create missing children
        for (int i = transform.childCount; i < targetCount; i++)
        {
            GameObject go;
            if (Application.isPlaying)
                go = Instantiate(childPrefab, transform);
            else
                go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(childPrefab, transform);

            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = childPrefab.transform.localScale;
        }
    }

    void PlaceChildren(int n)
    {
        if (n <= 0)
            return;

        for (int i = 0; i < n; i++)
        {
            Transform child = transform.GetChild(i);

            // Sweep from north pole to south pole spaced evenly by surface area.
            float t = (n == 1) ? 0.5f : (i / (float)(n - 1)); // t goes from 0 -> 1
            float y = 1f - 2f * t; // y goes from 1 -> -1

            // Get polar coordinate. (the latitude ring at height y)
            // Choose angle such that the points spiral around the sphere.
            float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
            float theta = GoldenAngleRAD * i;

            // Convert polar to Cartesian coordinate.
            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;
            Vector3 dir = new Vector3(x, y, z).normalized;

            child.localPosition = dir * radius;
        }
    }


    void AssignMaterials(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Transform child = transform.GetChild(i);
            var rend = child.GetComponentInChildren<Renderer>();
            if (rend == null)
                continue;

            // Use sharedMaterial in edit mode to avoid creating instances constantly.
            if (Application.isPlaying)
                rend.material = materials[i];
            else
                rend.sharedMaterial = materials[i];
        }
    }

    #region Visualization
    IEnumerator Visualize(int n)
    {
        if (n <= 0)
            yield break;

        // Visualize heights.
        for (int i = 0; i < n; i++)
        {
            Transform child = transform.GetChild(i);

            // Sweep from north pole (height=1) to south pole (height=-1)
            // spaced evenly by surface area.
            float t = (n == 1) ? 0.5f : (i / (float)(n - 1)); // t goes from 0 -> 1
            float y = 1f - 2f * t; // y goes from 1 -> -1

            child.localPosition = new Vector3(0, y, 0) * radius;
            yield return new WaitForSeconds(0.1f);
        }

        // Visualize projecting outward.
        for (int i = 0; i < n; i++)
        {
            Transform child = transform.GetChild(i);
            float t = (n == 1) ? 0.5f : (i / (float)(n - 1));
            float y = 1f - 2f * t;

            // Get polar coordinate. (the latitude ring at height y)
            // Choose angle such that the points spiral around the sphere.
            float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
            float theta = 0;

            // Convert polar to Cartesian coordinate.
            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;
            Vector3 dir = new Vector3(x, y, z).normalized;

            // Project the normal vector outward.
            _ = PositionTween.TweenPositionAsync(
                child,
                dir * radius,
                0.2f
            );
            yield return new WaitForSeconds(0.1f);
        }

        // Visualize sliding along ring.
        float prevAngle = 0;
        for (int i = 0; i < n; i++)
        {
            Transform child = transform.GetChild(i);

            // Sweep from north pole to south pole spaced evenly by surface area.
            float t = (n == 1) ? 0.5f : (i / (float)(n - 1)); // t goes from 0 -> 1
            float y = 1f - 2f * t; // y goes from 1 -> -1

            // Get polar coordinate. (the latitude ring at height y)
            // Choose angle such that the points spiral around the sphere.
            float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
            float theta = GoldenAngleRAD * i;

            // Convert polar to Cartesian coordinate.
            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;
            Vector3 dir = new Vector3(x, y, z).normalized;

            _ = RingTween.TweenLocalPositionAlongRingClockwiseAsync(
                target: child,
                centerLocal: new Vector3(0, y, 0) * radius,
                upAxisLocal: transform.up,
                radius: r * radius,
                fromAngleDeg: prevAngle,
                toAngleDeg: Mathf.Repeat(Mathf.Rad2Deg * theta, 360),
                duration: 0.5f
            );

            DrawDebugCircleForSeconds(
                transform.TransformPoint(new Vector3(0, y, 0) * radius),
                r * radius * transform.lossyScale.x, 0.5f, Color.green);

            prevAngle = Mathf.Repeat(Mathf.Rad2Deg * theta, 360);

            yield return new WaitForSeconds(0.5f);
        }
    }

    void DrawDebugCircleForSeconds(
        Vector3 center,
        float radius,
        float seconds,
        Color color,
        int segments = 32)
    {
        float angleStep = 2f * Mathf.PI / segments;

        Vector3 prevPoint = center + (Vector3.right * radius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 newPoint = center + (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius);

            Debug.DrawLine(prevPoint, newPoint, color, seconds);
            prevPoint = newPoint;
        }
    }
    #endregion
}
