using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] float m_MoveSpeed;
    [SerializeField] float m_GroundDrag;
    [SerializeField] float m_AirDrag;

    [Header("Ground Check")]
    [SerializeField] float m_PlayerHeight;
    [SerializeField] LayerMask m_GroundMask;

    [Header("Jump Settings")]
    [SerializeField] float m_JumpForce;

    [Header("KeyBinds")]
    [SerializeField] KeyCode m_JumpKey;
    [SerializeField] KeyCode m_SlideKey;

    [Header("References")]
    [SerializeField] Rigidbody m_Body;
    [SerializeField] Transform m_Orientation;

    //
    Vector2 m_Input;

    //
    Vector3 m_MoveDir;

    // Player state tracker
    bool m_Grounded;
    bool m_Sliding;

    bool m_JumpKeyPressed;
    bool m_SlidingKeyPressed;

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

        // Else it applies the air drag to the player
        else
        {
            m_Body.drag = m_AirDrag;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Updates wether the player is touching the ground or not
        m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_PlayerHeight * 0.5f + 0.2f, m_GroundMask);

        // Updates the state of the user input
        UpdateInput();

        // Applies drag to the player
        ApplyDrag();
    }

    // Updates basic movement and player jumping
    private void UpdatePlayerPosition()
    {
        // Calculates the movement direction
        m_MoveDir = (m_Orientation.forward * m_Input.y) + (m_Orientation.right * m_Input.x);

        // Adds the force to the rigid body
        m_Body.AddForce(m_MoveDir.normalized * m_MoveSpeed * 10.0f, ForceMode.Force);

        // Jumps if the jump key has been pressed
        if (m_JumpKeyPressed)
        {
            // The jump function stops jumping if not grounded so no check is needed here
            Jump();
        }
    }

    // Handles the logic for starting to slide
    private void StartSlide()
    { }

    // Handles the logic for ending the slide
    private void StopSlide()
    { }

    // Function to manage the sliding of the player
    private void UpdateSlidingState()
    {
        // Checks wether the key state is valid for starting a slide
        if (m_SlidingKeyPressed == true  && m_Sliding == false)
        {
            m_Sliding = true; // Updates the sliding state
            
            StartSlide();
        }

        // Checks wether the key state is valid for starting a slide
        else if (m_SlidingKeyPressed == false && m_Sliding == true)
        {
            m_Sliding = false; // Updates the sliding state

            StopSlide();
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
            m_Body.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
        }
    }

    // Fixed Update is called once per physics update
    private void FixedUpdate()
    {
        // Updates the player sliding state
        UpdateSlidingState();

        // Updates the position of the player
        UpdatePlayerPosition();
    }
}
