using UnityEngine;

public partial class AudioVisualizer
{
    private Texture2D GenerateWaveformSegment(float startTime, ref float startOffsetInSeconds, AudioClip audioClip)
    {

        wfcolorindex = (wfcolorindex + 1) % waveformColors.Length;

        // Define the length of the audio segment in seconds that each texture will represent
        float segmentLength = textureWidth / (float)pixelsPerSecond;
        int sampleSize = (int)(segmentLength * audioClip.frequency);

        // Calculate the start sample index based on the startTime and the audio clip's frequency
        int startSample = (int)(startTime * audioClip.frequency);

        // Get the audio clip data
        float[] samples = new float[sampleSize * audioClip.channels];
        audioClip.GetData(samples, startSample);

        // Create a new texture with the specified width and the height of the panel
        Texture2D texture = new Texture2D(textureWidth, (int)panel.rect.height);

        // Loop through each pixel in the width
        for (int x = 0; x < textureWidth; x++)
        {
            // Calculate the sample index for this x position
            int sampleIndex = (int)(x * (float)sampleSize / textureWidth) * audioClip.channels;

            // Find the maximum amplitude in this sample segment
            float maxAmplitude = 0f;
            for (int i = 0; i < audioClip.channels; i++)
            {
                maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(samples[sampleIndex + i]));
            }

            // Draw the waveform
            int yCenter = texture.height / 2;
            int yStart = yCenter - (int)(maxAmplitude * heightMultiplier);
            int yEnd = yCenter + (int)(maxAmplitude * heightMultiplier);
            yStart = Mathf.Clamp(yStart, 0, texture.height);
            yEnd = Mathf.Clamp(yEnd, 0, texture.height);

            for (int y = yStart; y <= yEnd; y++)
            {
                texture.SetPixel(x, y, waveformColors[wfcolorindex]);
            }
        }

        float timing = startOffsetInSeconds;
        for (timing = startOffsetInSeconds; timing < startTime + segmentLength; timing += beatDuration)
        {
            int x = Mathf.FloorToInt((timing - startTime) * pixelsPerSecond);
            if (x >= 0 && x < textureWidth)
            {
                DrawBar(texture, x, texture.height);
            }
        }

        startOffsetInSeconds = timing;
        texture.Apply();

        return texture;
    }

    void DrawBar(Texture2D texture, int x, int height)
    {
        for (int y = 0; y < height; y++)
        {
            texture.SetPixel(x, y, barColor);
        }
    }
}
