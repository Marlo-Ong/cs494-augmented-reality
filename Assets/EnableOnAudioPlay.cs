using UnityEngine;

public class EnableOnAudioPlay : MonoBehaviour
{
    public AudioSource source;
    public new GameObject gameObject;

    void Update()
    {
        if (source.isPlaying && !this.gameObject.activeSelf)
            this.gameObject.SetActive(true);
        else if (!source.isPlaying && this.gameObject.activeSelf)
            this.gameObject.SetActive(false);
    }
}
