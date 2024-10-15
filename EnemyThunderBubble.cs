using System;
using System.Collections.Generic;
namespace MyGame
{
    public class EnemyThunderBubble : Projectile, ICheckForCollision
    {
        public const float bulletHeight = 48;
        public const float bulletWidth = 48;

        private readonly int groundHeight = GameManager.Instance.LevelController.groundHeight;
        private readonly int screenWidth = GameManager.Instance.LevelController.screenWidth;

        private string idlePath = "assets/enemyBullet/thunder.png";

        private bool playerDetected;
        private bool shootThunder;

        private float initialDetection = 0.375f;
        private float timeSinceSpawn;
        private float detectionCoolDown;
        private float timeSinceDetection;

        private float waveFrequency = 6f;
        private float waveAmplitude = 3f;
        private float waveTime = 0;

        private Animation spawn;
        private Animation wait;

        private Sound lightningShootSound;

        public EnemyThunderBubble(Vector2 position, Vector2 playerPosition, Vector2 offset)
        {
            Vector2 adjustedPosition = new Vector2(position.x + offset.x, position.y + offset.y);
            transform = new Transform(adjustedPosition);
            direction = new Vector2(playerPosition.x + (Character.PlayerWidth / 2) - adjustedPosition.x, 0);
            float length = (float)Math.Sqrt(direction.x * direction.x + direction.y * direction.y);
            direction.x /= length;
            timeSinceDetection = 0;

            if (GameManager.Instance.HardMode)
            {
                acceleration = 250;
                bulletVel = 10;
                detectionCoolDown = 0.07f;
            }
            else
            {
                acceleration = 100;
                bulletVel = 150;
                detectionCoolDown = 0.13f;
            }

            CreateAnimations();
            lightningShootSound = new Sound("thunderShoot.wav");
        }
        public override void Update()
        {
            timeSinceSpawn += Time.DeltaTime;
            if (playerDetected)
            {
                if (!shootThunder)
                {
                    currentAnimation = wait;
                    currentAnimation.Update();
                    timeSinceDetection += Time.DeltaTime;
                }
            }
            else
            {
                currentAnimation = spawn;
                currentAnimation.Update();
                bulletVel += acceleration * Time.DeltaTime;
                transform.Translate(direction, bulletVel * Time.DeltaTime);
                waveTime += Time.DeltaTime;
                float waveOffset = (float)Math.Sin(waveTime * waveFrequency) * waveAmplitude;
                transform.Position = new Vector2(transform.Position.x, transform.Position.y + waveOffset);
            }
        }
        public void CheckPositions(Character player)
        {
            Vector2 playerPosition = player.Transform.Position;
            Vector2 bulletPosition = transform.Position;
            if (bulletPosition.x + bulletWidth > playerPosition.x && bulletPosition.x < playerPosition.x + Character.PlayerWidth &&
                bulletPosition.y + bulletHeight > playerPosition.y && bulletPosition.y < playerPosition.y + Character.PlayerHeight &&
                !shootThunder && timeSinceSpawn > initialDetection && player.Vulnerable)
            {
                if (player is IDamageable)
                {
                    player.TakeDamage(1);
                    playerDetected = true;
                    timeSinceDetection = 0;
                }
                return;
            }
            else if (Math.Abs(playerPosition.x - bulletPosition.x) <= Character.PlayerWidth / 2 && !playerDetected && timeSinceSpawn > initialDetection)
            {
                playerDetected = true;
                timeSinceDetection = 0;
            }
            else if (transform.Position.y >= groundHeight - bulletHeight || transform.Position.x >= screenWidth || transform.Position.x <= 0 - bulletWidth) // Fuera de escena
            {
                destroyed = true;
                bulletVel = 0;
                acceleration = 0;
                GameManager.Instance.LevelController.ThunderList.Remove(this);
            }

            if (timeSinceDetection >= detectionCoolDown)
            {
                bulletVel = 0;
                acceleration = 0;
                if (!shootThunder)
                {
                    lightningShootSound.PlayOnce(GameManager.Instance.audioMixer.LightningShootChannel);
                    shootThunder = true;

                    Vector2 lightningOffset = new Vector2(0, -20);
                    GameManager.Instance.LevelController.LightningList.Add(new EnemyLightningBolt(transform.Position, lightningOffset));
                    GameManager.Instance.LevelController.ThunderList.Remove(this);
                }
            }
        }
        private void CreateAnimations()
        {
            List<string> destroyPaths = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                destroyPaths.Add($"assets/enemyBullet/destroy/{i}.png");
            }
            List<IntPtr> spawn = new List<IntPtr>();
            for (int i = 0; i < 5; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/enemyBullet/thunderSpawn/{i}.png");
                spawn.Add(frame);
            }
            this.spawn = new Animation("Spawn", spawn, 0.075f, false);
            List<IntPtr> wait = new List<IntPtr>();
            for (int i = 0; i < 2; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/enemyBullet/thunderWait/{i}.png");
                wait.Add(frame);
            }
            this.wait = new Animation("Wait", wait, 0.05f, true);
            CreateAnimations(idlePath, destroyPaths, 0.1f);
        }
    }
}
