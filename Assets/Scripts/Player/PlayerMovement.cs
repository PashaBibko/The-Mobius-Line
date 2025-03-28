using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    RUNNING,
    SLIDING,
    WALL_RUNNING
}

public class PlayerMovement : MonoBehaviour
{
    [Header("VIEWABLE ONLY")]
    [SerializeField] PlayerState m_State = PlayerState.RUNNING;

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

    [Header("Wall Run Settings")]
    [SerializeField] float m_WallRunSpeed;
    [SerializeField] float m_WallCheckDistance;
    [SerializeField] float m_DistanceOfFloorToWallRide;

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

    // Key state trackers
    bool m_JumpKeyPressed = false;
    bool m_SlidingKeyPressed = false;

    // The direction to move the player
    Vector3 m_MoveDir;

    // Player state trackers
    bool m_Grounded = false;
    bool m_OnSlope = false;

    // Trackers for the walls
    bool m_HitLhsWall = false;
    bool m_HitRhsWall = false;

    // Tracks if the distance of the ground is big enough
    bool m_IsFarEnoughOffGroundToWallRide = false;

    // Timer for slide boost duration left
    int m_TicksOfSlideBoostLeft = 0;

    // Raycast hit objects
    RaycastHit m_GroundHit;
    RaycastHit m_SlopeHit;
    RaycastHit m_LhsWall;
    RaycastHit m_RhsWall;

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
        switch (m_State)
        {
            case PlayerState.SLIDING:
                m_Body.drag = m_SlideDrag;
                break;

            default:
                // Applies different drag depending on if the player is on the ground or not
                if (m_Grounded)
                    { m_Body.drag = m_GroundDrag; }
                else
                    { m_Body.drag = m_AirDrag; }

                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Performs raycasts to see what the player is standing on
        m_Grounded = Physics.Raycast(transform.position, Vector3.down, out m_GroundHit, m_PlayerHeight * 0.5f + 0.3f, m_GroundMask);
        m_OnSlope = Physics.Raycast(transform.position, Vector3.down, out m_SlopeHit, m_PlayerHeight * 0.5f + 0.3f, m_SlopeMask);

        // Checks for walls either side of the player
        m_HitLhsWall = Physics.Raycast(transform.position, m_Orientation.right, out m_LhsWall, m_WallCheckDistance, m_GroundMask);
        m_HitRhsWall = Physics.Raycast(transform.position, -m_Orientation.right, out m_RhsWall, m_WallCheckDistance, m_GroundMask);

        // Checks the player is far enough of the ground to start wall running
        m_IsFarEnoughOffGroundToWallRide = m_GroundHit.distance > m_DistanceOfFloorToWallRide;

        // Updates the state of the user input
        UpdateInput();

        // Applies drag to the player
        ApplyDrag();

        // Displays the speed of the player to the screen
        m_SpeedDisplay.text = "Speed: " + m_Body.velocity.magnitude.ToString("0.00");
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
        // Correctly applies force on slopes
        if (m_OnSlope)
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

        //m_Body.AddForce(Vector3.down * m_Body.mass * 5.0f, ForceMode.Impulse);
    }

    private void UpdateWallRunState()
    {
        // Calculates the foward direction of the wall
        Vector3 normal = m_HitRhsWall ? m_RhsWall.normal : m_LhsWall.normal;
        Vector3 foward = Vector3.Cross(normal, transform.up);

        // Flips the foward direction if facing the other direction
        if ((m_Orientation.forward - foward).magnitude > (m_Orientation.forward - (-foward)).magnitude)
            { foward = -foward; }

        // Applies the wall running force to the player
        m_Body.AddForce(foward * m_WallRunSpeed * m_Body.mass * 10.0f, ForceMode.Force);

        // Removes any vertical velocity the player may have
        m_Body.velocity = new Vector3(m_Body.velocity.x, 0.0f, m_Body.velocity.z);
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

    private void UpdatePlayerState()
    {
        // Stores previous state
        PlayerState previous = m_State;

        // Works out wether the player's velocity is high enough to slide
        Vector3 vel = m_Body.velocity;
        bool canSlide = !(Mathf.Abs(vel.x) < m_SlideRequiredSpeed && Mathf.Abs(vel.z) < m_SlideRequiredSpeed);

        // Checks if the player is in the wall running state
        if (m_HitLhsWall || m_HitRhsWall)
            { m_State = PlayerState.WALL_RUNNING; }

        // Checks if the player is in the wall riding state
        else if (m_SlidingKeyPressed && (canSlide || m_OnSlope) && m_Grounded)
            { m_State = PlayerState.SLIDING; }

        // Defaults to ruuning
        else { m_State = PlayerState.RUNNING; }

        // Exits early if the state has not changed
        if (previous == m_State)
            { return; }

        // Calls exit function of old state
        switch (previous)
        {
            case PlayerState.SLIDING:
                StopSlide();
                break;

            default:
                break;
        }

        // Calls entry function of new state
        switch (m_State)
        {
            case PlayerState.SLIDING:
                StartSlide();
                break;

            default:
                break;
        }
    }

    // Fixed Update is called once per physics update
    private void FixedUpdate()
    {
        // Works out the new state of the player
        UpdatePlayerState();

        // Calculates the movement direction
        m_MoveDir = (m_Orientation.forward * m_Input.y) + (m_Orientation.right * m_Input.x);

        //
        if (m_OnSlope)
        {
            // Calculates better move direction for sliding
            m_MoveDir = Vector3.ProjectOnPlane(m_MoveDir, m_SlopeHit.normal).normalized;
        }

        // Runs correct update function depending on player state
        switch (m_State)
        {
            case PlayerState.RUNNING:
                // Adds the force to the rigid body
                m_Body.AddForce(m_MoveDir.normalized * m_MoveSpeed * m_Body.mass * 10.0f, ForceMode.Force);

                // Stops player sliding slopes when they don't want to
                if (m_OnSlope)
                    { m_Body.useGravity = false; }

                // Non-Slope running requires gravity on
                else
                    { m_Body.useGravity = true; }

                break;

            case PlayerState.SLIDING:
                m_Body.useGravity = false; // Disables gravity on slopes

                // Calls correct update function
                UpdateSlidingState();
                break;

            case PlayerState.WALL_RUNNING:
                // Calls correct update function
                UpdateWallRunState();
                m_Body.useGravity = false; // Disables gravity on walls to stop the player sliding off them
                break;
        }

        // Calls the Jump function if the user has pressed jump
        // No grounded checks needed as Jump() function does that internally
        if (m_JumpKeyPressed)
        {
            Jump();
        }

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
