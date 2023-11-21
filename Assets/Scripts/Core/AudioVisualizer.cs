using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public partial class AudioVisualizer : MonoBehaviour
{
    public int pixelsPerSecond = 100;
    public float heightMultiplier = 100.0f;
    public Color[] waveformColors;
    private int wfcolorindex;

    public Color barColor = Color.red;
    public int textureWidth = 2048;
    public RectTransform panel; // The parent panel for the RawImages

    private AudioSource audioSource;
    private List<RawImage>[] _imagesPerInstrument;
    private float textureScrollSpeed;
    private float _bpm;
    private float pixelsPerBar;
    public float secondsPerBar;
    public float beatDuration;

    public float AudioSourceTime;

    public float songPositionInBeats;

    private int _selection = 0;

    public void Load(AudioClip[] audioClips, AnimationCurve animationCurve)
    {
        if (panel == null)
        {
            Debug.LogError("Panel RectTransform not assigned.");
            return;
        }

        _bpm = animationCurve.keys[0].value;
        beatDuration = 60.0f / _bpm;

        secondsPerBar = beatDuration * 4;
        pixelsPerBar = pixelsPerSecond * secondsPerBar * 4;

        // Calculate total texture width needed
        int totalWidth = Mathf.CeilToInt(audioClips[0].length * pixelsPerSecond);
        
        _imagesPerInstrument = new List<RawImage>[audioClips.Length];

        for (int clip_i = 0; clip_i < audioClips.Length; clip_i++)
        {
            _imagesPerInstrument[clip_i] = new List<RawImage>();

            for (int i = 0; i * textureWidth < totalWidth; i++)
            {
                GameObject rawImageObj = new GameObject(audioClips[clip_i].name + i);
                rawImageObj.transform.SetParent(panel);

                RawImage rawImage = rawImageObj.AddComponent<RawImage>();
                rawImage.rectTransform.sizeDelta = new Vector2(textureWidth, panel.rect.height);
                rawImage.rectTransform.anchorMin = new Vector2(0, 0.5f);
                rawImage.rectTransform.anchorMax = new Vector2(0, 0.5f);
                rawImage.rectTransform.pivot = new Vector2(0, 0.5f);
                rawImage.rectTransform.anchoredPosition = new Vector2(i * textureWidth, 0);
                rawImage.enabled = false;

                _imagesPerInstrument[clip_i].Add(rawImage);
            }

            float firstBar = animationCurve.keys[0].time;
            foreach (var rawImage in _imagesPerInstrument[clip_i])
            {
                rawImage.texture = GenerateWaveformSegment(rawImage.rectTransform.anchoredPosition.x / pixelsPerSecond, ref firstBar, audioClips[clip_i]);
            }

        }

        textureScrollSpeed = (float)pixelsPerSecond / audioClips[0].frequency;

    }

    public void Select(int new_selection)
    {
        foreach (var rawImage in _imagesPerInstrument[_selection])
            rawImage.enabled = false;

        foreach (var rawImage in _imagesPerInstrument[new_selection])
            rawImage.enabled = true;

        _selection = new_selection;

    }

    public void UpdateTrack(double AudioSourceDspTime)
    {
        float playbackPosition = (float)(AudioSourceDspTime * pixelsPerSecond);

        songPositionInBeats = (float)(AudioSourceDspTime / secondsPerBar / 4);

            for (int i = 0; i < _imagesPerInstrument[_selection].Count; i++)
            {
                // Set the position of each segment
                float segmentPosition = i * textureWidth - playbackPosition;
                _imagesPerInstrument[_selection][i].rectTransform.anchoredPosition = new Vector2(segmentPosition, 0);
            }
    }

    // Add more methods if necessary
}
