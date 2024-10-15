using MyGame;
using System;
using System.Collections.Generic;

public class Enemy : GameObject, IDamageable
{
    public const float enemyWidth = 80;
    public const float enemyHeight = 80;

    private bool vulnerable = true;
    private int health;
    private int maxHealth = 100;
    private float levitationAmplitude = 90;
    private float levitationSpeed = 2;

    private bool isShaking = false;
    private float shakeOffsetX = 0;
    private float shakeTimer = 0;
    private float shakeDuration = 0.15f;
    private float shakeMagnitude = 2;
    private float shakeFrequency = 10;

    private float moveDirectionX;
    private float speedX = 0;
    private float speedY = 0;
    private float acceleration;

    private bool goingLeft = false;
    private bool goingRight = false;
    private bool offScreen = false;
    private bool dashing = false;
    private bool finishedDash = false;

    private float teleportTimer = 0f;
    private float teleportDelay = 0.15f;

    private int damageCount = 0;

    Random random = new Random();

    private Transform transform;
    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;

    private Animation Idle;
    private Animation enemyattack;
    private Animation enemyattack2;
    private Animation currentAnimation;
    private Animation teleport;
    private Animation right;
    private Animation left;

    private Sound hitEnemySound;
    private Sound hitEnemySound1;
    private Sound whistle;
    public Transform Transform { get { return transform; } }
    public bool IsShaking { set { isShaking = value; } get { return isShaking; } }
    public bool Vulnerable { set { vulnerable = value; } get { return vulnerable; } }
    public int Health { set { health = value; } get { return health; } }
    public int MaxHealth { set { maxHealth = value; } get { return maxHealth; } }
    public float ShakeOffsetX { set { shakeOffsetX = value; } get { return shakeOffsetX; } }

    public Enemy(Vector2 position) : base(position, enemyWidth, enemyHeight)
    {
        transform = new Transform(position);
        enemyMovement = new EnemyMovement(Transform);
        enemyAttack = new EnemyAttack(enemyMovement);
        CreateAnimations();
        currentAnimation = Idle;

        hitEnemySound = new Sound("shoot.wav");
        hitEnemySound1 = new Sound("hitEnemy1.wav");
        whistle = new Sound("whistle.wav");
    }
    public void Update()
    {
        vulnerable = !enemyAttack.IsTeleportOnCooldown;
        enemyAttack.Update(transform.Position);
        ManageAnimations();

        if (isShaking)
        {
            shakeTimer -= Time.DeltaTime;
            if (shakeTimer <= 0)
            {
                isShaking = false;
            }
        }

        if (!enemyAttack.IsTeleportOnCooldown && !enemyAttack.DashAttacking)
        {
            float levitationOffset = (float)Math.Sin(levitationSpeed * DateTime.Now.Millisecond / 1000f * Math.PI) * levitationAmplitude;
            transform.Position = new Vector2(transform.Position.x, (transform.Position.y + levitationOffset * Time.DeltaTime));
        }
        else if (enemyAttack.DashAttacking)
        {
            DashState();
        }
    }

    public void Render()
    {
        if (isShaking)
        {
            shakeOffsetX = (float)(shakeMagnitude * Math.Sin(2 * Math.PI * (shakeDuration - shakeTimer) * shakeFrequency));
        }
        Engine.Draw(currentAnimation.CurrentFrame, transform.Position.x + shakeOffsetX + CameraShake.Instance.value, transform.Position.y);
    }

