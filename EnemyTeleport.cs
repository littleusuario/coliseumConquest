using System;

namespace MyGame
{
    public class EnemyTeleport
    {
        public const int TeleportHeight = 20;
        public const int TeleportWidth = 20;

        private float imageVel = -10;
        private float acceleration = 1000;

        private float destructionTimer = 0.40f;
        private float coolDown = 0.3f;
        private bool isDestroyed = false;

        private Transform transform;
        private IntPtr image;
        private Vector2 direction;

        public EnemyTeleport(int x, int y, Vector2 dir, string imagePath)
        {
            transform = new Transform(new Vector2(x, y));
            direction = dir;
            image = Engine.LoadImage(imagePath);
        }

        public EnemyTeleport(Vector2 position, Vector2 dir, string imagePath)
        {
            transform = new Transform(position);
            direction = dir;
            image = Engine.LoadImage(imagePath);
        }

        public void Render()
        {
            if (!isDestroyed)
                Engine.Draw(image, (int)transform.Position.x + CameraShake.Instance.value, (int)transform.Position.y);
        }

        public void Update()
        {
            if (!isDestroyed)
            {
                coolDown -= Time.DeltaTime;
                if (coolDown <= 0)
                {
                    imageVel += acceleration * Time.DeltaTime;
                    transform.Translate(new Vector2(direction.x * imageVel * Time.DeltaTime, direction.y * imageVel * Time.DeltaTime));

                    destructionTimer -= Time.DeltaTime;
                    if (destructionTimer <= 0)
                    {
                        Destroy();
                    }
                }
            }
        }

        private void Destroy()
        {
            GameManager.Instance.LevelController.TeleportList.Remove(this);
            isDestroyed = true;
        }
    }
}
