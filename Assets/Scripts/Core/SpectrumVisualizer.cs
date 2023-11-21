using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpectrumVisualizer : MonoBehaviour
{
    public float NoteSensetivity;
    public float _currentNoteSensetivity;
    public float BPM = 112;
    private float Deaf => 60 / BPM;
    public int BarsToAddNote;

    public StemPlayer Player;

    public RectTransform panel; // The parent panel containing the bar images
    public int numberOfBars = 64; // The number of frequency bars to display
    public float minHeight = 0.2f; // Minimum visible height of a bar
    public float maxHeight = 5.0f; // Maximum height a bar can reach
    public float updateSensitivity = 0.5f; // Determines how quickly the bars respond to the sound
    public int spectrumSampleSize = 1024; // Number of samples, must be a power of 2
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // Algorithm used to process the spectrum data

    private List<Image> bars = new List<Image>();
    private double _deafUntil;

    private Image SensitivityBar;

    void Start()
    {
        if (Player)
            Player.Stem.InstrumentData[0].Notes.Clear();

        _currentNoteSensetivity = NoteSensetivity;
        float barWidth = panel.rect.width / numberOfBars;

        GameObject barObj = new GameObject("Sensi");
        barObj.transform.SetParent(panel, false);

        SensitivityBar = barObj.AddComponent<Image>();
        SensitivityBar.color = new Color(0.3f, 0.4f, 0.6f, 0.3f); // Set the color of the bars (default is white)
        SensitivityBar.rectTransform.sizeDelta = new Vector2(panel.rect.width, minHeight);
        SensitivityBar.rectTransform.anchorMin = new Vector2(0, 0);
        SensitivityBar.rectTransform.anchorMax = new Vector2(0, 0);
        SensitivityBar.rectTransform.pivot = new Vector2(0.5f, 0);
        SensitivityBar.rectTransform.anchoredPosition = new Vector2(panel.rect.width * .5f, 0);

        for (int i = 0; i < numberOfBars; i++)
        {
            barObj = new GameObject("Bar" + i);
            barObj.transform.SetParent(panel, false);

            Image barImage = barObj.AddComponent<Image>();
            barImage.color = Color.white; // Set the color of the bars (default is white)
            barImage.rectTransform.sizeDelta = new Vector2(barWidth, minHeight);
            barImage.rectTransform.anchorMin = new Vector2(0, 0);
            barImage.rectTransform.anchorMax = new Vector2(0, 0);
            barImage.rectTransform.pivot = new Vector2(0.5f, 0);
            barImage.rectTransform.anchoredPosition = new Vector2(i * barWidth, 0);
            bars.Add(barImage);
        }
    }


    public float recoveryRate = 1;    
    public float lossRate = 10;

    void Update()
    {
        float[] spectrum = new float[spectrumSampleSize];
        AudioListener.GetSpectrumData(spectrum, 0, fftWindow);
        
        int triggeredBars = 0;

        _currentNoteSensetivity = Mathf.MoveTowards(_currentNoteSensetivity, NoteSensetivity, recoveryRate * Time.deltaTime);

        for (int i = 0; i < numberOfBars; i++)
        {
            if (i < spectrum.Length)
            {
                float intensity = Mathf.Clamp(spectrum[i] * (maxHeight - minHeight) + minHeight, minHeight, maxHeight);
                
                bool triggers = false;
                if(BarsToAddNote > 0 &&  intensity > _currentNoteSensetivity && _deafUntil < AudioSettings.dspTime)
                {
                    triggers = true;
                }

                if(triggers)
                    triggeredBars++;

                bars[i].rectTransform.sizeDelta = new Vector2(bars[i].rectTransform.sizeDelta.x, intensity);
                bars[i].color = intensity > _currentNoteSensetivity ? Color.yellow : Color.white;
            }
        }

        SensitivityBar.color = AudioSettings.dspTime < _deafUntil ? new Color(0.9f, 0.1f, 0.1f, 0.4f) : new Color(0.1f, 0.9f, 0.1f, 0.5f);
        SensitivityBar.rectTransform.sizeDelta = new Vector2(SensitivityBar.rectTransform.sizeDelta.x, _currentNoteSensetivity);

        if(triggeredBars > BarsToAddNote && BarsToAddNote > 0 && Player)
        {
            _currentNoteSensetivity += lossRate;
            var note = new Note();
            note.Time = (float)Player.AudioSourceDspTime;
            Player.Stem.InstrumentData[0].Notes.AddNote(note);
            _deafUntil = AudioSettings.dspTime + Deaf / 4.0f;
            Debug.Log("Note");
            SensitivityBar.color = Color.white;
        }
    }
}