    public void CheckCollision(Character player)
    {
        float enemyLeft = transform.Position.x;
        float enemyRight = transform.Position.x + enemyWidth;
        float enemyTop = transform.Position.y;
        float enemyBottom = transform.Position.y + enemyHeight;
        float playerLeft = player.Transform.Position.x;
        float playerRight = player.Transform.Position.x + Character.PlayerWidth;
        float playerTop = player.Transform.Position.y;
        float playerBottom = player.Transform.Position.y + Character.PlayerHeight;

        if (vulnerable && enemyRight >= playerLeft && enemyLeft <= playerRight && enemyBottom >= playerTop &&
            enemyTop <= playerBottom && player.Vulnerable && GameManager.Instance.LevelController.player is IDamageable)
        {
            if (player is IDamageable)
                player.TakeDamage(1);
        }
    }
    public void TakeDamage(int amount)
    {
        if (vulnerable)
        {
            damageCount = random.Next(-1, 1);

            if (damageCount == 0)
            {
                hitEnemySound.PlayOnce(GameManager.Instance.audioMixer.HitEnemyChannel);
            }
            else
            {
                hitEnemySound1.PlayOnce(GameManager.Instance.audioMixer.HitEnemyChannel);
            }

            health -= amount;
            isShaking = true;
            shakeTimer = shakeDuration;
        }
    }
    public void ResetTransform(Vector2 newPosition)
    {
        transform.Position = newPosition;
    }
    public void ResetEnemy()
    {
        goingLeft = false;
        goingRight = false;
        finishedDash = false;
        dashing = false;
        offScreen = false;
        teleportTimer = 0f;
        speedX = 0f;
        speedY = 0f;
        isShaking = false;
        dashing = false;
        offScreen = false;
        enemyAttack.IsAttacking = false;
        enemyAttack.DashAttacking = false;
        enemyAttack.AttackTimer = -1;
        enemyAttack.AttackNumber = 0;
        currentAnimation = Idle;
        currentAnimation.Update();
        enemyAttack.ResetCurrent();
    }
    private void DashState()
    {
        if (!offScreen)
        {

            if (GameManager.Instance.HardMode)
            {
                acceleration = 1200;
            }
            else
            {
                acceleration = 750;
            }

            speedY += acceleration * Time.DeltaTime;
            transform.Translate(new Vector2(0, -1), speedY * Time.DeltaTime);

            if (transform.Position.y < -enemyHeight * 2)
            {
                offScreen = true;

                speedY = 0;
            }
        }
        else
        {
            if (!dashing)
            {
                moveDirectionX = random.Next(0, 2) == 0 ? -1 : 1;

                if (!GameManager.Instance.HardMode)
                {
                    whistle.PlayOnce(GameManager.Instance.audioMixer.HitEnemyChannel);

                    if (moveDirectionX == -1)
                    {
                        whistle.SetPanning(0, 255);
                    }
                    else if (moveDirectionX == 1)
                    {
                        whistle.SetPanning(255, 0);
                    }
                }

                float newX = moveDirectionX == 1 ? 0 - (enemyWidth * 5) : GameManager.Instance.LevelController.screenWidth + (enemyWidth * 5);
                float newY = GameManager.Instance.LevelController.groundHeight - enemyHeight - 10;

                ResetTransform(new Vector2(newX, newY));

                dashing = true;
                teleportTimer = 0f;
            }
            else
            {
                teleportTimer += Time.DeltaTime;
                if (teleportTimer >= teleportDelay)
                {
                    if (moveDirectionX == -1 && !finishedDash)
                    {
                        goingLeft = true;
                        goingRight = false;
                        speedX += acceleration * Time.DeltaTime;
                        transform.Translate(new Vector2(-1, 0), speedX * Time.DeltaTime);
                    }
                    else if (!finishedDash)
                    {
                        goingRight = true;
                        goingLeft = false;
                        speedX += acceleration * Time.DeltaTime;
                        transform.Translate(new Vector2(1, 0), speedX * Time.DeltaTime);
                    }

                    if (teleportTimer >= teleportDelay + 2.6f)
                    {
                        if (!finishedDash)
                        {
                            acceleration = 750;
                            goingLeft = false;
                            goingRight = false;
                            float newX = random.Next(200, 1000);
                            ResetTransform(new Vector2(newX, -enemyHeight));
                            finishedDash = true;
                        }
                        else
                        {
                            speedY += acceleration * Time.DeltaTime;
                            transform.Translate(new Vector2(0, 1), speedY * Time.DeltaTime);
                            if (teleportTimer >= teleportDelay + 3.4f)
                            {
                                finishedDash = false;
                                dashing = false;
                                offScreen = false;
                                teleportTimer = 0f;
                                speedX = 0f;
                                speedY = 0f;
                                enemyAttack.DashAttacking = false;
                            }
                        }
                    }
                }
            }
        }
    }
    private void ManageAnimations()
    {
        switch (enemyAttack.AttackNumber)
        {
            case 1:
            case 5:
                currentAnimation = enemyattack2;
                break;
            case 3:
                if (!goingLeft && !goingRight)
                {
                    currentAnimation = Idle;
                }
                else if (goingLeft && !goingRight)
                {
                    currentAnimation = left;
                }
                else if (goingRight && !goingLeft)
                {
                    currentAnimation = right;
                }
                break;
            case 2:
                currentAnimation = enemyAttack.IsTeleportOnCooldown ? teleport : Idle;
                break;
            case 4:
                currentAnimation = enemyattack;
                break;
            default:
                currentAnimation = Idle;
                break;
        }
        currentAnimation.Update();
    }
    private void CreateAnimations()
    {
        List<IntPtr> idle = new List<IntPtr>();
        for (int i = 0; i < 16; i++)
        {
            IntPtr frame = Engine.LoadImage($"assets/enemy/idle/{i}.png");
            idle.Add(frame);
        }
        Idle = new Animation("Idle", idle, 0.060f, true);

        List<IntPtr> Attack = new List<IntPtr>();
        for (int i = 0; i < 1; i++)
        {
            IntPtr frame = Engine.LoadImage($"assets/enemy/shooting/bullet.png");
            Attack.Add(frame);
        }
        enemyattack = new Animation("enemyattack", Attack, 0.45f, false);

        List<IntPtr> Attack2 = new List<IntPtr>();
        for (int i = 0; i < 1; i++)
        {
            IntPtr frame = Engine.LoadImage($"assets/enemy/shooting/thunder.png");
            Attack2.Add(frame);
        }
        enemyattack2 = new Animation("enemyattack2", Attack2, 0.45f, false);

        List<IntPtr> Teleport = new List<IntPtr>();
        for (int i = 0; i < 1; i++)
        {
            IntPtr frame = Engine.LoadImage($"assets/enemy/teleport/2.png");
            Teleport.Add(frame);
        }
        teleport = new Animation("teleport", Teleport, 0.1f, false);

        List<IntPtr> Left = new List<IntPtr>();
        for (int i = 0; i < 1; i++)
        {
            IntPtr frame = Engine.LoadImage($"assets/enemy/dash/left.png");
            Left.Add(frame);
        }
        left = new Animation("left", Left, 0.1f, false);

        List<IntPtr> Right = new List<IntPtr>();
        for (int i = 0; i < 1; i++)
        {
            IntPtr frame = Engine.LoadImage($"assets/enemy/dash/right.png");
            Right.Add(frame);
        }
        right = new Animation("right", Right, 0.1f, false);
    }
}