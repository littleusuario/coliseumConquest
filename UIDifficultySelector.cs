using System;

namespace MyGame
{
    public class UIDifficultySelector
    {
        private bool xKeyReleased = true;
        private IntPtr normalImage;
        private IntPtr hardImage;
        private Sound difficulty;

        public UIDifficultySelector()
        {
            normalImage = Engine.LoadImage("assets/mainMenu/normal.png");
            hardImage = Engine.LoadImage("assets/mainMenu/hard.png");
            difficulty = new Sound("difficulty.wav");
        }

        public void Update()
        {
            if (Engine.KeyPress(Engine.KEY_X) && xKeyReleased)
            {
                xKeyReleased = false;
                CycleDifficulty();
            }
            else if (!Engine.KeyPress(Engine.KEY_X))
            {
                xKeyReleased = true;
            }
        }

        private void CycleDifficulty()
        {
            GameManager.Instance.HardMode = !GameManager.Instance.HardMode;
            difficulty.PlayOnce(GameManager.Instance.audioMixer.UIDifficulty);
        }

        public void Render()
        {
            if (GameManager.Instance.HardMode)
            {
                Engine.Draw(hardImage, 1280 - 220, 10);
            }
            else
            {
                Engine.Draw(normalImage, 1280 - 220, 10);
            }
        }
    }
}
