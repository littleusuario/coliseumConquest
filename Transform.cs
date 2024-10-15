namespace MyGame
{
    public class Transform
    {
        private Vector2 position;
        public Vector2 Position
        {
            set
            {
                position = value;
            }

            get
            {
                return position;
            }
        }


        public Transform(Vector2 position)
        {
            this.position = position;
        }

        public void Translate(Vector2 direction, float speed)
        {
            position.x += direction.x * speed;
            position.y += direction.y * speed;
        }

        public void Translate(Vector2 direction)
        {
            position.x += direction.x;
            position.y += direction.y;
        }
    }
}
