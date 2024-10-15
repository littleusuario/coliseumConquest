using System;

namespace MyGame
{
    public class AudienceBounce
    {
        private IntPtr leftAudience = Engine.LoadImage("assets/screens/leftAudience.png");
        private IntPtr rightAudience = Engine.LoadImage("assets/screens/rightAudience.png");

        private int leftX;
        private int rightX;

        private float initialY;
        private float spriteY;
        private float velocityY;

        private float gravity;
        private float groundHeight;

        private bool isRising;

        private float jumpSpeed = 210f;

        public AudienceBounce(int leftX, int rightX, float groundHeight)
        {
            this.leftX = leftX;
            this.rightX = rightX;
            this.initialY = 145;
            spriteY = initialY;
            velocityY = 0f;
            gravity = 1200f;
            this.groundHeight = groundHeight - 20;
            isRising = false;
        }

        public void Update()
        {
            if (!isRising)
            {
                velocityY += gravity * Time.DeltaTime;
            }

            spriteY += velocityY * Time.DeltaTime;

            if (spriteY >= groundHeight)
            {
                spriteY = groundHeight;

                velocityY = -velocityY * 0.6f;

                if (Math.Abs(velocityY) < 50)
                {
                    velocityY = 0;
                }

                if (CameraShake.Instance.value > 3.5 && !CameraShake.Instance.ThunderShake && GameManager.Instance.LevelController.player.Health >= 1
                    || !GameManager.Instance.LevelController.player.Vulnerable)
                {
                    velocityY = -jumpSpeed;
                    isRising = true;
                }
            }
            else
            {
                isRising = false;
            }
        }

        public void Render()
        {
            Engine.Draw(leftAudience, leftX + CameraShake.Instance.value, spriteY);
            Engine.Draw(rightAudience, rightX + CameraShake.Instance.value, spriteY - 20);
        }

        public void Reset()
        {
            spriteY = initialY;
            velocityY = 0f;
            isRising = false;
        }
    }
}
