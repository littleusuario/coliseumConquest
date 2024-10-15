using MyGame;
using System;

public class WaveMovement
{
    private float waveFrequency = 6f;
    private float waveAmplitude = 3f;
    private float waveTime = 0;

    public float GetWaveOffset()
    {
        waveTime += Time.DeltaTime;
        return (float)Math.Sin(waveTime * waveFrequency) * waveAmplitude;
    }
}