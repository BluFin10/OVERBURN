using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("Mixer Settings")]
    public AudioMixer masterMixer; 
    public string parameterName = "MusicLowPass";

    [Header("Frequency Settings")]
    public float muffledFreq = 500f;
    public float clearFreq = 22000f;
    public float transitionSpeed = 5f;

    public float targetFreq;
    public float currentFreq;
    

    // The "Instant Muffle" you wanted to keep here
    public void ToggleMuffle(float freq = 500f)
    {
        masterMixer.SetFloat(parameterName, freq);
    }

    // The method HeatManager will call every frame
    public void SetDynamicFrequency(float freq)
    {
        // We use SetFloat to talk to the 'Exposed Parameter' in the Mixer
        masterMixer.SetFloat(parameterName, freq);
    }
}

