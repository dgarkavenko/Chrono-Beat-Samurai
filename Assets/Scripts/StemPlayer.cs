using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;


public class StemPlayer : MonoBehaviour
{
    public StemData Stem; // Assign this array in the inspector with your StemData assets.
    
    private List<AudioSource> audioSources = new List<AudioSource>(); // To keep track of our AudioSources.
    public AudioMixerGroup LeadMixerGroup;
    public AudioMixerGroup BackupMixerGroup;
    public AudioVisualizer Visualizer;

    public double AudioSourceDspTime;

    private double _startDspTime;
    
    void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            StartCoroutine(PlayAllStems());
        }

        foreach (EInstrument instrument in System.Enum.GetValues(typeof(EInstrument)))
        {
            if (GUILayout.Button(instrument.ToString()))
            {
                HandleInstrumentButtonPress(instrument);
            }
        }
    }

    void HandleInstrumentButtonPress(EInstrument instrument)
    {
        for (int i = 0; i < Stem.InstrumentData.Length; i++)
        {
            if (Stem.InstrumentData[i].Instrument == instrument)
            {
                SelectTrack(i);
                break;
            }
        }
    }

    void SelectTrack(int i)
    {
        Visualizer.Select(i);
        foreach (var audioSource in audioSources)
            audioSource.outputAudioMixerGroup = BackupMixerGroup;

        audioSources[i].outputAudioMixerGroup = LeadMixerGroup;
        
    }

    void Start()
    {
        // Create an AudioSource for each StemData and play it.
        foreach (var instrumentData in Stem.InstrumentData)
        {
            var child = new GameObject($"{instrumentData.name}");
            child.transform.parent = transform;

            AudioSource source = child.AddComponent<AudioSource>();
            AudioClip mergedClip = instrumentData.GetMixedClip();
            if (mergedClip != null)
            {
                source.clip = mergedClip;
                source.playOnAwake = false;
                source.outputAudioMixerGroup = BackupMixerGroup;
                audioSources.Add(source);
            }
            else
            {
                Debug.LogError("Merged clip was null, cannot play audio for: " + instrumentData.name);
            }
        }

        AudioClip[] audioClips = audioSources.Where(source => source != null && source.clip != null)
                                         .Select(source => source.clip)
                                         .ToArray();

        Visualizer.Load(audioClips, Stem.BPM);
    }

    void Update()
    {
        if(_startDspTime > 0)
        {
            AudioSourceDspTime = AudioSettings.dspTime - _startDspTime;
            Visualizer.UpdateTrack(AudioSourceDspTime);
        }
    }

    private IEnumerator PlayAllStems()
    {
        yield return null; // Wait one frame to ensure all AudioSources are ready.

        foreach (var source in audioSources)
            source.Play();
        
        _startDspTime = AudioSettings.dspTime;
    }

    void OnDestroy()
    {
        // Clean up the dynamically created AudioClips to avoid memory leaks.
        foreach (var source in audioSources)
        {
            if (source.clip != null)
            {
                Destroy(source.clip);
            }
        }
    }
}
