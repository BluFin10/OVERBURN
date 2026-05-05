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

    private float targetFreq;
    private float currentFreq;

    void Start()
    {
        targetFreq = clearFreq;
        currentFreq = clearFreq;
    }

    void Update()
    {
        // Smoothly interpolate the frequency value
        currentFreq = Mathf.Lerp(currentFreq, targetFreq, Time.deltaTime * transitionSpeed);
        
        // Apply the value to the Mixer
        masterMixer.SetFloat(parameterName, currentFreq);

        // Press 'M' to test
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMuffle();
        }
    }

    public void ToggleMuffle()
    {
        targetFreq = (targetFreq == clearFreq) ? muffledFreq : clearFreq;
    }
}
