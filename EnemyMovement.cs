using System;

namespace MyGame
{
    public class EnemyMovement
    {
        private Transform transform;
        private Random rnd = new Random();
        private bool teleported = false;

        public bool Teleported
        {
            set { teleported = value; }
            get { return teleported; }
        }

        public EnemyMovement(Transform transform)
        {
            this.transform = transform;
        }

        public void Teleport()
        {
            GameManager.Instance.LevelController.TeleportList.Add(new EnemyTeleport((int)transform.Position.x, (int)transform.Position.y, new Vector2(1, 0), "assets/enemy/teleport/0.png"));
            GameManager.Instance.LevelController.TeleportList.Add(new EnemyTeleport((int)transform.Position.x, (int)transform.Position.y, new Vector2(-1, 0), "assets/enemy/teleport/1.png"));

            float newX, newY;
            do
            {
                newX = rnd.Next(200, 1000);

                if (GameManager.Instance.HardMode)
                {
                    newY = rnd.Next(100, 250);
                }
                else
                {
                    newY = rnd.Next(100, 300);
                }
            }
            while (Math.Abs(newX - transform.Position.x) < 400 && Math.Abs(newY - transform.Position.y) < 400);

            transform.Position = new Vector2(newX, newY);
        }
    }
}
