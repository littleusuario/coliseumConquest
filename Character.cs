using System;
using System.Collections.Generic;

namespace MyGame
{
    public class Character : GameObject, IDamageable
    {
        public const float PlayerWidth = 64;
        public const float PlayerHeight = 65;

        private bool isJumping = false;
        private bool isDead = false;
        private bool vulnerable = true;
        private bool justHit = false;
        private bool exploded = false;

        private int invulnerabilityFrames = 95;
        private int currentInvulnerabilityFrame = 0;

        private int health;
        private int maxHealthNormal = 3;
        private int maxHealthHard = 1;
        private int maxJumps = 2;

        private const float deathWait = 0.75f;
        private float currentDeath;

        public bool Exploded { set { exploded = value; } get { return exploded; } }
        public bool JustHit { set { justHit = value; } get { return justHit; } }
        public bool IsDead { set { isDead = value; } get { return isDead; } }
        public int Health { set { health = value; } get { return health; } }
        public int MaxJumps { set { maxJumps = value; } get { return maxJumps; } }
        public int MaxHealthNormal { set { maxHealthNormal = value; } get { return maxHealthNormal; } }
        public int MaxHealthHard { set { maxHealthHard = value; } get { return maxHealthHard; } }
        public bool Vulnerable { set { vulnerable = value; } get { return vulnerable; } }
        public int CurrentInvulnerabilityFrame { set { currentInvulnerabilityFrame = value; } get { return currentInvulnerabilityFrame; } }

        private Animation walk;
        private Animation walkUp;
        private Animation walkRight;
        private Animation walkUpRight;
        private Animation idleLeft;
        private Animation idleUpLeft;
        private Animation idleRight;
        private Animation idleUpRight;
        private Animation jumpLeft;
        private Animation jumpRight;
        private Animation jumpUpLeft;
        private Animation jumpUpRight;
        private Animation died;

        private Animation currentAnimation;

        private Sound hitSound;
        private Sound laughSound;
        private Sound explotion;

        public Transform Transform
        {
            get;
            set;
        }

        public CharacterController controller;

        public Character(Vector2 position) : base(position, PlayerWidth, PlayerHeight)
        {
            Transform = new Transform(position);
            controller = new CharacterController(Transform);

            CreateAnimations();
            currentAnimation = idleRight;

            hitSound = new Sound("hitSound.wav");
            laughSound = new Sound("hitSoundLaugh.wav");
            explotion = new Sound("explotion.wav");
        }

        public void ResetMomentum()
        {
            controller.VelocityX = 0;
            controller.VelocityY = 0;
            controller.JumpBufferCounter = 0f;
            controller.IsLookingLeft = false;
            controller.IsLookingRight = true;
            controller.Landed = false;
            controller.IsJumping = false;
        }

