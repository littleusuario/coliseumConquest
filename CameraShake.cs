using MyGame;
using System;

public class CameraShake
{
    private static CameraShake instance;

    private bool isShaking = false;
    private bool thunderShake = false;
    private float shakeOffset = 0;
    private float shakeTimer = 0;
    private float shakeDuration = 0.15f;
    private float shakeMagnitude = 2;
    private float shakeFrequency = 10;

    public static CameraShake Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CameraShake();
            }
            return instance;
        }
    }

    public float value { set { shakeOffset = value; } get { return shakeOffset; } }
    public bool ThunderShake { set { thunderShake = value; } get { return thunderShake; } }

    public void Shake(float duration, float magnitude, float frequency)
    {
        isShaking = true;
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeFrequency = frequency;
        shakeTimer = 0;
    }

    public void HitShake()
    {
        Shake(1f, 5f, 8f);
    }

    public void EnemyHugeShake()
    {
        Shake(2f, 5f, 5f);
    }

    public void EnemySmallShake()
    {
        Shake(0.3f, -10f, -3f);
        thunderShake = true;
    }

    public void Update()
    {
        if (isShaking)
        {
            shakeTimer += Time.DeltaTime;

            if (shakeTimer <= shakeDuration)
            {
                float progress = shakeTimer / shakeDuration;
                float currentMagnitude = shakeMagnitude * (1 - progress);
                shakeOffset = currentMagnitude * (float)Math.Sin(2 * Math.PI * shakeTimer * shakeFrequency);
            }
            else
            {
                thunderShake = false;
                isShaking = false;
                shakeTimer = 0;
                shakeOffset = 0;
            }
        }
    }
}
