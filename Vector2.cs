namespace MyGame
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static Vector2 Zero()
        {
            return new Vector2(0, 0);
        }
    }
}
