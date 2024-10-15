using System;
using System.Collections.Generic;

namespace MyGame
{
    public class EnemyBullet : Projectile, ICheckForCollision
    {
        private readonly int groundHeight = GameManager.Instance.LevelController.groundHeight;
        private readonly int screenWidth = GameManager.Instance.LevelController.screenWidth;

        public const float bulletHeight = 24;
        public const float bulletWidth = 24;

        private readonly string idlePath = "assets/enemyBullet/bullet.png";
        private float timeSinceSpawn;

        private Animation spawn;
        private bool directionSet = false;

        private bool soundPlayed;

        private readonly Sound bubblePop;

        public EnemyBullet(Vector2 position, Vector2 offset) : base(new Vector2(position.x + offset.x, position.y + offset.y), new Vector2(0, 0))
        {
            bulletVel = GameManager.Instance.HardMode ? 30 : 10;
            acceleration = GameManager.Instance.HardMode ? 2500 : 2000;
            coolDown = GameManager.Instance.HardMode ? 0.3f : 0.4f;
            timeSinceSpawn = 0f;
            bubblePop = new Sound("bubblePop.wav");

            CreateAnimations();
            currentAnimation = spawn;
        }

        private Vector2 CalculateDirection(Vector2 position, Vector2 playerPosition, Vector2 offset)
        {
            Vector2 adjustedPosition = new Vector2(position.x + offset.x, position.y + offset.y);
            Vector2 direction = new Vector2(playerPosition.x + (Character.PlayerWidth / 2) - adjustedPosition.x, playerPosition.y + (Character.PlayerHeight / 2) - adjustedPosition.y);
            float length = (float)Math.Sqrt(direction.x * direction.x + direction.y * direction.y);
            direction.x /= length;
            direction.y /= length;
            return direction;
        }

        public override void Update()
        {
            timeSinceSpawn += Time.DeltaTime;

            if (timeSinceSpawn >= coolDown)
            {
                if (!directionSet)
                {
                    Vector2 playerPosition = GameManager.Instance.LevelController.player.Transform.Position;

                    direction = CalculateDirection(transform.Position, playerPosition, new Vector2(0, 0));
                    directionSet = true;
                }

                bulletVel += acceleration * Time.DeltaTime;
                transform.Translate(direction, bulletVel * Time.DeltaTime);
            }
            else
            {
                currentAnimation.Update();
            }

            base.Update();
        }

        public void CheckPositions(Character player)
        {
            float bulletRight = transform.Position.x + bulletWidth;
            float bulletBottom = transform.Position.y + bulletHeight;

            float playerLeft = player.Transform.Position.x;
            float playerRight = player.Transform.Position.x + Character.PlayerWidth;
            float playerTop = player.Transform.Position.y;
            float playerBottom = player.Transform.Position.y + Character.PlayerHeight;

            if (bulletRight >= playerLeft && transform.Position.x <= playerRight && bulletBottom >= playerTop && transform.Position.y <= playerBottom && !destroyed)
            {
                HandleCollision(player);
            }

            if (transform.Position.y >= groundHeight - bulletHeight || transform.Position.x >= screenWidth || transform.Position.x <= 0 - bulletWidth)
            {
                HandleOutOfBounds();
            }
        }

        private void HandleCollision(Character player)
        {
            if (!soundPlayed)
            {
                bubblePop.PlayOnce(GameManager.Instance.audioMixer.BubblePopChannel);
                soundPlayed = true;
            }

            if (player.Vulnerable && player is IDamageable)
                player.TakeDamage(1);

            DestroyBullet();
        }

        private void HandleOutOfBounds()
        {
            if (transform.Position.x <= screenWidth && transform.Position.x >= 0 - bulletWidth)
            {
                if (!soundPlayed)
                {
                    bubblePop.PlayOnce(GameManager.Instance.audioMixer.BubblePopChannel);
                    soundPlayed = true;
                }
            }

            DestroyBullet();
        }

        private void DestroyBullet()
        {
            destroyed = true;
            currentAnimation = destroy;
            currentAnimation?.Update();
            bulletVel = 0;
            acceleration = 0;

            coolDown -= Time.DeltaTime;
            if (coolDown <= 0)
                GameManager.Instance.LevelController.EnemyBulletList.Remove(this);
        }

        private void CreateAnimations()
        {
            List<IntPtr> spawnFrames = new List<IntPtr>();
            for (int i = 0; i < 4; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/enemyBullet/spawn/{i}.png");
                spawnFrames.Add(frame);
            }
            spawn = new Animation("Spawn", spawnFrames, 0.1f, false);

            List<string> destroyPaths = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                destroyPaths.Add($"assets/enemyBullet/destroy/{i}.png");
            }
            CreateAnimations(idlePath, destroyPaths, 0.1f);
        }
    }
}
