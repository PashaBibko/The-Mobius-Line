using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject m_OtherPortal;
    [SerializeField] float m_AngleDif;
    [SerializeField] float m_CamDif;

    [Header("Set References")]
    [SerializeField] GameObject m_CameraPrefab;
    [SerializeField] MeshRenderer[] m_Renderers;

    [Header("Points")]
    [SerializeField] Transform m_PlayerPoint;

    // Private variables //

    PortalManager m_OtherManager;
    PortalCamera m_PortalCamera;

    // Gets the other end of the portal
    public PortalManager Linked() => m_OtherManager;

    // Gets the location of the player relative to the portal
    public Vector3 PlayerOffset() => m_PlayerPoint.localPosition;

    public float CamDif() => m_CamDif;

    static bool s_TeleportedThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        // Validates that it is a portal
        m_OtherManager = m_OtherPortal.GetComponentInChildren<PortalManager>();
        if (m_OtherManager == null)
        {
            Debug.LogError("OtherPortal was not valid portal");
            return;
        }

        // Creates the camera in top-level heirachry and stores the PortalCamera script
        GameObject cam = Instantiate(m_CameraPrefab, transform.root.parent);
        m_PortalCamera = cam.GetComponentInChildren<PortalCamera>();

        // Initialises the camera so it renders to the portal and not the screen
        m_PortalCamera.InitCamera(m_Renderers, this, transform.parent.localEulerAngles * 2.0f);
    }

    // Updates is called every frame
    void Update()
    {
        // Updates the player position relative to the portal
        m_PlayerPoint.position = CameraController.Instance().transform.position;
    }

    void LateUpdate()
    {
        s_TeleportedThisFrame = true;
    }

    // When something enters the portal
    private void OnTriggerEnter(Collider other)
    {
        // Changing the state if it is not the player will causes issues
        if (other.CompareTag(PlayerMovement.Object().tag) == false) { return; }

        // Calculates if the player is going towards the portal
        Vector3 difference = PlayerMovement.Pos() - transform.position;

        // If this is true the player has crossed the portal
        if (PlayerMovement.CanGoThroughPortals() && s_TeleportedThisFrame == true)
        {
            // Rotates the player
            float rotDif = -Quaternion.Angle(transform.rotation, m_OtherManager.transform.rotation) + m_AngleDif;
            CameraController.Instance().RotatePlayerDirection(new Vector2(0f, rotDif));

            // Tellss the player it went through a portal
            PlayerMovement.Instance().WentThroughPortal(rotDif);

            // Teleports the player
            Vector3 offset = Quaternion.Euler(0f, rotDif, 0f) * difference;
            PlayerMovement.SetPos(m_OtherManager.transform.position + offset - new Vector3(0, 1.0f, 0));
        }
    }
}
