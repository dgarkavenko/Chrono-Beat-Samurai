using System;
using UnityEngine;

public enum EInstrument
{
    Bass = 1 << 0,
    Guitar = 1 << 2,
    Piano = 1 << 3,
    Vocals = 1 << 4,
    Drums = 1 << 5,
    Horn = 1 << 6,
    Misc = 1 << 15
}


[CreateAssetMenu(fileName = "StemData", menuName = "Audio/StemData", order = 1)]
public class StemData : ScriptableObject
{
    public AnimationCurve BPM;

    public InstrumentData[] InstrumentData;
}

[Serializable]
public class InstrumentData
{   
    public string name;

    public AudioClip[] audioClips;
    public EInstrument Instrument = EInstrument.Misc;
    public Sheet Notes;

    public AudioClip GetMixedClip()
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("No AudioClips to mix!");
            return null;
        }

        // Assuming all audio clips have the same number of channels and frequency
        int maxLength = 0;
        int channels = audioClips[0].channels;
        int frequency = audioClips[0].frequency;

        foreach (var clip in audioClips)
        {
            if (clip.channels != channels || clip.frequency != frequency)
            {
                Debug.LogError("AudioClips must have the same number of channels and frequency!");
                return null;
            }
            if (clip.samples > maxLength)
            {
                maxLength = clip.samples;
            }
        }

        float[] mixedSamples = new float[maxLength * channels];
        
        foreach (var clip in audioClips)
        {
            float[] clipSamples = new float[clip.samples * clip.channels];
            clip.GetData(clipSamples, 0);
            
            for (int i = 0; i < clipSamples.Length; i++)
            {
                // Summing the samples values to mix them
                mixedSamples[i] += clipSamples[i];
            }
        }

        // Normalize the mixed samples to prevent clipping
        float maxSample = 0f;
        for (int i = 0; i < mixedSamples.Length; i++)
        {
            if (Mathf.Abs(mixedSamples[i]) > maxSample)
            {
                maxSample = Mathf.Abs(mixedSamples[i]);
            }
        }
        if (maxSample > 0f)
        {
            for (int i = 0; i < mixedSamples.Length; i++)
            {
                mixedSamples[i] /= maxSample;
            }
        }

        // Create a new AudioClip
        AudioClip mixedClip = AudioClip.Create(this.name, mixedSamples.Length / channels, channels, frequency, false);
        mixedClip.SetData(mixedSamples, 0);

        return mixedClip;
    }

}
