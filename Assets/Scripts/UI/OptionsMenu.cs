using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetVolume(float volume)
    {
        // Convert linear (0.0001–1) to logarithmic dB scale
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("volume", dB);
    }
}