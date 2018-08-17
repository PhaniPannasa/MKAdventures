using UnityEngine;
using System.Collections;
using System;

public delegate void DeadEventHandler();

public class Player : Character
{
    private static Player instance;

    private IUseable useable;

    private int coins;

    [SerializeField]
    private float climbSpeed;

    private Vector2 startPos;

    public event DeadEventHandler Dead;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    private bool immortal = false;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float immortalTime;

    private float direction;

    private bool move;

    private float btnHorizontal;
    public  float horizontal;
    public float horizontal_temp;

    private float vertical;

    //start touch 
    public float maxTime = 0.5f;
    public float minSwifeDist = 50f;

    public float startTime;
    public float endTime;

    Vector3 touchstartPos;
    Vector3 touchendPos;

    float swipeDistance;
    float swipeTime;

    bool touchJump;
    private bool touchThrow;
    private bool directionChanged = true;

    public float rd_velocity_x = 0f;
    public float rd_velocity_y = 0f;

    
    //end touch

    public Rigidbody2D MyRigidbody { get; set; }

    public bool Slide { get; set; }

    public bool Jump { get; set; }

    public bool OnGround { get; set; }

    public bool OnLadder { get; set; }

    public bool Falling
    {
        get
        {
            return MyRigidbody.velocity.y < 0;
        }
    }

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    public override bool IsDead
    {
        get
        {
            if (healthStat.CurrentValue <= 0)
            {
                OnDead();
            }

            return healthStat.CurrentValue <= 0;
        }
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        MyRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= -14f)
            {
                Death();

            }
            OnGround = IsGrounded();
            HandleInput();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }


    void TouchDetect()
    {

        if (Input.touchCount > 0)
        {
            Debug.Log("touchCount");

            Touch touch = Input.GetTouch(0);
            

            if (touch.phase == TouchPhase.Began)
            {
                startTime = Time.time;
                touchstartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTime = Time.time;
                touchendPos = touch.position;
                swipeDistance = (touchendPos - touchstartPos).magnitude;// dis between two vectors.
                swipeTime = endTime - startTime;

                if (swipeTime < maxTime && swipeDistance > minSwifeDist)
                {

                    Swipe();

                }
                else
                {
                    //jump
                    Debug.Log(" Touch() ---swipe()--- else -- Jump");
                    touchJump = true;

                }
            }
            else
            {
                //jump
                //Debug.Log(" Touch() --- else -- Jump");
            }
        }

    }

    private void Swipe()
    {
        Vector2 distance = touchendPos - touchstartPos;
        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {

            //Debug.Log("Horizontal Swipe directionChanged ----------" + directionChanged);
            // Debug.Log("Horizontal Swipe directionChanged" + directionChanged);

            if (distance.x > 0)
            {
                //Debug.Log("right Swipe");
                horizontal = 1f;
            }

            if (distance.x < 0)
            {
                //Debug.Log("left Swipe");
                horizontal = -1f;

            }

            touchThrow = true; // for swipe attack


            //if (directionChanged)
            //{
            //    directionChanged = false;
            //}
            //else
            //{
            //    touchThrow = true;
            //}

        }
        else if (Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
        {
           // Debug.Log("vertical Swipe");

            if (distance.y > 0)
            {
                //Debug.Log("up Swipe");
                //m_Jump = true;
            }

            if (distance.y < 0)
            {
                //Debug.Log("down Swipe");
            }

        }
        else
        {
            //Jump
            //Debug.Log(" Swipe() --- else -- Jump");

        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if (!TakingDamage && !IsDead)
        {

            //OnGround = IsGrounded(); moving to update

            if (move)
            {
                this.btnHorizontal = Mathf.Lerp(btnHorizontal, direction, Time.deltaTime * 2);
                //HandleMovement(btnHorizontal);
                Flip(direction);
            }
            else
            {
                HandleMovement(horizontal, vertical);
                //Flip(horizontal);
            }

            HandleLayers();
        }

    }


    private void Use()
    {
        if (useable != null)
        {
            useable.Use();
        }
    }

    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }



    private void HandleMovement(float horizontal, float vertical)
    {
        if (Falling)
        {
            gameObject.layer = 10;
            MyAnimator.SetBool("land", true);
        }
        if (Jump && OnGround /*MyRigidbody.velocity.y == 0*/ && !OnLadder)
        {
            Debug.Log("-----------in side jumpmp OnGround--------------" + OnGround);
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
            //MyRigidbody.velocity =  (new Vector2(Mathf.Lerp(btnHorizontal, horizontal, Time.fixedDeltaTime * 2), jumpForce));
            //Player.Instance.transform.position += new Vector3(Player.Instance.horizontal == 1 ? 1 : -1, 0.5f, 0) * Time.fixedDeltaTime * 100;
            Jump = false;
        }
        if (!Attack && !Slide && (OnGround || airControl))
        {
            //Debug.Log("-----------in side run--------------" + Mathf.Lerp(btnHorizontal, horizontal, Time.fixedDeltaTime * 2));
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
        }
        //Debug.Log("-----------Jump--------------" + Jump/*+ "MyRigidbody.velocity.y == "+ MyRigidbody.velocity.y*/);
        //Debug.Log("-----------OnGround--------------" + OnGround);

        rd_velocity_x = MyRigidbody.velocity.x;
        rd_velocity_y = MyRigidbody.velocity.y;
        if (OnLadder)
        {
            MyAnimator.speed = vertical != 0 ? Mathf.Abs(vertical) : Mathf.Abs(horizontal);
            MyRigidbody.velocity = new Vector2(horizontal * climbSpeed, vertical * climbSpeed);
        }

        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
        Flip(horizontal);

        if (directionChanged)
        {
            directionChanged = false;
            touchThrow = false;
        }
        else
        {
            if (touchThrow)
            {
                MyAnimator.SetTrigger("throw");
                touchThrow = false;
            }

        }

    }

    private void HandleInput()
    {
        TouchDetect();
        // Commented for DUT
        //horizontal = Input.GetAxis("Horizontal");
        //vertical = Input.GetAxis("Vertical");
        //Debug.Log("-----------HandleInput--------------" + touchJump);
        if ((Input.GetKeyDown(KeyCode.Space) || touchJump) && !OnLadder && !Falling)
        {
            MyAnimator.SetTrigger("jump");
            Jump = true;
            touchJump = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            MyAnimator.SetTrigger("slide");
        }
        if ((Input.GetKeyDown(KeyCode.V) /*|| touchThrow*/))
        {
            MyAnimator.SetTrigger("throw");
            touchThrow = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            directionChanged = true;
            ChangeDirection();
        }
    }

    private bool IsGrounded()
    {
        if (MyRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }



    private void HandleLayers()
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
        }
    }

    public override void ThrowKnife(int value)
    {
        if (!OnGround && value == 1 || OnGround && value == 0)
        {
            base.ThrowKnife(value);
        }
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            spriteRenderer.enabled = false;

            yield return new WaitForSeconds(.1f);

            spriteRenderer.enabled = true;

            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator TakeDamage()
    {
        if (!immortal)
        {
            healthStat.CurrentValue -= 10;

            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");
                immortal = true;

                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);

                immortal = false;
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }

        }
    }

    public override void Death()
    {
        MyRigidbody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentValue = healthStat.MaxVal;
        transform.position = startPos;
    }

    public void BtnJump()
    {
        MyAnimator.SetTrigger("jump");
        Jump = true;
    }

    public void BtnAttack()
    {
        MyAnimator.SetTrigger("attack");
    }

    public void BtnSlide()
    {
        MyAnimator.SetTrigger("slide");
    }

    public void BtnTrow()
    {
        MyAnimator.SetTrigger("throw");
    }

    public void BtnMove(float direction)
    {
        this.direction = direction;
        this.move = true;
    }

    public void BtnStopMove()
    {
        this.direction = 0;
        this.btnHorizontal = 0;
        this.move = false;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "Coin")
        {
            GameManager.Instance.CollectedCoins++;
            Destroy(other.gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Player OnTriggerEnter2D " + other.tag);

        if (other.tag == "Useable")
        {
            useable = other.GetComponent<IUseable>();
        }

        base.OnTriggerEnter2D(other);
    }

    public void OnTriggerExit2D(Collider2D other)
    {

        if (other.tag == "Useable")
        {
            useable = null;
        }
    }

}
