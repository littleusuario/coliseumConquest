using System;
namespace MyGame
{
    public class StaticImage
    {
        static IntPtr image = Engine.LoadImage("assets/screens/background.png");

        public void RenderBG()
        {
            Engine.Draw(image, CameraShake.Instance.value, 0);
        }
    }
}