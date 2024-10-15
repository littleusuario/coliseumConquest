using System;
using System.Collections.Generic;

namespace MyGame
{
    public enum ButtonType
    {
        Start,
        Tutorial,
        Exit
    }
    public enum ButtonState
    {
        On,
        Off
    }

    public class ButtonFactory
    {
        private static readonly Dictionary<ButtonType, string> baseSpritePaths;
        private static readonly Dictionary<ButtonType, Dictionary<ButtonState, string>> spritePaths;

        static ButtonFactory()
        {
            baseSpritePaths = new Dictionary<ButtonType, string>
            {
                { ButtonType.Start, "assets/mainMenu/start_" },
                { ButtonType.Tutorial, "assets/mainMenu/tutorial_" },
                { ButtonType.Exit, "assets/mainMenu/exit_" }
            };

            spritePaths = new Dictionary<ButtonType, Dictionary<ButtonState, string>>();
            foreach (ButtonType type in Enum.GetValues(typeof(ButtonType)))
            {
                var paths = new Dictionary<ButtonState, string>
                {
                    { ButtonState.On, baseSpritePaths[type] + "on.png" },
                    { ButtonState.Off, baseSpritePaths[type] + "off.png" }
                };
                spritePaths.Add(type, paths);
            }
        }

        public Button CreateButton(ButtonType type, Vector2 position)
        {
            string imagePath = spritePaths[type][ButtonState.On];
            return new Button(position, imagePath);
        }

        public Button CreateButton(ButtonType type, ButtonState state, Vector2 position)
        {
            string imagePath = spritePaths[type][state];
            return new Button(position, imagePath);
        }
    }
}