using System;

namespace MyGame
{
    internal class UIEnemyHealthBar
    {
        static IntPtr bossBar = Engine.LoadImage("assets/HUD/bossBar.png");
        static IntPtr skull = Engine.LoadImage("assets/HUD/skull.png");
        static IntPtr skullPhase2 = Engine.LoadImage("assets/HUD/skullPhase2.png");
        static IntPtr skullDying = Engine.LoadImage("assets/HUD/skullDying.png");
        static IntPtr hp = Engine.LoadImage("assets/HUD/hp.png");
        static IntPtr hpHit = Engine.LoadImage("assets/HUD/hpHit.png");

        static private int positionX = 424;
        static private int positionY = 650;

        public static void RenderEnemyHP(Enemy enemy)
        {
            float hpBarCount = enemy.Health * 4.56f - 1;
            int normalHpCount = (int)hpBarCount - 4;

            for (int i = 0; i < normalHpCount; i++)
            {
                Engine.Draw(hp, positionX + 3 + i + ((int)enemy.ShakeOffsetX * 2), positionY + 1);
            }

            for (int i = normalHpCount; i < hpBarCount; i++)
            {
                if (enemy.IsShaking)
                {
                    Engine.Draw(hpHit, positionX + 3 + i + ((int)enemy.ShakeOffsetX * 2), positionY + 1);
                }
                else
                {
                    Engine.Draw(hp, positionX + 3 + i + ((int)enemy.ShakeOffsetX * 2), positionY + 1);
                }
            }

            Engine.Draw(bossBar, positionX + ((int)enemy.ShakeOffsetX * 2), positionY);

            if (enemy.Health >= 25 && enemy.Health < 75 && GameManager.Instance.HardMode || enemy.Health >= 10 && enemy.Health < 50)
            {
                Engine.Draw(skullPhase2, positionX - 23 + enemy.ShakeOffsetX, positionY - 6);
            }
            else if (enemy.Health >= 50 || !GameManager.Instance.HardMode)
            {
                Engine.Draw(skull, positionX - 23 + enemy.ShakeOffsetX, positionY - 6);
            }
            if (enemy.Health <= 25)
            {
                Engine.Draw(skullDying, positionX - 23 + enemy.ShakeOffsetX, positionY - 6);
            }

            FontManager.Render(enemy, new Vector2(((positionX - 75) / 2), positionY - 24));
        }
    }
}
