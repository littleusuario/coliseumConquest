using System;
using System.Collections.Generic;

namespace MyGame
{
    public abstract class Projectile
    {
        protected Transform transform;
        protected Animation currentAnimation;
        protected IntPtr image;

        protected Vector2 direction;
        protected Animation destroy;
        protected Animation idle;

        protected float bulletVel;
        protected float acceleration;
        protected float coolDown;
        protected bool destroyed;

        public Projectile()
        {
            transform = new Transform(new Vector2());
        }

        public Projectile(Vector2 position, Vector2 dir)
        {
            transform = new Transform(position);
            direction = dir;
        }

        public virtual void Render()
        {
            Engine.Draw(currentAnimation.CurrentFrame, transform.Position.x + CameraShake.Instance.value, transform.Position.y);
        }

        public virtual void Update()
        {
            currentAnimation.Update();
        }

        protected virtual void CreateAnimations(string idlePath, List<string> destroyPaths, float destroyFrameDuration)
        {
            IntPtr idleTexture = Engine.LoadImage(idlePath);
            idle = new Animation("Idle", new List<IntPtr> { idleTexture }, 1.0f, false);

            List<IntPtr> destroyTextures = new List<IntPtr>();
            foreach (var path in destroyPaths)
            {
                IntPtr frame = Engine.LoadImage(path);
                destroyTextures.Add(frame);
            }
            destroy = new Animation("Destroy", destroyTextures, destroyFrameDuration, false);
        }
    }
}
