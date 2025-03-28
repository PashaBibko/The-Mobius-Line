using UnityEngine;
using UnityEngine.UI;

public partial class PlayerMovement : MonoBehaviour
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
}
