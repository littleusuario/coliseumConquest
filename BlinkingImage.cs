using System;

namespace MyGame
{
    public static class BlinkingImage
    {
        private static float timer;
        private static bool isVisible = true;
        private static IntPtr pressZ = Engine.LoadImage("assets/screens/pressZ.png");

        public static void Update(float blinkInterval)
        {
            timer += Time.DeltaTime;

            if (timer >= blinkInterval)
            {
                isVisible = !isVisible;
                timer = 0f;
            }
        }

        public static void ShowBlinkingText(int x, int y)
        {
            Update(0.75f);

            if (isVisible)
            {
                Engine.Draw(pressZ, x, y);
            }
        }
    }
}
