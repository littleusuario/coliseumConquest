namespace MyGame
{
    public abstract class GameObject
    {
        protected Vector2 position;
        protected float width;
        protected float height;

        public Vector2 Position { get { return position; } }
        public float Width { get { return width; } }
        public float Height { get { return height; } }

        public GameObject(Vector2 position, float width, float height)
        {
            this.position = position;
            this.width = width;
            this.height = height;
        }
    }
}
