using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] float m_MoveSpeed;
    [SerializeField] float m_GroundDrag;
    [SerializeField] float m_AirDrag;

    [Header("Ground Check")]
    [SerializeField] float m_PlayerHeight;
    [SerializeField] LayerMask m_GroundMask;
    [SerializeField] LayerMask m_SlopeMask;

    [Header("Sliding Settings")]
    [SerializeField] float m_SlideRequiredSpeed;
    [SerializeField] float m_SlideScaler;
    [SerializeField] float m_SlideDrag;
    [SerializeField] float m_SlideSpeed;

    [Header("Jump Settings")]
    [SerializeField] float m_JumpForce;

    [Header("KeyBinds")]
    [SerializeField] KeyCode m_JumpKey;
    [SerializeField] KeyCode m_SlideKey;

    [Header("References")]
    [SerializeField] Rigidbody m_Body;
    [SerializeField] Transform m_Orientation;
    [SerializeField] Transform m_PlayerTransform;

    [Header("Debug Settings")]
    [SerializeField] Text m_SpeedDisplay;

    // Current direction the user has inputted
    Vector2 m_Input;

    // The direction to move the player
    Vector3 m_MoveDir;

    // Player state tracker
    bool m_Grounded = false;
    bool m_OnSlope = false;
    bool m_Sliding = false;

    bool m_JumpKeyPressed = false;
    bool m_SlidingKeyPressed = false;

    int m_TicksOfSlideBoostLeft = 0;

    RaycastHit m_SlopeHit;

    // Start is called before the first frame update
    private void Start()
    {
        // Stops the rigidbody from rotatating when we don't want it to
        m_Body.freezeRotation = true;
    }

    // Updates the state of the user input
    private void UpdateInput()
    {
        // Calls get axis raw to ignore any uneeded scaling
        m_Input.x = Input.GetAxisRaw("Horizontal");
        m_Input.y = Input.GetAxisRaw("Vertical");

        // Checks wether the jump button has been pressed
        m_JumpKeyPressed = Input.GetKey(m_JumpKey);

        // Checks wehter the slide key is being pressed
        m_SlidingKeyPressed = Input.GetKey(m_SlideKey);
    }

    // Applies drag to the player
    private void ApplyDrag()
    {
        // Only applies ground drag if the player is on the floor
        if (m_Grounded)
        {
            m_Body.drag = m_GroundDrag;
        }

        // Applies sliding drag if sliding <- Very useful comment
        if (m_Sliding)
        {
            m_Body.drag = m_SlideDrag;
        }

        // Else it applies the air drag to the player
        else
        {
            m_Body.drag = m_AirDrag;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Performs raycasts to see what the player is standing on
        m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_PlayerHeight * 0.5f + 0.3f, m_GroundMask);
        m_OnSlope = Physics.Raycast(transform.position, Vector3.down, out m_SlopeHit, m_PlayerHeight * 0.5f + 0.3f, m_SlopeMask);

        // Updates the state of the user input
        UpdateInput();

        // Applies drag to the player
        ApplyDrag();

        // Displays the speed of the player to the screen
        m_SpeedDisplay.text = "Speed: " + m_Body.velocity.magnitude.ToString("0.00");
    }

    // Updates basic movement and player jumping
    private void UpdatePlayerPosition()
    {
        // Sliding has its own movement code so the force being applied here is not needed
        if (m_Sliding == false)
        {
            // Adds the force to the rigid body
            m_Body.AddForce(m_MoveDir.normalized * m_MoveSpeed * m_Body.mass * 10.0f, ForceMode.Force);
        }

        // Jumps if the jump key has been pressed
        if (m_JumpKeyPressed)
        {
            // The jump function stops jumping if not grounded so no check is needed here
            Jump();
        }
    }

    // Handles the logic for starting to slide
    private void StartSlide()
    {
        // Shrinks the player to give appearance of sliding
        m_PlayerTransform.localScale = new Vector3(1.0f, m_SlideScaler, 1.0f);

        // Applies a downward force as shrinking the player scale causes them to float
        m_Body.AddForce(Vector3.down * m_Body.mass * 5.0f, ForceMode.Impulse);

        // Applies a boost of a force at the beginning of a slide
        m_TicksOfSlideBoostLeft = 10;
    }

    // Handles the logic for ending the slide
    private void StopSlide()
    {
        // Grows the player back to normal scale
        m_PlayerTransform.localScale = Vector3.one;

        // Removes any of the slide boost that may be left
        m_TicksOfSlideBoostLeft = 0;
    }

    // Function to manage the sliding of the player
    private void UpdateSlidingState()
    {
        // Works out wether the player's velocity is high enough to slide
        Vector3 vel = m_Body.velocity;
        bool canSlide =
        !(
            Mathf.Abs(vel.x) < m_SlideRequiredSpeed &&
            Mathf.Abs(vel.z) < m_SlideRequiredSpeed
        ) || true;

        // Checks wether the key state is valid for starting a slide
        if (m_SlidingKeyPressed == true && m_Sliding == false)
        {
            // Checks player is moving in a direction
            if (canSlide)
            {
                m_Sliding = true; // Updates the sliding state

                StartSlide();
            }
        }

        // Checks wether the player has stopped a slide or
        // the player sliding if they are moving too slow
        else if ((m_SlidingKeyPressed == false && m_Sliding == true) || (canSlide == false && m_Sliding == true))
        {
            m_Sliding = false; // Updates the sliding state

            StopSlide();
        }

        // Correctly applies force on slopes
        if (m_Sliding && m_OnSlope)
        {
            Vector3 slopeDir = m_SlopeHit.normal;
            slopeDir.y = 0.0f - slopeDir.y;
            m_Body.AddForce(slopeDir.normalized * m_SlideSpeed * m_Body.mass * 10, ForceMode.Force);
        }

        // If at the start of a slide provides a boost to the player or if the player is on a slope
        else if (m_TicksOfSlideBoostLeft != 0)
        {
            m_Body.AddForce(m_MoveDir.normalized * m_SlideSpeed * m_Body.mass * 10, ForceMode.Force);
        }
    }

    // Function to make the player jump
    // The function checks wether the player is grounded so external checks are not needed
    private void Jump(bool force = false)
    {
        // Checks wether the player is grounded
        // Can be overriden by passing true to force a jump
        if (m_Grounded || force)
        {
            // Applies an upwards force simulating a jump
            m_Body.AddForce(transform.up * m_JumpForce * m_Body.mass, ForceMode.Impulse);
        }
    }

    // Fixed Update is called once per physics update
    private void FixedUpdate()
    {
        // Calculates the movement direction
        m_MoveDir = (m_Orientation.forward * m_Input.y) + (m_Orientation.right * m_Input.x);

        // Does additional calculations on the movement direction if on a slope
        if (m_OnSlope)
        {
            m_MoveDir = Vector3.ProjectOnPlane(m_MoveDir, m_SlopeHit.normal).normalized;

            m_Body.useGravity = false; // Disables gravity on slopes
        }

        else
        {
            m_Body.useGravity = true; // Renables gravity to stop errors
        }

        // Updates the player sliding state
        UpdateSlidingState();

        // Updates the position of the player
        UpdatePlayerPosition();

        // Updates the counter for slide boost updates left
        m_TicksOfSlideBoostLeft = (int)Mathf.Clamp(m_TicksOfSlideBoostLeft - 1, 0, Mathf.Infinity);

        // Remvoes tiny ammounts of velocity because it was annoying me
        Vector3 v = m_Body.velocity;
        if (Mathf.Abs(v.x) < 0.1f) { v.x = 0.0f; }
        if (Mathf.Abs(v.y) < 0.1f) { v.y = 0.0f; }
        if (Mathf.Abs(v.z) < 0.1f) { v.z = 0.0f; }
        m_Body.velocity = v;
    }
}
