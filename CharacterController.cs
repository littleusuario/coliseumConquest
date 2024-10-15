using MyGame;
using System;

public class CharacterController
{
    // Jugador
    private const float PlayerWidth = Character.PlayerWidth;
    private const float PlayerHeight = Character.PlayerHeight;

    // Escenario
    private int groundHeight = GameManager.Instance.LevelController.groundHeight;
    private int screenWidth = GameManager.Instance.LevelController.screenWidth;

    // Movimiento
    private float acceleration = 3000;
    private float deceleration = 2000;

    private float velocityX = 0;
    private float velocityY = 0;

    private float MaxSpeed = 450;

    private const float jumpSpeed = 700;
    private const float gravity = 1600f;

    private bool canMoveLeft = true;
    private bool canMoveRight = true;
    private bool isLookingLeft = false;
    private bool isLookingRight = true;

    private float footCounter = 0;

    // Salto doble
    private bool isJumping = false;
    private bool landed = false;

    private int jumpCounter = 0;
    private float actualCoolDown = 0f;
    private float dobleJumpCoolDown = 0.25f;

    // Input buffer
    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter = 0f;

    // Cooldown disparo
    private float shootCooldown = 0.15f;
    private float shootTime;

    private Transform transform;
    private CharacterWeapon weapon;

    private Sound shootSound;
    private Sound walkSound;
    private Sound walkSound2;
    private Sound emptyWalkSound;
    private Sound jumpSound;
    private Sound landSound;

    public float VelocityX { set { velocityX = value; } get { return velocityX; } }
    public float VelocityY { set { velocityY = value; } get { return velocityY; } }
    public float JumpBufferCounter { set { jumpBufferCounter = value; } get { return jumpBufferCounter; } }
    public bool IsLookingLeft { set { isLookingLeft = value; } get { return isLookingLeft; } }
    public bool IsLookingRight { set { isLookingRight = value; } get { return isLookingRight; } }
    public bool IsJumping { set { isJumping = value; } get { return isJumping; } }
    public bool Landed { set { landed = value; } get { return landed; } }
    public int JumpCounter { set { jumpCounter = value; } get { return jumpCounter; } }

    public CharacterController(Transform transform)
    {
        this.transform = transform;
        weapon = new CharacterWeapon();

        shootSound = new Sound("hitEnemy.wav");
        walkSound = new Sound("walk.wav");
        walkSound2 = new Sound("walk2.wav");
        emptyWalkSound = new Sound("emptyWalk.wav");
        jumpSound = new Sound("jump.wav");
        landSound = new Sound("land.wav");
    }

    public void Update()
    {
        GetInputs();
        HorizontalMovement();
        ConstraintArea();

        if (actualCoolDown > 0f)
        {
            actualCoolDown -= Time.DeltaTime;
        }

        if (shootCooldown > 0.25f)
        {
            shootCooldown -= Time.DeltaTime;
        }

        if (shootTime > 0)
        {
            shootTime -= Time.DeltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.DeltaTime;
        }
    }

