using System;

namespace MyGame
{
    public class UIMainMenu
    {
        private ButtonFactory buttonFactory;
        private Button[] buttons;
        private int selectedItem = 0;
        private bool arrowKeyReleased = true;

        private IntPtr mainMenuScreen = Engine.LoadImage("assets/mainMenu/MainMenu.png");
        private IntPtr cursorImage = Engine.LoadImage("assets/mainMenu/cursor.png");
        private IntPtr crownImage = Engine.LoadImage("assets/mainMenu/crown.png");
        private IntPtr goldCrownImage = Engine.LoadImage("assets/mainMenu/crownHard.png");

        private int[] cursorXPositions = { 565, 425, 565 };
        private int[] cursorYPositions = { 345, 480, 610 };
        private float cursorBaseXPosition;
        private float cursorXPosition;
        private float cursorTime = 0f;

        private float cursorYPosition;
        private float cursorTargetYPosition;
        private float cursorTargetXPosition;
        private const float lerpSpeed = 25f;

        private float orbXPosition;
        private float orbYPosition;
        private float orbTime = 0f;
        private const float orbCenterX = 950f;
        private const float orbCenterY = 65f;

        private int[] menuXPositions = { 202, 62, 202 };
        private int[] menuYPositions = { 303, 438, 572 };

        private UIDifficultySelector difficultySelector;

        public delegate void MenuAction();
        public event MenuAction OnStartGame;
        public event MenuAction OnTutorial;
        public event MenuAction OnExit;

        private Sound moveSound;

        public UIMainMenu()
        {
            buttonFactory = new ButtonFactory();
            buttons = new Button[]
            {
                buttonFactory.CreateButton(ButtonType.Start, ButtonState.Off, new Vector2(cursorXPositions[0], cursorYPositions[0])),
                buttonFactory.CreateButton(ButtonType.Tutorial, ButtonState.Off, new Vector2(cursorXPositions[1], cursorYPositions[1])),
                buttonFactory.CreateButton(ButtonType.Exit, ButtonState.Off, new Vector2(cursorXPositions[2], cursorYPositions[2]))
            };
            cursorBaseXPosition = cursorXPositions[selectedItem];
            cursorXPosition = cursorBaseXPosition;
            cursorYPosition = cursorYPositions[selectedItem];
            cursorTargetYPosition = cursorYPosition;
            cursorTargetXPosition = cursorBaseXPosition;

            difficultySelector = new UIDifficultySelector();

            moveSound = new Sound("moveUI.wav");
        }

        public void Update()
        {
            if (!Engine.KeyPress(Engine.KEY_UP) && !Engine.KeyPress(Engine.KEY_DOWN))
            {
                arrowKeyReleased = true;
            }
            if (Engine.KeyPress(Engine.KEY_UP) && arrowKeyReleased)
            {
                arrowKeyReleased = false;
                selectedItem = (selectedItem - 1 + buttons.Length) % buttons.Length;
                cursorTargetYPosition = cursorYPositions[selectedItem];
                cursorTargetXPosition = cursorXPositions[selectedItem];
                cursorTime = 0f;

                moveSound.PlayOnce(GameManager.Instance.audioMixer.UIChannel);
            }
            if (Engine.KeyPress(Engine.KEY_DOWN) && arrowKeyReleased)
            {
                arrowKeyReleased = false;
                selectedItem = (selectedItem + 1) % buttons.Length;
                cursorTargetYPosition = cursorYPositions[selectedItem];
                cursorTargetXPosition = cursorXPositions[selectedItem];
                cursorTime = 0f;

                moveSound.PlayOnce(GameManager.Instance.audioMixer.UIChannel);
            }
            if (Engine.KeyPress(Engine.KEY_Z) && GameManager.Instance.ZKeyReleased)
            {
                GameManager.Instance.ZKeyReleased = false;
                ExecuteItem();
            }
            cursorTime += Time.DeltaTime;
            cursorXPosition = Lerp(cursorXPosition, cursorTargetXPosition + 10f * (float)Math.Sin(5f * cursorTime), lerpSpeed * Time.DeltaTime);
            cursorYPosition = Lerp(cursorYPosition, cursorTargetYPosition, lerpSpeed * Time.DeltaTime);

            orbTime += Time.DeltaTime * 1.5f;
            orbXPosition = orbCenterX + 20f * (float)Math.Cos(orbTime);
            orbYPosition = orbCenterY + 20f * (float)Math.Sin(orbTime);

            difficultySelector.Update();
        }

        private float Lerp(float start, float end, float t)
        {
            return start + t * (end - start);
        }

        private void ExecuteItem()
        {
            switch (selectedItem)
            {
                case 0:
                    OnStartGame?.Invoke();
                    GameManager.Instance.LevelController.Restart();
                    break;
                case 1:
                    OnTutorial?.Invoke();
                    break;
                case 2:
                    OnExit?.Invoke();
                    break;
            }
        }

        public void Render()
        {
            Engine.Clear();
            Engine.Draw(mainMenuScreen, 0, 0);

            for (int i = 0; i < buttons.Length; i++)
            {
                ButtonState state = (i == selectedItem) ? ButtonState.On : ButtonState.Off;
                buttons[i] = buttonFactory.CreateButton((ButtonType)i, state, new Vector2(menuXPositions[i], menuYPositions[i]));
                buttons[i].Render();
            }

            difficultySelector.Render();

            Engine.Draw(cursorImage, (int)cursorXPosition, (int)cursorYPosition);
            if (GameManager.Instance.HardModeWon)
            {
                Engine.Draw(goldCrownImage, (int)orbXPosition, (int)orbYPosition);
            }
            else
            {
                Engine.Draw(crownImage, (int)orbXPosition, (int)orbYPosition);
            }
            Engine.Show();
        }
    }
}
