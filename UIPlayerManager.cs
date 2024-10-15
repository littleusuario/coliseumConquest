using System;

namespace MyGame
{
    internal class UIPlayerManager
    {
        static IntPtr hpSprite = Engine.LoadImage("assets/HUD/player/hp.png");
        static IntPtr hpEmptySprite = Engine.LoadImage("assets/HUD/player/hpEmpty.png");
        static IntPtr jump = Engine.LoadImage("assets/HUD/player/jump.png");
        static IntPtr jumpBar = Engine.LoadImage("assets/HUD/player/jumpBar.png");

        static private int positionX = 41;
        static private int positionY = 37;

        public static void RenderPlayerUI(Character player)
        {
            if (GameManager.Instance.HardMode)
            {
                for (int i = 0; i < player.MaxHealthHard; i++)
                {
                    IntPtr sprite = (i < player.Health) ? hpSprite : hpEmptySprite;
                    Engine.Draw(sprite, positionX + i * (34 + 11), positionY);
                }
            }
            else
            {
                for (int i = 0; i < player.MaxHealthNormal; i++)
                {
                    IntPtr sprite = (i < player.Health) ? hpSprite : hpEmptySprite;
                    Engine.Draw(sprite, positionX + i * (34 + 11), positionY);
                }
            }

            int jumpsLeft = player.MaxJumps - player.controller.JumpCounter;

            for (int i = 0; i < Math.Min(jumpsLeft, player.MaxJumps); i++)
            {
                Engine.Draw(jump, positionX + i * 62, positionY + 45);
            }

            Engine.Draw(jumpBar, positionX, positionY + 45);
        }
    }
}