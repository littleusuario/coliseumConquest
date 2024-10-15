using System.Collections.Generic;

namespace MyGame
{
    public class LevelController
    {
        public int groundHeight = 574;        // De arriba a abajo
        public int screenWidth = 1280;       // De izquierda a derecha

        public Character player;
        public Enemy enemy;
        public StaticImage staticImage;
        public AudienceBounce audienceBounce;

        public List<Bullet> BulletList;
        public List<EnemyTeleport> TeleportList;
        public List<EnemyBullet> EnemyBulletList;
        public List<EnemyThunderBubble> ThunderList;
        public List<EnemyLightningBolt> LightningList;
        public List<EnemyAnvil> AnvilList;

        public void Initialize()
        {
            audienceBounce = new AudienceBounce(8, 882, 165);
            staticImage = new StaticImage();
            player = new Character(new Vector2(0, 0));
            enemy = new Enemy(new Vector2(0, 0));
            BulletList = new List<Bullet>();
            TeleportList = new List<EnemyTeleport>();
            EnemyBulletList = new List<EnemyBullet>();
            ThunderList = new List<EnemyThunderBubble>();
            LightningList = new List<EnemyLightningBolt>();
            AnvilList = new List<EnemyAnvil>();
        }

        public void Render()
        {
            if (player.Health > 0)
            {
                audienceBounce.Render();
                staticImage.RenderBG();
                enemy.Render();

                for (int i = 0; i < TeleportList.Count; i++)
                {
                    TeleportList[i].Render();
                }
            }

            player.Render();

            if (player.Health > 0)
            {
                for (int i = 0; i < BulletList.Count; i++)
                {
                    BulletList[i].Render();
                }
                for (int i = 0; i < EnemyBulletList.Count; i++)
                {
                    EnemyBulletList[i].Render();
                }
                for (int i = 0; i < ThunderList.Count; i++)
                {
                    ThunderList[i].Render();
                }
                for (int i = 0; i < LightningList.Count; i++)
                {
                    LightningList[i].Render();
                }
                for (int i = 0; i < AnvilList.Count; i++)
                {
                    AnvilList[i].Render();
                }

                if (!GameManager.Instance.Paused)
                {
                    UIEnemyHealthBar.RenderEnemyHP(enemy);
                    UIPlayerManager.RenderPlayerUI(player);
                }
            }
        }

        public void Update()
        {
            if (player.JustHit)
            {
                player.JustHit = false;
                GameManager.Instance.PauseCounter = 3;
            }

            if (GameManager.Instance.PauseCounter > 0)
            {
                if (!GameManager.Instance.Paused)
                {
                    GameManager.Instance.PauseCounter--;
                }
                return;
            }

            CameraShake.Instance.Update();
            audienceBounce.Update();
            player.Update(player);

            if (player.Health > 0)
            {
                enemy.Update();
                enemy.CheckCollision(player);

                for (int i = 0; i < BulletList.Count; i++)
                {
                    BulletList[i].Update();
                    BulletList[i].CheckCollisions(enemy);
                }

                for (int i = 0; i < EnemyBulletList.Count; i++)
                {
                    EnemyBulletList[i].Update();
                    EnemyBulletList[i].CheckPositions(player);
                }

                for (int i = 0; i < TeleportList.Count; i++)
                {
                    TeleportList[i].Update();
                }

                for (int i = 0; i < ThunderList.Count; i++)
                {
                    ThunderList[i].Update();
                    ThunderList[i].CheckPositions(player);
                }

                for (int i = 0; i < LightningList.Count; i++)
                {
                    LightningList[i].Update();
                    LightningList[i].CheckPositions(player);
                }

                for (int i = 0; i < AnvilList.Count; i++)
                {
                    AnvilList[i].Update();
                    AnvilList[i].CheckPositions(player);
                }
            }
        }

        public void Restart()
        {
            // Reset Scene
            TeleportList.Clear();
            BulletList.Clear();
            EnemyBulletList.Clear();
            ThunderList.Clear();
            LightningList.Clear();
            AnvilList.Clear();
            audienceBounce.Reset();
            CameraShake.Instance.value = 0;
            FontManager.ResetText();

            // Reset Player
            player.Transform.Position = new Vector2(screenWidth / 4 - Character.PlayerWidth / 2, groundHeight - Character.PlayerHeight);
            player.ResetMomentum();
            player.Health = GameManager.Instance.HardMode ? player.MaxHealthHard : player.MaxHealthNormal;
            player.IsDead = false;
            player.Vulnerable = true;
            player.CurrentInvulnerabilityFrame = 0;
            player.Exploded = false;

            // Reset Enemy
            enemy.ResetTransform(new Vector2((screenWidth / 4) * 3 - Enemy.enemyWidth / 2, 250));
            enemy.ResetEnemy();
            enemy.Health = enemy.MaxHealth;
        }
    }
}
