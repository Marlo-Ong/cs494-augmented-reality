using System;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeMaterialModifier : MonoBehaviour
{
    public Renderer targetRenderer;
    public Slider metallicSlider;
    public Slider smoothnessSlider;

    // We’ll cache an instance material so we don't edit the shared asset
    private Material _mat;

    // Property IDs (fast + avoids typos)
    private static readonly int Color_URP = Shader.PropertyToID("_BaseColor");
    private static readonly int Metallic = Shader.PropertyToID("_Metallic");
    private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

    void Awake()
    {
        if (!targetRenderer)
        {
            Debug.LogError("ObjectMaterialUIController: targetRenderer not set.");
            enabled = false;
            return;
        }

        _mat = targetRenderer.material;

        // Hook up slider callbacks
        if (metallicSlider)
            metallicSlider.onValueChanged.AddListener(SetMetallic);

        if (smoothnessSlider)
            smoothnessSlider.onValueChanged.AddListener(SetSmoothness);

        // Initialize UI from material
        if (metallicSlider && _mat.HasProperty(Metallic))
            metallicSlider.value = _mat.GetFloat(Metallic);

        if (smoothnessSlider)
            smoothnessSlider.value = _mat.GetFloat(Smoothness);

        _mat.SetColor(Color_URP, new Color(0, 0, 0));
    }

    // --- Button callbacks ---
    public void SetColorRed() => IncrementRedColor();
    public void SetColorGreen() => IncrementGreenColor();
    public void SetColorBlue() => IncrementBlueColor();

    public void IncrementRedColor()
    {
        if (_mat == null) return;

        if (_mat.HasProperty(Color_URP))
        {
            Color color = _mat.GetColor(Color_URP);
            float newR = Mathf.Repeat(color.r + 0.1f, 1.0f);
            _mat.SetColor(Color_URP, new Color(newR, color.g, color.b));
        }
    }

    public void IncrementGreenColor()
    {
        if (_mat == null) return;

        if (_mat.HasProperty(Color_URP))
        {
            Color color = _mat.GetColor(Color_URP);
            float newG = Mathf.Repeat(color.g + 0.1f, 1.0f);
            _mat.SetColor(Color_URP, new Color(color.r, newG, color.b));
        }
    }

    public void IncrementBlueColor()
    {
        if (_mat == null) return;

        if (_mat.HasProperty(Color_URP))
        {
            Color color = _mat.GetColor(Color_URP);
            float newB = Mathf.Repeat(color.b + 0.1f, 1.0f);
            _mat.SetColor(Color_URP, new Color(color.r, color.g, newB));
        }
    }

    // --- Slider callbacks ---
    public void SetMetallic(float v)
    {
        if (_mat == null) return;
        if (_mat.HasProperty(Metallic))
            _mat.SetFloat(Metallic, v);
    }

    public void SetSmoothness(float v)
    {
        if (_mat == null) return;

        if (_mat.HasProperty(Smoothness))
            _mat.SetFloat(Smoothness, v);
    }
}