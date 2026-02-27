using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interpolate the base color of all materials of a given
/// renderer when this object enters a trigger.
/// </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ChangeColorOnTrigger : MonoBehaviour
{
    public Color targetColor;
    public float lerpSpeed;
    public Renderer targetRenderer;

    private class ColorTracker
    {
        private Color? targetColor;
        private readonly Material material;
        private readonly Color startingColor;
        private readonly float lerpSpeed;
        private const string MaterialColorName = "_BaseColor";

        public ColorTracker(Material material, float lerpSpeed = 1.0f)
        {
            this.material = material;
            this.startingColor = material.GetColor(MaterialColorName);
            this.targetColor = this.startingColor;
            this.lerpSpeed = lerpSpeed;
        }

        public void SetTargetColor(Color? color)
        {
            this.targetColor = color;
        }

        public void Update(float deltaTime)
        {
            Color current = this.material.GetColor(MaterialColorName);
            Color next = Color.Lerp(
                current,
                this.targetColor ?? this.startingColor,
                this.lerpSpeed * deltaTime);

            this.material.SetColor(MaterialColorName, next);
        }
    }

    private readonly List<ColorTracker> colorTrackers = new();

    void Start()
    {
        foreach (var material in targetRenderer.materials)
        {
            var tracker = new ColorTracker(material, this.lerpSpeed);
            this.colorTrackers.Add(tracker);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (var tracker in this.colorTrackers)
            tracker.SetTargetColor(this.targetColor);
    }

    void OnTriggerExit(Collider other)
    {
        foreach (var tracker in this.colorTrackers)
            tracker.SetTargetColor(null);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (var tracker in this.colorTrackers)
            tracker.Update(deltaTime);
    }
}
