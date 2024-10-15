using System;
using System.Diagnostics;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        static float delayFrame = 60f;             // FPS
        public static bool targetFrame = false;

        static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            if (SdlMixer.Mix_OpenAudio(48000, (short)Sdl.AUDIO_S16SYS, 2, 4096) == -1)
            {
                //Engine.Debug("No se puede reproducir el audio.");
            }
            if (SdlMixer.Mix_AllocateChannels(20) < 20)
            {
                //Engine.Debug("No se puede reproducir el audio, no hay suficientes canales.");
            }

            Engine.Initialize(1280, 720);
            GameManager.Instance.Initialize();
            Time.Initialize();
            stopwatch.Start();

            int frameCount = 0;
            float elapsedTime = 0f;

            while (Engine.running)
            {
                Engine.HandleEvents();

                float startTime = (float)stopwatch.Elapsed.TotalSeconds;
                float targetFrameTime = 1f / delayFrame;

                GameManager.Instance.Update();
                GameManager.Instance.Render();

                frameCount++;
                elapsedTime += Time.DeltaTime;

                if (elapsedTime >= 1.0f)
                {
                    AdjustDelayFrame(frameCount);
                    frameCount = 0;
                    elapsedTime = 0f;
                }

                float endTime = (float)stopwatch.Elapsed.TotalSeconds;
                float frameTime = endTime - startTime;
                float delayTime = targetFrameTime - frameTime;

                if (delayTime > 0)
                {
                    int delayMilliseconds = (int)(delayTime * 1000);
                    Sdl.SDL_Delay(delayMilliseconds);
                }
            }
        }

        private static void AdjustDelayFrame(int frameCount)
        {
            if (!targetFrame)
            {
                float diff = 60 - frameCount;
                float factor = Math.Sign(diff) * Math.Min(10, Math.Abs(diff) / 10f);
                delayFrame += factor;
            }
        }
    }
}
