namespace MyGame
{
    public class CharacterWeapon
    {
        private GenericPool<Bullet> pool;

        public CharacterWeapon()
        {
            InitializeDelegate<Bullet> bulletInitializer = (bullet, position, velocity, image, horizontalCheck) =>
            {
                bullet.Initialize((int)position.x, (int)position.y, velocity, image, horizontalCheck);
            };

            pool = new GenericPool<Bullet>(bulletInitializer);
        }

        public void ShootVertical()
        {
            Vector2 position = new Vector2(
                (int)GameManager.Instance.LevelController.player.Transform.Position.x + (Character.PlayerWidth / 2) - 5,
                (int)GameManager.Instance.LevelController.player.Transform.Position.y - 30
            );

            Bullet newBullet = pool.GetObject(position, new Vector2(0, -1), "assets/bullet/bulletY.png", false);
            GameManager.Instance.LevelController.BulletList.Add(newBullet);
        }

        public void ShootHorizontal(bool isLookingRight, bool isLookingLeft)
        {
            if (isLookingRight)
            {
                Vector2 position = new Vector2(
                    (int)GameManager.Instance.LevelController.player.Transform.Position.x + (Character.PlayerWidth / 2),
                    (int)GameManager.Instance.LevelController.player.Transform.Position.y + 16 + (Character.PlayerHeight / 2)
                );

                Bullet newBullet = pool.GetObject(position, new Vector2(1, 0), "assets/bullet/bulletX.png", true);
                GameManager.Instance.LevelController.BulletList.Add(newBullet);
            }
            else if (isLookingLeft)
            {
                Vector2 position = new Vector2(
                    (int)GameManager.Instance.LevelController.player.Transform.Position.x + (Character.PlayerWidth / 2) - 60,
                    (int)GameManager.Instance.LevelController.player.Transform.Position.y + 16 + (Character.PlayerHeight / 2)
                );

                Bullet newBullet = pool.GetObject(position, new Vector2(-1, 0), "assets/bullet/bulletX.png", true);
                GameManager.Instance.LevelController.BulletList.Add(newBullet);
            }
        }
    }
}
