using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private AudioSource musicSource;

    void Start()
    {
        this.musicSource = this.GetComponent<AudioSource>();
    }

    public void SetMusicPlaying(bool enabled)
    {
        if (enabled)
            this.musicSource.UnPause();
        else
            this.musicSource.Pause();
    }

    public void SetMusicVolume(float value)
    {
        float db = Mathf.Approximately(value, 0) ? -80 : 20 * Mathf.Log10(value);
        this.audioMixer.SetFloat("Volume_Music", db);
    }

    public void SetSFXVolume(float value)
    {
        float db = Mathf.Approximately(value, 0) ? -80 : 20 * Mathf.Log10(value);
        this.audioMixer.SetFloat("Volume_SFX", db);
    }
}