    private void GetInputs()
    {
        bool pressingLeft = Engine.KeyPress(Engine.KEY_LEFT) && canMoveLeft;
        bool pressingRight = Engine.KeyPress(Engine.KEY_RIGHT) && canMoveRight;

        float relativePositionX = transform.Position.x / screenWidth;

        byte leftVolume = (byte)(255 * (1 - relativePositionX));
        byte rightVolume = (byte)(255 * relativePositionX);

        if (pressingLeft && !pressingRight)
        {
            velocityX = Math.Max(velocityX - acceleration * Time.DeltaTime, -MaxSpeed);
            isLookingLeft = true;
            isLookingRight = false;

            if (!walkSound.IsPlaying() && !isJumping)
            {
                footCounter++;

                if (footCounter == 0)
                {
                    emptyWalkSound.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    emptyWalkSound.SetPanning(leftVolume, rightVolume);
                }
                if (footCounter == 2)
                {
                    walkSound.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    walkSound.SetPanning(leftVolume, rightVolume);
                }
                if (footCounter == 3)
                {
                    emptyWalkSound.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    emptyWalkSound.SetPanning(leftVolume, rightVolume);
                }
                if (footCounter == 4)
                {
                    walkSound2.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    walkSound2.SetPanning(leftVolume, rightVolume);
                    footCounter = 0;
                }
            }
        }
        else if (pressingRight && !pressingLeft)
        {
            velocityX = Math.Min(velocityX + acceleration * Time.DeltaTime, MaxSpeed);
            isLookingLeft = false;
            isLookingRight = true;

            if (!walkSound.IsPlaying() && !isJumping)
            {
                footCounter++;

                if (footCounter == 1)
                {
                    walkSound.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    walkSound.SetPanning(leftVolume, rightVolume);
                }
                if (footCounter == 2)
                {
                    emptyWalkSound.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    emptyWalkSound.SetPanning(leftVolume, rightVolume);
                }
                if (footCounter == 3)
                {
                    walkSound2.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    walkSound2.SetPanning(leftVolume, rightVolume);
                }
                if (footCounter == 4)
                {
                    emptyWalkSound.PlayOnce(GameManager.Instance.audioMixer.WalkChannel);
                    emptyWalkSound.SetPanning(leftVolume, rightVolume);
                    footCounter = 0;
                }
            }
        }
        else
        {
            if (velocityX > 0)
            {
                velocityX = Math.Max(velocityX - deceleration * Time.DeltaTime, 0);
            }
            else if (velocityX < 0)
            {
                velocityX = Math.Min(velocityX + deceleration * Time.DeltaTime, 0);
            }

            walkSound.Stop();
        }

        // Saltar
        if (!Engine.KeyPress(Engine.KEY_Z))
        {
            GameManager.Instance.ZKeyReleased = true;
        }

        if (Engine.KeyPress(Engine.KEY_Z) && GameManager.Instance.ZKeyReleased)
        {
            jumpBufferCounter = jumpBufferTime;
            GameManager.Instance.ZKeyReleased = false;
        }

        if (jumpBufferCounter > 0 && actualCoolDown <= 0 && jumpCounter < GameManager.Instance.LevelController.player.MaxJumps)
        {
            Jump();
            jumpCounter++;
            actualCoolDown = dobleJumpCoolDown;
            jumpBufferCounter = 0;

            jumpSound.PlayOnce(GameManager.Instance.audioMixer.JumpChannel);
            jumpSound.SetPanning(leftVolume, rightVolume);
        }

        // Disparar
        if (Engine.KeyPress(Engine.KEY_X) && Engine.KeyPress(Engine.KEY_UP) && shootTime <= 0)
        {
            weapon.ShootVertical();
            shootTime = shootCooldown;

            shootSound.PlayOnce(GameManager.Instance.audioMixer.ShootChannel);
            shootSound.SetPanning(leftVolume, rightVolume);
        }

        if (Engine.KeyPress(Engine.KEY_X) && !Engine.KeyPress(Engine.KEY_UP) && shootTime <= 0)
        {
            weapon.ShootHorizontal(isLookingRight, isLookingLeft);
            shootTime = shootCooldown;

            shootSound.PlayOnce(GameManager.Instance.audioMixer.ShootChannel);
            shootSound.SetPanning(leftVolume, rightVolume);
        }
    }

    private void HorizontalMovement()
    {
        transform.Translate(new Vector2(velocityX * Time.DeltaTime, 0));
    }

    private void Jump()
    {
        velocityY = -jumpSpeed;
        isJumping = true;
        landed = false;
    }

    private void ConstraintArea()
    {
        velocityY += gravity * Time.DeltaTime;
        float playerY = transform.Position.y + velocityY * Time.DeltaTime;

        float relativePositionX = transform.Position.x / screenWidth;

        byte leftVolume = (byte)(255 * (1 - relativePositionX));
        byte rightVolume = (byte)(255 * relativePositionX);

        if (playerY > (groundHeight - PlayerHeight))
        {
            transform.Translate(new Vector2(0, (groundHeight - PlayerHeight) - transform.Position.y));
            jumpCounter = 0;
            velocityY = 0;
            actualCoolDown = 0;
        }
        else
        {
            transform.Translate(new Vector2(0, velocityY * Time.DeltaTime));
        }

        if (transform.Position.x < 0)
        {
            transform.Translate(new Vector2(-transform.Position.x, 0));
        }
        else if (transform.Position.x > screenWidth - PlayerWidth)
        {
            transform.Translate(new Vector2(screenWidth - PlayerWidth - transform.Position.x, 0));
        }

        if (transform.Position.y < 0)
        {
            transform.Translate(new Vector2(0, -transform.Position.y));
        }
        else if (playerY > (groundHeight - PlayerHeight))
        {
            transform.Translate(new Vector2(0, (groundHeight - PlayerHeight) - transform.Position.y));

            jumpCounter = 0;
            velocityY = 0;
            actualCoolDown = 0;

            if (isJumping && !landed)
            {
                landSound.PlayOnce(GameManager.Instance.audioMixer.LandChannel);
                landSound.SetPanning(leftVolume, rightVolume);
                isJumping = false;
                landed = true;
            }
        }

        if (transform.Position.x <= 0)
        {
            transform.Position = new Vector2(0, transform.Position.y);
            velocityX = 0;
            canMoveLeft = false;
        }
        else if (transform.Position.x >= screenWidth - PlayerWidth)
        {
            transform.Position = new Vector2((screenWidth - PlayerWidth), transform.Position.y);
            velocityX = 0;
            canMoveRight = false;
        }
        else
        {
            canMoveLeft = true;
            canMoveRight = true;
        }
    }
}
