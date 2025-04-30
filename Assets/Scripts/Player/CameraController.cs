using UnityEngine;

// Stops errors from happening by making sure there is a Camera in the same group of components
[RequireComponent(typeof(Camera))]

// Controller from the Player Camera, NOT FOR PORTAL CAMERAS
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 m_Sensitivity;
    [SerializeField] float m_MaxAngle;

    [Header("References")]
    [SerializeField] Transform m_Orientation;
    [SerializeField] Transform m_Tracking;

    // Private variables //

    // Current angle the player is looking at
    Vector2 m_Rotation;

    // The instance of the camera
    static CameraController s_Instance = null;

    // Provides a way for external objects to interact with the camera
    public static CameraController Instance() => s_Instance;

    // Start is called before the first frame update
    void Start()
    {
        // Checks it is the only instance
        if (s_Instance == null)
        {
            // Sets itself as the instance
            s_Instance = this;
        }

        // Else throws an error in the console
        else
        {
            Debug.LogError("Multiple instances of CameraController within Scene");
        }

        // Locks the cursor and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovement.Instance().IsDead())
        {
            transform.position = m_Tracking.position;
            return;
        }

        // Gets the mouse input from the user
        Vector2 mouse = new Vector2
        (
            Input.GetAxisRaw("Mouse X") * Time.deltaTime * m_Sensitivity.x * MainMenu.sens,
            Input.GetAxisRaw("Mouse Y") * Time.deltaTime * m_Sensitivity.y * MainMenu.sens
        );

        // Applies the mouse movement to the camera angle
        m_Rotation.x -= mouse.y; // Yes these lines look cursed but blame Unity not me
        m_Rotation.y += mouse.x;

        // Clamps the angle to stop the camera from looping around
        m_Rotation.x = Mathf.Clamp(m_Rotation.x, -m_MaxAngle, m_MaxAngle);

        // Applies the rotation to the objects within Unity
        m_Orientation.rotation = Quaternion.Euler(0, m_Rotation.y, 0);
        transform.rotation = Quaternion.Euler(m_Rotation.x, m_Rotation.y, 0);

        // Sets its location to where it is tracking
        transform.position = m_Tracking.position;
    }

    // Adds a way for external forces to modify the direction the player is looking
    public void RotatePlayerDirection(Vector2 dif) => m_Rotation += dif;
}
