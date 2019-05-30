using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerStateController : MonoBehaviour {

    public float moveSpeed = 2f;
    public float jumpForce = 15f;
    public GameObject bulletPrefab;
    [Range(0.01f,0.2f)]
    public float checkWallRadius = 0.2f;
    [Range(0.01f, 0.2f)]
    public float checkGroundRadius = 0.01f;
    private static Animator anim;
    private Rigidbody2D r2d;
    public PlayerController.PlayerStates previousState;
    private PlayerController.PlayerStates currentState;
    private Transform bulletSpawn;
    private Transform wallCheck;
    private Transform groundCheck;
    //private bool playerHasLanded = true;
    private bool jumpingFromWall = false;
    

    void OnEnable()
    {
        PlayerController.OnStateChange += OnStateChange;
    }

    void OnDisable()
    {
        PlayerController.OnStateChange -= OnStateChange;
    }
    
    void Start () {
        anim = GetComponent<Animator>();
        r2d = GetComponent<Rigidbody2D>();
        //landingCheck = transform.Find("LandingCheck").GetComponent<Collider2D>();
        //landingCheck.enabled = false;
        bulletSpawn = transform.Find("BulletSpawn");
        wallCheck = transform.Find("WallCheck");
        groundCheck = transform.Find("GroundCheck");
	}

    private void FixedUpdate()
    {
        OnStateCycle();
    }

    void OnStateCycle()
    {
        Vector3 localScale = transform.localScale;
        transform.localEulerAngles = Vector3.zero;
        //Debug.Log(currentState);
        switch (currentState)
        {
            case PlayerController.PlayerStates.idle:
                if (IsGrounded())
                {
                    anim.SetBool("Falling", false);
                    anim.SetBool("isOnWall", false);
                }
                r2d.velocity = new Vector2(0, r2d.velocity.y);
                break;

            case PlayerController.PlayerStates.left:
                if (!IsGrounded())
                {
                    anim.SetBool("Walking", false);
                    anim.SetBool("Falling", true);
                    if (IsNextToWall())
                    {
                        if (!jumpingFromWall)
                        {
                            anim.SetBool("Landing", false);
                            anim.SetBool("Falling", false);
                            anim.SetBool("Jumping", false);
                            anim.SetBool("isOnWall", true);
                            r2d.velocity = new Vector2(r2d.velocity.x, 0);
                            break;
                        }
                        jumpingFromWall = false;
                    }
                }
                else
                {
                    anim.SetBool("Falling", false);
                    anim.SetBool("isOnWall", false);
                }

                r2d.velocity = new Vector2(-moveSpeed, r2d.velocity.y);
                if (localScale.x > 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;
                }
                break;

            case PlayerController.PlayerStates.right:
                if (!IsGrounded())
                {
                    anim.SetBool("Walking", false);
                    anim.SetBool("Falling", true);
                    if (IsNextToWall())
                    {
                        if (!jumpingFromWall)
                        {
                            anim.SetBool("Landing", false);
                            anim.SetBool("Falling", false);
                            anim.SetBool("Jumping", false);
                            anim.SetBool("isOnWall", true);
                            r2d.velocity = new Vector2(r2d.velocity.x, 0);
                            break;
                        }
                        jumpingFromWall = false;
                    }
                }
                else
                {
                    anim.SetBool("Falling", false);
                    anim.SetBool("isOnWall", false);
                }

                r2d.velocity = new Vector2(moveSpeed, r2d.velocity.y);
                if (localScale.x < 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;   
                }
                break;
            case PlayerController.PlayerStates.jump:
                if (IsGrounded()) OnStateChange(PlayerController.PlayerStates.falling);
                break;

            case PlayerController.PlayerStates.landing:
                break;

            case PlayerController.PlayerStates.falling:
                if (IsGrounded()) OnStateChange(PlayerController.PlayerStates.landing);
                //if (IsNextToWall()) anim.SetBool("Falling", false);
                break;

            case PlayerController.PlayerStates.kill:
                OnStateChange(PlayerController.PlayerStates.resurrect);
                break;

            case PlayerController.PlayerStates.resurrect:
                OnStateChange(PlayerController.PlayerStates.idle);
                break;
        }
    }

    public void OnStateChange(PlayerController.PlayerStates newState)
    {
        if (newState == currentState)
            return;

        if (CheckIfAbortOnStateCondition(newState))
            return;

        if (!CheckForValidStatePair(newState))
            return;
        
        switch (newState)
        {
            case PlayerController.PlayerStates.idle:
                anim.SetBool("Walking", false);
                break;
            case PlayerController.PlayerStates.left:
            case PlayerController.PlayerStates.right:
                if (IsGrounded())
                {
                    anim.SetBool("Walking", true);
                }
                else anim.SetBool("Walking", false);
                break;
            case PlayerController.PlayerStates.jump:
                if ((/*playerHasLanded && */IsGrounded()) || (!IsGrounded() && IsNextToWall()))
                {
                    anim.SetBool("isOnWall", false);
                    anim.SetBool("Landing", false);
                    anim.SetBool("Jumping", true);

                    if (IsNextToWall())
                    {
                        jumpingFromWall = true;
                    }
                    r2d.AddForce(new Vector2(r2d.velocity.x, jumpForce) * moveSpeed * Time.deltaTime, ForceMode2D.Impulse);
                    //playerHasLanded = false;
                }
                break;
            case PlayerController.PlayerStates.landing:
                anim.SetBool("Falling", false);
                anim.SetBool("Landing", true);
                //playerHasLanded = true;
                break;
            case PlayerController.PlayerStates.falling:
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
                break;
            case PlayerController.PlayerStates.kill:
                break;
            case PlayerController.PlayerStates.resurrect:
                //transform.position = playerRespawnPoint.transform.position;
                //transform.rotation = Quaternion.identity; //rotacio: cap
                //GetComponent<Rigidbody2D>().velocity = Vector2.zero; //velocitat lineal: zero
                break;
            case PlayerController.PlayerStates.firingWeapon:
                PlayerController.stateDelayTimer[(int)PlayerController.PlayerStates.firingWeapon] = Time.time + 0.2f;
                GameObject newBullet = Instantiate(bulletPrefab);
                if(IsNextToWall())
                {
                    newBullet.transform.localScale = -transform.localScale;
                }
                else
                {
                    newBullet.transform.localScale = transform.localScale;
                }
                newBullet.transform.position = bulletSpawn.position;
                BulletController bullCon = newBullet.GetComponent<BulletController>();
                bullCon.ShootBullet();
                OnStateChange(currentState);
                break;
        }

        previousState = currentState;

        currentState = newState;
    }

    bool CheckForValidStatePair(PlayerController.PlayerStates newState)
    {
        bool returnVal = false;

        switch (currentState)
        {
            case PlayerController.PlayerStates.idle:
                // From idle you can go to any state
                returnVal = true;
                break;
            case PlayerController.PlayerStates.left:
                // From left you can go to any state
                returnVal = true;
                break;
            case PlayerController.PlayerStates.right:
                // From right you can go to any state
                returnVal = true;
                break;
            case PlayerController.PlayerStates.jump:
                // From jump you can go to the next step if certain conditions are met
                if (newState == PlayerController.PlayerStates.falling || newState == PlayerController.PlayerStates.kill || newState == PlayerController.PlayerStates.firingWeapon)
                    returnVal = true;
                else if (!IsNextToWall() && (newState == PlayerController.PlayerStates.left || newState == PlayerController.PlayerStates.right) || ((transform.localScale.x < 0 && previousState == PlayerController.PlayerStates.right && newState == PlayerController.PlayerStates.right) || (transform.localScale.x > 0 && previousState == PlayerController.PlayerStates.left  && newState == PlayerController.PlayerStates.left )))
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerController.PlayerStates.landing:
                // You get how this works
                if (newState == PlayerController.PlayerStates.left || newState == PlayerController.PlayerStates.right || newState == PlayerController.PlayerStates.idle || newState == PlayerController.PlayerStates.firingWeapon)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerController.PlayerStates.falling:
                if (newState == PlayerController.PlayerStates.landing || newState == PlayerController.PlayerStates.kill || newState == PlayerController.PlayerStates.firingWeapon || newState == PlayerController.PlayerStates.right || newState == PlayerController.PlayerStates.left)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerController.PlayerStates.kill:
                if (newState == PlayerController.PlayerStates.resurrect)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerController.PlayerStates.resurrect:
                if (newState == PlayerController.PlayerStates.idle)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerController.PlayerStates.firingWeapon:
                returnVal = true;
                break;
        }
        return returnVal;
    }

    bool CheckIfAbortOnStateCondition(PlayerController.PlayerStates newState)
    {
        bool returnVal = false;
        switch (newState)
        {
            case PlayerController.PlayerStates.idle:
                break;
            case PlayerController.PlayerStates.left:
                break;
            case PlayerController.PlayerStates.right:
                break;
            case PlayerController.PlayerStates.jump:
                break;
            case PlayerController.PlayerStates.landing:
                break;
            case PlayerController.PlayerStates.falling:
                break;
            case PlayerController.PlayerStates.kill:
                break;
            case PlayerController.PlayerStates.resurrect:
                break;
            case PlayerController.PlayerStates.firingWeapon:
                if (PlayerController.stateDelayTimer[(int)PlayerController.PlayerStates.firingWeapon] > Time.time) returnVal = true;
                break;
        }
        // True means 'Abort', false means 'Continue'
        return returnVal;
    }

    bool IsGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, checkGroundRadius, LayerMask.GetMask("Default"));
        if (hit != null)
        {
            return true;
        }
        return false;
    }

    bool IsNextToWall()
    {
        Collider2D hit = Physics2D.OverlapCircle(wallCheck.position, checkWallRadius, LayerMask.GetMask("Default"));
        if (hit != null)
        {
            return true;
        }
        return false;
    }

    public void Aim()
    {
        anim.SetFloat("Shooting", 1f);
        OnStateChange(PlayerController.PlayerStates.firingWeapon);
    }

    public void StopAim()
    {
        anim.SetFloat("Shooting", 0f);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(wallCheck.position, checkWallRadius);
    //}
}
