using UnityEngine;

public class AudioClipWindow : MonoBehaviour
{
    public AudioClip originalClip; // Assign in the inspector
    public float windowStart; // The start time of the window
    public float windowEnd;   // The end time of the window
    public AudioClip newClip; // The new clip, accessible publicly

    // Call this method to create a new clip segment from the windowStart to windowEnd
    public void CreateClipSegment()
    {
        if (originalClip == null)
        {
            Debug.LogError("Original clip is not assigned.");
            return;
        }

        if (windowStart < 0 || windowStart > originalClip.length || windowEnd < 0 || windowEnd > originalClip.length || windowStart >= windowEnd)
        {
            Debug.LogError("Invalid start or end time.");
            return;
        }

        int startSample = (int)(windowStart * originalClip.frequency);
        int endSample = (int)(windowEnd * originalClip.frequency);
        int sampleWindow = endSample - startSample;

        // Create a new audio clip
        newClip = AudioClip.Create("clipSegment", sampleWindow, originalClip.channels, originalClip.frequency, false);

        // Create a temporary buffer and copy the segment of the samples
        float[] data = new float[sampleWindow * originalClip.channels];
        originalClip.GetData(data, startSample);

        // Set the data to the new clip
        newClip.SetData(data, 0);
    }

    // Example usage: Call this method to play the newClip
    public void PlayNewClip()
    {
        if (newClip != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = newClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("New clip has not been created.");
        }
    }

    // For demonstration, let's create and play a segment when the game starts
    void Start()
    {
        CreateClipSegment(); // Create the new clip segment
        PlayNewClip();       // Play the new clip
    }
}
