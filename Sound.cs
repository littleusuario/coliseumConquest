using System;
using Tao.Sdl;

public class Sound
{
    IntPtr pointer;
    int channel;

    public Sound(string fileName)
    {
        string fullPath = $"assets/audio/{fileName}";
        pointer = SdlMixer.Mix_LoadWAV(fullPath);
        if (pointer == IntPtr.Zero)
        {
            Console.WriteLine("No se puede cargar el archivo de sonido: " + fullPath);
        }
    }

    public void PlayOnce(int channel = -1)
    {
        if (channel != -1)
        {
            this.channel = channel;
        }
        SdlMixer.Mix_PlayChannel(this.channel, pointer, 0);
    }

    public void Play(int channel = -1)
    {
        if (channel != -1)
        {
            this.channel = channel;
        }
        SdlMixer.Mix_PlayChannel(this.channel, pointer, -1);
    }

    public void Stop()
    {
        SdlMixer.Mix_HaltChannel(channel);
    }

    public bool IsPlaying()
    {
        return SdlMixer.Mix_Playing(channel) == 1;
    }

    public void SetPanning(byte left, byte right)
    {
        if (SdlMixer.Mix_SetPanning(this.channel, left, right) == 0)
        {
            Console.WriteLine("No se pudo establecer el panning para el canal: " + this.channel);
        }
    }
}
