using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class RuntimePostProcessingController : MonoBehaviour
{
    private Bloom bloom;
    private WhiteBalance whiteBalance;
    private LensDistortion lensDistortion;

    void Awake()
    {
        VolumeProfile runtimeProfile = this.GetComponent<Volume>().profile;

        if (runtimeProfile.TryGet(out bloom))
            bloom.active = true;
        if (runtimeProfile.TryGet(out whiteBalance))
            whiteBalance.active = true;
        if (runtimeProfile.TryGet(out lensDistortion))
            lensDistortion.active = true;
    }

    public void SetBloomIntensity(float value)
    {
        if (bloom != null)
            bloom.intensity.value = value;
    }

    public void SetWhiteBalanceTemp(float value)
    {
        if (whiteBalance != null)
            whiteBalance.temperature.value = value;
    }

    public void SetWhiteBalanceTint(float value)
    {
        if (whiteBalance != null)
            whiteBalance.tint.value = value;
    }

    public void SetLensDistortionIntensity(float value)
    {
        if (lensDistortion != null)
            lensDistortion.intensity.value = value;
    }
}
