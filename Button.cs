namespace MyGame
{
    public class Button
    {
        private Transform transform;
        private string imagePath;

        public Button(Vector2 position, string imagePath)
        {
            transform = new Transform(position);
            this.imagePath = imagePath;
        }

        public void Render()
        {
            Engine.Draw(Engine.LoadImage(imagePath), (int)transform.Position.x, (int)transform.Position.y);
        }
    }
}
