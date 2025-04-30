using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class AryaMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    [Tooltip("A child Transform at Arya’s head height.")]
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    private Animator animator;
    private CharacterController cc;
    private Vector3 velocity;
    private bool wasGrounded;

    private bool isPickingUp = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        animator.applyRootMotion = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ─── MOUSE LOOK ─────────────────────────────
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ─── GROUND CHECK & JUMP RESET ─────────────────
        bool isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            if (!wasGrounded)
                animator.SetBool("IsJumping", false);
        }
        wasGrounded = isGrounded;

        // ─── INPUTS ────────────────────────────────
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump");
        bool pickupPressed = Input.GetKeyDown(KeyCode.E);

        // ─── JUMP & GRAVITY ─────────────────────────
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true);
        }
        velocity.y += gravity * Time.deltaTime;

        // ─── PICK UP ITEM ─────────────────────────────
        if (pickupPressed && !isPickingUp)
        {
            StartCoroutine(PlayPickUpAnimation());
        }

        // ─── MOVE CHARACTER ────────────────────────
        Vector3 move = transform.right * h + transform.forward * v;
        float speed = (v > 0.6f ? runSpeed : walkSpeed);
        cc.Move(move.normalized * speed * Time.deltaTime);

        cc.Move(Vector3.up * velocity.y * Time.deltaTime);

        // ─── DRIVE ANIMATOR Bools ───────────────────
        bool walking = Mathf.Abs(v) > 0.1f;
        bool strafeLeft = h < -0.1f;
        bool strafeRight = h > 0.1f;
        bool idle = !walking && !strafeLeft && !strafeRight && !animator.GetBool("IsJumping") && !isPickingUp;

        animator.SetBool("Idle", idle);
        animator.SetBool("Walking", walking && !isPickingUp);
        animator.SetBool("StrafeLeft", strafeLeft && !isPickingUp);
        animator.SetBool("StrafeRight", strafeRight && !isPickingUp);

        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);
    }

    private IEnumerator PlayPickUpAnimation()
    {
        isPickingUp = true;
        animator.SetBool("IsPickingUp", true);

        yield return new WaitForSeconds(1.0f); // Normal timing now

        animator.SetBool("IsPickingUp", false);
        isPickingUp = false;
    }


}
