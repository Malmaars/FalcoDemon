using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject cursorObject;

    [Header("Ground Movement")]
    public float moveSpeed;
    public float moveClamp;
    public float moveSlowdown;

    [Header("Air Movement")]
    public float airMoveSpeed;
    public float airMoveClamp;
    public float airMoveSlowdown;

    [Header("Jump")]
    bool ableToJump = true;
    public float jumpStrength;
    public float timeBetweenJumps;
    public float coyoteTime;
    public float jumpBuffer;
    bool bufferJumpBool;
    bool coyoteBool;

    [Header("Item")]
    public float distanceTillGrab;
    public bool holdingItem;
    public float itemWeight;
    public MainItem mainItem;
    public float throwForce;
    bool canPickup = true;
    bool airSlow = false;
    public float timeBetweenPickups;

    [Header("Collision stuff")]
    public LayerMask groundLayer;
    public BoxCollider2D playerCol;
    bool grounded;
    public float groundedDistance;
    public float collisionPushForce;
    bool leftBlock, rightBlock;
    Vector2 baseColSize;

    [Header("Gravity")]
    public float fallSpeedClamp;
    public float landDamp;

    [Header("Time")]
    public LevelTimer levelTimer;

    [Header("Animations")]
    public Animator playerAnimator;
    public SpriteRenderer playerRend;

    [Header("Sound")]
    AudioSource oneShotSource;
    public AudioClip[] jumps, throws;
    public AudioClip pickUp;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.StartLevel();
        rb = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<BoxCollider2D>();
        oneShotSource = GetComponent<AudioSource>();

        Cursor.visible = false;
        cursorObject = Instantiate(Resources.Load("AimCursor") as GameObject);

        Physics2D.IgnoreLayerCollision(6, 7);
        baseColSize = playerCol.size;

        PickUpItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.levelEnd)
        {
            Debug.Log("Level is at an end");
            return;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorObject.transform.position = new Vector2(worldPosition.x, worldPosition.y);

        playerAnimator.SetBool("Grounded", grounded);

        if (!levelTimer.timerRunning)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump") || Input.GetAxisRaw("Horizontal") != 0)
            {
                levelTimer.StartTimer();
            }
        }

        CheckForGround();
        CheckSides();

        if (grounded)
        {
            MovePlayer();
            DampenLanding();

            if (holdingItem)
                airSlow = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (holdingItem)
                ThrowItem();

            //else if (Vector2.Distance(transform.position, mainItem.currentPosition) < distanceTillGrab)
            //    PickUpItem();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LevelManager.ChangeScene(2);    
        }

        if (Input.GetButtonDown("Jump") || bufferJumpBool)
        {
            if (ableToJump && (grounded || coyoteBool))
                Jump(1);

            else
                StartCoroutine(BufferJump(jumpBuffer));
        }

        if (!grounded)
        {
            MovePlayerInAir();
            ClampFallSpeed();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1)
                SlowDownTime();
            else
                ReturnTimeToNormal();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.RestartCurrentLevel();
        }

        FlipPlayerVisual();

        //pass through some values to the animator

        playerAnimator.SetBool("HeadOn", holdingItem);
    }

    private void OnDrawGizmos()
    {
        Vector2[] raysPosDown = new Vector2[] {
            transform.position - new Vector3(0  - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y),
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y),
            transform.position - new Vector3(-playerCol.size.x / 2 - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y) };

        Vector2[] raysPosLeft = new Vector2[] {
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, 0 - playerCol.offset.y),
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y),
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, -playerCol.size.y / 2 - playerCol.offset.y) };

        Vector2[] raysPosRight = new Vector2[] {
            transform.position + new Vector3(playerCol.size.x / 2 + playerCol.offset.x, 0 + playerCol.offset.y),
            transform.position + new Vector3(playerCol.size.x / 2 + playerCol.offset.x, playerCol.size.y / 2 + playerCol.offset.y),
            transform.position + new Vector3(playerCol.size.x / 2 + playerCol.offset.x, -playerCol.size.y / 2 + playerCol.offset.y) };

        Gizmos.color = Color.blue;
        foreach (Vector2 pos in raysPosDown)
        {
            //draw a line
            Gizmos.DrawLine(pos, pos + Vector2.down * groundedDistance);
        }

        foreach (Vector2 pos in raysPosLeft)
        {
            //draw a line
            Gizmos.DrawLine(pos, pos + Vector2.left * collisionPushForce);
        }

        foreach (Vector2 pos in raysPosRight)
        {
            //draw a line
            Gizmos.DrawLine(pos, pos + Vector2.right * collisionPushForce);
        }

        if (Vector2.Distance(transform.position, mainItem.currentPosition) < distanceTillGrab)
        {
            Gizmos.color = Color.green;
        }
        else
            Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, mainItem.currentPosition);
    }

    void MovePlayer()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        float newMoveSpeed = moveSpeed;
        float newMoveClamp = moveClamp;
        float newMoveSlowDown = moveSlowdown;

        if (holdingItem)
        {
            //slow the player down

            newMoveSpeed = newMoveSpeed - itemWeight;
            newMoveSlowDown = newMoveSlowDown + itemWeight;

            newMoveClamp = newMoveClamp - itemWeight;
        }

        if(xInput < 0 && !leftBlock)
        {
            playerAnimator.SetBool("Walking", true);
            //move left
            rb.velocity = new Vector2(rb.velocity.x + xInput * Time.deltaTime * newMoveSpeed, rb.velocity.y);
        }

        if(xInput > 0 && !rightBlock)
        {
            playerAnimator.SetBool("Walking", true);
            //move right
            rb.velocity = new Vector2(rb.velocity.x + xInput * Time.deltaTime * newMoveSpeed, rb.velocity.y);
        }

        //no input received, give slowdown
        if (xInput == 0)
        {
            playerAnimator.SetBool("Walking", false);
            if (rb.velocity.x < 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x + newMoveSlowDown * Time.deltaTime, rb.velocity.y);
            }

            if (rb.velocity.x > 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x - newMoveSlowDown * Time.deltaTime, rb.velocity.y);
            }
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -newMoveClamp, newMoveClamp), rb.velocity.y);
    }

    void MovePlayerInAir()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        float newMoveSpeed = airMoveSpeed;
        float newMoveClamp = airMoveClamp;
        float newMoveSlowDown = airMoveSlowdown;

        if (holdingItem && airSlow)
        {
            //slow the player down

            newMoveSpeed = newMoveSpeed - itemWeight;
            newMoveSlowDown = newMoveSlowDown + itemWeight;

            newMoveClamp = newMoveClamp - itemWeight;
        }

        if (xInput < 0 && !leftBlock)
        {
            //move left
            rb.velocity = new Vector2(rb.velocity.x + xInput * Time.deltaTime * newMoveSpeed, rb.velocity.y);
        }

        if (xInput > 0 && !rightBlock)
        {
            //move right
            rb.velocity = new Vector2(rb.velocity.x + xInput * Time.deltaTime * newMoveSpeed, rb.velocity.y);
        }


        //no input received, give slowdown
        if (xInput == 0)
        {
            if (rb.velocity.x < 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x + newMoveSlowDown * Time.deltaTime, rb.velocity.y);
            }

            if (rb.velocity.x > 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x - newMoveSlowDown * Time.deltaTime, rb.velocity.y);
            }
        }


        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -newMoveClamp, newMoveClamp), rb.velocity.y);
    }

    void ClampFallSpeed()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -fallSpeedClamp, 100));
    }

    void CheckSides()
    {
        CheckLeft();
        CheckRight();
    }

    void CheckLeft()
    {
        Vector2[] raysPosLeft = new Vector2[] {
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, 0 - playerCol.offset.y),
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y),
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, -playerCol.size.y / 2 - playerCol.offset.y) };

        foreach (Vector2 pos in raysPosLeft)
        {
            //cast a ray down
            if (Physics2D.Raycast(pos, Vector2.left, collisionPushForce, groundLayer))
            {
                //push the player a little to the right
                leftBlock = true;
                return;
            }
        }

        leftBlock = false;
    }
    void CheckRight()
    {

        Vector2[] raysPosRight = new Vector2[] {
            transform.position + new Vector3(playerCol.size.x / 2 + playerCol.offset.x, 0 + playerCol.offset.y),
            transform.position + new Vector3(playerCol.size.x / 2 + playerCol.offset.x, playerCol.size.y / 2 + playerCol.offset.y),
            transform.position + new Vector3(playerCol.size.x / 2 + playerCol.offset.x, -playerCol.size.y / 2 + playerCol.offset.y) };


        foreach (Vector2 pos in raysPosRight)
        {
            //cast a ray down
            if (Physics2D.Raycast(pos, Vector2.right, collisionPushForce, groundLayer))
            {
                //push the player a little to the right
                rightBlock = true;
                return;
            }
        }
        rightBlock = false;
    }

    void CheckForGround()
    {
        //cast a ray down from the edges and center of the bottom of the player

        Vector2[] raysPos = new Vector2[] { 
            transform.position - new Vector3(0  - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y), 
            transform.position - new Vector3(playerCol.size.x / 2 - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y), 
            transform.position - new Vector3(-playerCol.size.x / 2 - playerCol.offset.x, playerCol.size.y / 2 - playerCol.offset.y) };

        foreach(Vector2 pos in raysPos)
        {
            //cast a ray down
            if(Physics2D.Raycast(pos, Vector2.down, groundedDistance, groundLayer))
            { 
                grounded = true;
                return;
            }
        }

        if (grounded && !coyoteBool)
        {
            coyoteBool = true;
            grounded = false;
            StartCoroutine(CoyoteTime(coyoteTime));
        }
    }

    void FlipPlayerVisual()
    {
        if(rb.velocity.x < -0.1f)
        {
            playerRend.flipX = true;
        }

        else if (rb.velocity.x > 0.1f)
        {
            playerRend.flipX = false;
        }
    }

    void DampenLanding()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -landDamp, 100));
    }

    public void Jump(float multiplier)
    {
        oneShotSource.PlayOneShot(jumps[Random.Range(0, 2)]);
        playerAnimator.SetBool("Jump", true);
        rb.velocity = new Vector2(rb.velocity.x, 0);

        if (!holdingItem)
        {
            rb.AddForce(Vector2.up * jumpStrength * multiplier, ForceMode2D.Impulse);
        }

        else
        {
            rb.AddForce(Vector2.up * (jumpStrength - itemWeight * 3) * multiplier, ForceMode2D.Impulse);
        }

        bufferJumpBool = false;

        StartCoroutine(JumpReset(timeBetweenJumps));
    }

    public void PickUpItem()
    {
        if (!canPickup)
            return;

        oneShotSource.PlayOneShot(pickUp);
        //check if item is in range, if so, pick it up
        holdingItem = true;

        //don't put it at exactly 1 or it will interfere with collision
        mainItem.AttachToObject(this.transform, new Vector2(0,0.86f));

        //change the collision
        //playerCol.size = new Vector2(playerCol.size.x, playerCol.size.y + mainItem.thisCol.radius + 1.15f);
        //playerCol.offset = new Vector2(0, (playerCol.size.y - 1) / 2);

    }

    public void ReleaseItem()
    {
        holdingItem = false;
        airSlow = false;
        mainItem.Detach();
    }

    void ThrowItem()
    {
        oneShotSource.PlayOneShot(throws[Random.Range(0, 2)]);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 throwDirection = (new Vector2(worldPosition.x, worldPosition.y) - new Vector2(transform.position.x, transform.position.y)).normalized;

        holdingItem = false;
        airSlow = false;

        playerAnimator.SetTrigger("Throw");

        if (playerRend.flipX)
            playerAnimator.SetFloat("ThrowXDirection", -throwDirection.x);
        else
            playerAnimator.SetFloat("ThrowXDirection", throwDirection.x);


        Debug.Log(throwDirection.x);

        mainItem.GetThrown(throwDirection * throwForce, 1);
        StartCoroutine(GrabReset(timeBetweenPickups));

        //playerCol.size = baseColSize;
        //playerCol.offset = Vector2.zero;
    }

    void SlowDownTime()
    {
        Time.timeScale = 0.5f;
    }

    void ReturnTimeToNormal()
    {
        Time.timeScale = 1;
    }

    public bool IsPlayerHoldingItem()
    {
        return holdingItem;
    }

    IEnumerator BufferJump(float delay)
    {
        //jump input is received, but the player isn't on the ground, so I'll buffer the jump, so that when the player is close to the ground, they will jump when they hit the ground
        bufferJumpBool = true;
        
        float elapsed = 0.0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        bufferJumpBool = false;
    }

    IEnumerator JumpReset(float delay)
    {
        ableToJump = false;
        float elapsed = 0.0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        ableToJump = true;
        playerAnimator.SetBool("Jump", false);
    }

    IEnumerator GrabReset(float delay)
    {
        canPickup = false;
        float elapsed = 0.0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        canPickup = true;
    }

    IEnumerator CoyoteTime(float delay)
    {
        float elapsed = 0.0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        coyoteBool = false;
    }
}
