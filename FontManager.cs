using System;

namespace MyGame
{
    internal class FontManager
    {
        static IntPtr gameFont = Engine.LoadFont("assets/Cave-Story.ttf", 30);
        static string fullText = "Shiori Hisoka, Eminencia del Torneo";
        static string displayedText = fullText[0].ToString();
        static float letterCooldown;
        static float coolDownDuration = 0.03f;

        public static void Render(Enemy enemy, Vector2 position)
        {
            Engine.DrawText(displayedText, GameManager.Instance.LevelController.screenWidth / 2 - (int)position.x + (int)enemy.ShakeOffsetX, (int)position.y, 255, 255, 255, gameFont);

            if (displayedText.Length < fullText.Length && !GameManager.Instance.Paused)
            {
                letterCooldown -= Time.DeltaTime;
                if (letterCooldown <= 0)
                {
                    letterCooldown = coolDownDuration;
                    displayedText += fullText[displayedText.Length];
                }
            }
        }

        public static void ResetText()
        {
            displayedText = fullText[0].ToString();
            letterCooldown = coolDownDuration;
        }
    }
}