        public void Update(Character player)
        {
            if (player.Health > 0)
            {
                controller.Update();
                isJumping = controller.IsJumping;
                currentDeath = 0f;
                isDead = false;
                died.Restart();

                if (currentInvulnerabilityFrame > 0)
                {
                    currentInvulnerabilityFrame--;
                    if (currentInvulnerabilityFrame == 0)
                    {
                        vulnerable = true;
                    }
                }

                if (isJumping)
                {
                    if (controller.IsLookingLeft)
                    {
                        currentAnimation = Engine.KeyPress(Engine.KEY_UP) ? jumpUpLeft : jumpLeft;
                    }
                    if (controller.IsLookingRight)
                    {
                        currentAnimation = Engine.KeyPress(Engine.KEY_UP) ? jumpUpRight : jumpRight;
                    }
                }
                else
                {
                    if (Engine.KeyPress(Engine.KEY_LEFT) && !Engine.KeyPress(Engine.KEY_RIGHT))
                    {
                        if (Engine.KeyPress(Engine.KEY_UP))
                        {
                            currentAnimation = walkUp;
                        }
                        else
                        {
                            currentAnimation = walk;
                        }
                        currentAnimation.Update();
                    }
                    else if (Engine.KeyPress(Engine.KEY_RIGHT) && !Engine.KeyPress(Engine.KEY_LEFT))
                    {
                        if (Engine.KeyPress(Engine.KEY_UP))
                        {
                            currentAnimation = walkUpRight;
                        }
                        else
                        {
                            currentAnimation = walkRight;
                        }
                        currentAnimation.Update();
                    }
                    else
                    {
                        if (controller.IsLookingLeft)
                        {
                            currentAnimation = Engine.KeyPress(Engine.KEY_UP) ? idleUpLeft : idleLeft;
                        }
                        else if (controller.IsLookingRight)
                        {
                            currentAnimation = Engine.KeyPress(Engine.KEY_UP) ? idleUpRight : idleRight;
                        }
                    }
                }
            }
            else
            {
                currentDeath += Time.DeltaTime;
                vulnerable = true;

                if (currentDeath >= deathWait)
                {
                    if (!exploded)
                    {
                        exploded = true;
                        isDead = true;
                        explotion.PlayOnce(GameManager.Instance.audioMixer.HitChannel);
                        currentAnimation = died;
                    }
                    currentAnimation.Update();
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (vulnerable)
            {
                CameraShake.Instance.HitShake();
                health -= damage;

                if (health > 0)
                {
                    laughSound.PlayOnce(GameManager.Instance.audioMixer.HitChannel);
                    justHit = true;
                    vulnerable = false;
                    currentInvulnerabilityFrame = invulnerabilityFrames;
                }

                if (health == 0)
                {
                    hitSound.PlayOnce(GameManager.Instance.audioMixer.HitChannel);
                }
            }
        }

        public void Render()
        {
            if (!isDead)
            {
                if (currentInvulnerabilityFrame > 0 && !vulnerable)
                {
                    if ((currentInvulnerabilityFrame / 3) % 2 == 1)
                    {
                        Engine.Draw(currentAnimation.CurrentFrame, Transform.Position.x + CameraShake.Instance.value, Transform.Position.y);
                    }
                }
                else
                {
                    Engine.Draw(currentAnimation.CurrentFrame, Transform.Position.x + CameraShake.Instance.value, Transform.Position.y);
                }
            }
            else
            {
                Engine.Draw(currentAnimation.CurrentFrame, Transform.Position.x - 30, Transform.Position.y - 50);
            }
        }
        private void CreateAnimations()
        {

            // Movimiento

            List<IntPtr> walkTextures = new List<IntPtr>();
            for (int i = 0; i < 4; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/player/walk/walkLeft/{i}.png");
                walkTextures.Add(frame);
            }
            walk = new Animation("Walk", walkTextures, 0.1f, true);

            List<IntPtr> walkRightTextures = new List<IntPtr>();
            for (int i = 0; i < 4; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/player/walk/walkRight/{i}.png");
                walkRightTextures.Add(frame);
            }
            walkRight = new Animation("WalkRight", walkRightTextures, 0.1f, true);

            List<IntPtr> walkUpTextures = new List<IntPtr>();
            for (int i = 0; i < 4; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/player/walk/walkUpLeft/{i}.png");
                walkUpTextures.Add(frame);
            }
            walkUp = new Animation("WalkUp", walkUpTextures, 0.1f, true);

            List<IntPtr> walkUpRightTextures = new List<IntPtr>();
            for (int i = 0; i < 4; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/player/walk/walkUpRight/{i}.png");
                walkUpRightTextures.Add(frame);
            }
            walkUpRight = new Animation("WalkUpRight", walkUpRightTextures, 0.1f, true);

            // Idle

            IntPtr idleLeftTexture = Engine.LoadImage("assets/player/walk/walkLeft/0.png");
            idleLeft = new Animation("IdleLeft", new List<IntPtr> { idleLeftTexture }, 1.0f, false);

            IntPtr idleRightTexture = Engine.LoadImage("assets/player/walk/walkRight/0.png");
            idleRight = new Animation("IdleRight", new List<IntPtr> { idleRightTexture }, 1.0f, false);

            List<IntPtr> idleUpLeftTextures = new List<IntPtr>();
            IntPtr idleUpLeftTexture = Engine.LoadImage("assets/player/walk/walkUpLeft/0.png");
            idleUpLeftTextures.Add(idleUpLeftTexture);
            idleUpLeft = new Animation("IdleUpLeft", idleUpLeftTextures, 1.0f, false);

            List<IntPtr> idleUpRightTextures = new List<IntPtr>();
            IntPtr idleUpRightTexture = Engine.LoadImage("assets/player/walk/walkUpRight/0.png");
            idleUpRightTextures.Add(idleUpRightTexture);
            idleUpRight = new Animation("IdleUpRight", idleUpRightTextures, 1.0f, false);

            // Salto

            List<IntPtr> jumpLeftTextures = new List<IntPtr>();
            for (int i = 0; i < 1; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/player/jump/left/{i}.png");
                jumpLeftTextures.Add(frame);
            }
            jumpLeft = new Animation("JumpLeft", jumpLeftTextures, 1.0f, false);

            List<IntPtr> jumpRightTextures = new List<IntPtr>();
            for (int i = 0; i < 1; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/player/jump/right/{i}.png");
                jumpRightTextures.Add(frame);
            }
            jumpRight = new Animation("JumpRight", jumpRightTextures, 1.0f, false);

            List<IntPtr> jumpUpLeftTextures = new List<IntPtr>();
            IntPtr jumpUpLeftTexture = Engine.LoadImage("assets/player/jump/upLeft/0.png");
            jumpUpLeftTextures.Add(jumpUpLeftTexture);
            jumpUpLeft = new Animation("JumpUpLeft", jumpUpLeftTextures, 1.0f, false);

            List<IntPtr> jumpUpRightTextures = new List<IntPtr>();
            IntPtr jumpUpRightTexture = Engine.LoadImage("assets/player/jump/UpRight/0.png");
            jumpUpRightTextures.Add(jumpUpRightTexture);
            jumpUpRight = new Animation("JumpUpRight", jumpUpRightTextures, 1.0f, false);

            // Explosión

            List<IntPtr> boomTextures = new List<IntPtr>();
            for (int i = 0; i < 18; i++)
            {
                IntPtr frame = Engine.LoadImage($"assets/explosion/{i}.png");
                boomTextures.Add(frame);
            }
            died = new Animation("Boom", boomTextures, 0.035f, false);
        }
    }
}
