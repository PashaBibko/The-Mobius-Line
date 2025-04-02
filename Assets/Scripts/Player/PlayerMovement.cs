using System.Collections.Generic;
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
    [SerializeField] float m_WallRideDrag;

    [Header("KeyBinds")]
    [SerializeField] KeyCode m_JumpKey;
    [SerializeField] KeyCode m_SlideKey;
    [SerializeField] KeyCode m_WallRunKey;

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
    bool m_WallRunKeyPressed = false;

    // The direction to move the player
    Vector3 m_MoveDir;

    // Player state trackers
    bool m_Grounded = false;
    bool m_OnSlope = false;

    // Tracks if the distance of the ground is big enough
    bool m_IsFarEnoughOffGroundToWallRide = false;

    // Timer for slide boost duration left
    int m_TicksOfSlideBoostLeft = 0;

    // Wall riding trackers
    bool m_FirstFrameWallRiding = true;
    bool m_FlippedWallRideDirectionFirstFrame = false;
    Vector3 m_LastWallNormal;

    // What the player is standing on
    RaycastHit m_StandingOn;

    //
    BoxCollider m_WallCollider;

    //
    List<Collider> m_WallCollisions;

    //
    Vector3 m_WallNormal;

    // Only instance of the player
    static PlayerMovement s_Instance;

    public static Transform Orientation() => s_Instance.m_Orientation;
    public static Vector3 Pos() => s_Instance.transform.position;
    public static void SetPos(Vector3 v) => s_Instance.transform.parent.position = v;
    public static GameObject Object() => s_Instance.gameObject;

    // Start is called before the first frame update
    private void Start()
    {
        // Checks there is not more than one player at one time
        if (s_Instance != null)
        {
            Debug.LogError("Multiple players");
            return;
        }

        // Sets it to the instance
        s_Instance = this;

        // Stops the rigidbody from rotatating when we don't want it to
        m_Body.freezeRotation = true;

        // Creates the wall collider
        m_WallCollider = gameObject.AddComponent<BoxCollider>();
        m_WallCollider.size = new Vector3(m_WallCheckDistance * 2, 0.2f, m_WallCheckDistance * 2);
        m_WallCollider.center = new Vector3(0.0f, 0.5f, 0.5f);
        m_WallCollider.providesContacts = true;
        m_WallCollider.isTrigger = true;

        // Allocates memory for the list of collisions
        m_WallCollisions = new List<Collider>();
    }
}
