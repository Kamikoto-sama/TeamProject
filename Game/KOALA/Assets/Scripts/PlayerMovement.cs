using System;
using UnityEngine;

public class PlayerMovement : Interactive
{
    public CharacterController2D controller;

    public float runSpeed;
    public float climbSpeed;
    float horizontalMove;
    float verticalMove;
    public Transform CheckCeiling;
    public bool jump;
    public bool crouch;
    public bool run;
    public bool isCrossingCeiling;
    public bool isStuck;
    public LayerMask whatIsLadder;
    public LayerMask whatIsGround;
    public float distance;
    public Animator animator;
    private bool climbing;
    private Rigidbody2D rigidBody;
    private Animation _animation;

    private void Start()
    {
        TriggersTags.Add("Interactive");
        rigidBody = GetComponent<Rigidbody2D>();
        _animation = GetComponent<Animation>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speeds", Mathf.Abs(horizontalMove));
        isCrossingCeiling = Physics2D.OverlapCircle(CheckCeiling.position, .1f, whatIsGround);
        CheckForJump();
        CheckForCrouch();
        CheckForLadder();
        CheckForRun();
        CheckForInteraction();
    }

    private void CheckForInteraction()
    {
        if (ObjectInArea && Input.GetButtonDown("Interact"))
            animator.Play("interact");
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, run);
        jump = false;
        animator.SetBool("IsFallingDown", rigidBody.velocity.y < -1);
    }

    private void CheckForJump()
    {
        if (!Input.GetButtonDown("Jump")) return;
        jump = true;
        animator.SetBool("IsJumping", true);
    }

    private void CheckForCrouch()
    {
        if (Input.GetButtonDown("Crouch") && !isStuck)
        {
            crouch = true;
            animator.SetBool("IsCrouch", true);
        }

        if (Input.GetButtonUp("Crouch"))
            if (!isCrossingCeiling)
            {
                crouch = false;
                animator.SetBool("IsCrouch", false);
            }

        if (crouch && isCrossingCeiling)
            isStuck = true;
        if (isStuck && !isCrossingCeiling)
        {
            crouch = false;
            isStuck = false;
            animator.SetBool("IsCrouch", false);
        }
    }

    private void CheckForLadder()
    {
        var trans = transform;
        RaycastHit2D hitInfo = Physics2D.Raycast(trans.position, Vector2.up, distance, whatIsLadder);
        if (hitInfo.collider == null || !Input.GetKey(KeyCode.Tab))
        {
            if (climbing)
                rigidBody.gravityScale = 6;
            climbing = false;
            return;
        }

        climbing = true;
        rigidBody.gravityScale = 0;
        verticalMove = Input.GetAxisRaw("Vertical") * climbSpeed;
        // controller.Climb(verticalMove, jump);
        trans.Translate(Vector3.up * (verticalMove * Time.deltaTime));
    }

    private void CheckForRun()
    {
        var isRunning = Input.GetButton("Run") && !crouch;
        runSpeed = isRunning ? 140 : 70;
        animator.SetBool("IsRunn", isRunning);
    }
    
    public void OnLanding() => animator.SetBool("IsJumping", false);
}