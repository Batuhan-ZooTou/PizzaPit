using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public Animator anim;
    public Vector3 dir;
    public GameObject camHolder;
    public float speed, sensitivity,maxForce,jumpForce,sprintSpeed,crouchSpeed;
    private Vector2 move, look;
    private float lookRotation;
    [SerializeField]bool isSprinting,isCrouching,isGrounded;

    [Header("Player Grounded")]
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    //input
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.ReadValueAsButton();
    }
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        Crouch();
        CheckForGround();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }



    void MovePlayer()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        dir = transform.TransformDirection(targetVelocity);
        targetVelocity *= isCrouching ? crouchSpeed : (isSprinting? sprintSpeed : speed);

        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, dir.y, velocityChange.z);

        //limit forces
        Vector3.ClampMagnitude(velocityChange, maxForce);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

    }
    
    void Jump()
    {
        Vector3 jumpForces = rb.velocity;
        if (isGrounded)
        {
            jumpForces.y = jumpForce;
        }
        rb.velocity = jumpForces;
    }
    void Crouch()
    {
        anim.SetBool("isCrouching", isCrouching);
    }
    void Look()
    {
        //Turn 
        transform.Rotate(Vector3.up * look.x * sensitivity);

        //Look
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    void LateUpdate()
    {
        Look();
    }

    public void CheckForGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        //if (isGrounded)
        //{
        //    rb.useGravity = false;
        //
        //}
        //else
        //{
        //    rb.useGravity = true;
        //
        //}

    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}
