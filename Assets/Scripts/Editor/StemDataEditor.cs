using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StemData))]
public class StemDataEditor : Editor
{
    private GameObject _stemPlayer;
    private AudioSource audioSource;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Add a custom button to play all clips
        if (GUILayout.Button("Play"))
        {
            PlayAllClips();
        }

        if (GUILayout.Button("Stop"))
        {
            Stop();
        }
    }

    private void Stop()
    {
        var stemPlayer = GameObject.Find("StemPlayer");
        if(stemPlayer)
            DestroyImmediate(stemPlayer);
    }

    private void PlayAllClips()
    {
        Stop();

        // StemData data = (StemData)target;
        // if (data.audioClips == null || data.audioClips.Length == 0)
        // {
        //     Debug.LogError("No AudioClips in the array!");
        //     return;
        // }

        // // Find existing StemPlayer or create a new one
        // _stemPlayer = GameObject.Find("StemPlayer") ?? new GameObject("StemPlayer");
        // audioSource = _stemPlayer.AddComponent<AudioSource>();

        // // Play each clip using PlayOneShot
        // foreach (AudioClip clip in data.audioClips)
        // {
        //     if (clip != null)
        //     {
        //         audioSource.PlayOneShot(clip);
        //     }
        // }
    }

    void OnDestroy()
    {
        // Clean up our player when the inspector is destroyed or if we hit play (which destroys all temporary objects)
        if (_stemPlayer != null)
        {
            //DestroyImmediate(stemPlayer);
        }
    }
}
