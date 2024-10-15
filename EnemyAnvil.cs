using System;
using System.Collections.Generic;

namespace MyGame
{
    public class EnemyAnvil : Projectile, ICheckForCollision
    {
        private readonly int groundHeight = GameManager.Instance.LevelController.groundHeight;
        private string idlePath = "assets/enemyBullet/anvil.png";

        private bool bounced;
        private float shakeSize = 3f;
        private float anvilShakeTime = 0f;
        private float anvilShakeSpeed = 55f;

        public const float anvilWidth = 171;
        public const float anvilHeight = 25;
        public const float smallAnvilWidth = 82;
        public const float smallAnvilHeight = 27;

        private Sound anvilHit;
        private Sound anvilFall;

        public EnemyAnvil(Vector2 position, Vector2 offset) : base(new Vector2(position.x + offset.x, position.y + offset.y), new Vector2(0, 1))
        {
            bulletVel = 0;

            if (GameManager.Instance.HardMode)
            {
                acceleration = 1500;
            }
            else
            {
                acceleration = 1250;
            }

            coolDown = 0.4f;
            CreateAnimations();

            anvilHit = new Sound("anvilHit.wav");
            anvilFall = new Sound("anvilFall.wav");
            currentAnimation = idle;
        }

        public override void Update()
        {
            if (bulletVel == 0 && !bounced && !GameManager.Instance.HardMode)
            {
                anvilFall.PlayOnce(GameManager.Instance.audioMixer.AnvilFallChannel);
            }

            bulletVel += acceleration * Time.DeltaTime;
            transform.Translate(direction, bulletVel * Time.DeltaTime);

            if (transform.Position.y >= groundHeight - 92 && !bounced)
            {
                anvilHit.PlayOnce(GameManager.Instance.audioMixer.AnvilHitChannel - 1);

                CameraShake.Instance.EnemyHugeShake();
                bounced = true;
                shakeSize = 6.5f;
                anvilShakeSpeed = 60f;
                bulletVel = -bulletVel * 0.5f;
            }
        }

        public override void Render()
        {
            if (!GameManager.Instance.Paused)
            {
                anvilShakeTime += Time.DeltaTime;
            }

            float offsetX = shakeSize * (float)Math.Sin(anvilShakeSpeed * anvilShakeTime);
            Engine.Draw(currentAnimation.CurrentFrame, (transform.Position.x + offsetX) + CameraShake.Instance.value, transform.Position.y);
        }

        public void CheckPositions(Character player)
        {
            Vector2 playerPosition = player.Transform.Position;
            Vector2 bulletPosition = transform.Position;

            if (bulletPosition.x + anvilWidth > playerPosition.x && bulletPosition.x + 4 < playerPosition.x + Character.PlayerWidth &&
                bulletPosition.y + anvilHeight > playerPosition.y && bulletPosition.y < playerPosition.y + Character.PlayerHeight && player.Vulnerable)
            {
                if (player is IDamageable)
                    player.TakeDamage(1);
                return;
            }

            float smallHitboxX = bulletPosition.x + 64;
            float smallHitboxY = bulletPosition.y + anvilHeight + 58;

            if (smallHitboxX + smallAnvilWidth > playerPosition.x && smallHitboxX < playerPosition.x + Character.PlayerWidth &&
                smallHitboxY + smallAnvilHeight > playerPosition.y && smallHitboxY < playerPosition.y + Character.PlayerHeight && player.Vulnerable)
            {
                if (player is IDamageable)
                    player.TakeDamage(1);
                return;
            }

            if (transform.Position.y >= 720)
            {
                destroyed = true;
                GameManager.Instance.LevelController.AnvilList.Remove(this);
            }
        }

        private void CreateAnimations()
        {
            List<string> destroyPaths = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                destroyPaths.Add($"assets/enemyBullet/thunderBolt/destroy/{i}.png");
            }
            CreateAnimations(idlePath, destroyPaths, 0.05f);
        }
    }
}
