using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject m_OtherPortal;

    [Header("Set References")]
    [SerializeField] GameObject m_CameraPrefab;
    [SerializeField] MeshRenderer m_PortalRenderer;

    [Header("Points")]
    [SerializeField] Transform m_PlayerPoint;

    // Private variables //

    PortalManager m_OtherManager;
    PortalCamera m_PortalCamera;

    // Gets the other end of the portal
    public PortalManager Linked() => m_OtherManager;

    // Gets the location of the player relative to the portal
    public Vector3 PlayerOffset() => m_PlayerPoint.localPosition;

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
        m_PortalCamera.InitCamera(m_PortalRenderer, this, transform.parent.localEulerAngles * 2.0f);
    }

    // Updates is called once per frame
    void Update()
    {
        // Updates the player position relative to the portal
        m_PlayerPoint.position = CameraController.Instance().transform.position;
    }

    // When something enters the portal
    private void OnTriggerEnter(Collider other)
    {
        // Changing the state if it is not the player will causes issues
        if (other.CompareTag(PlayerMovement.Object().tag) == false) { return; }

        // Calculates the differemce of some stuff
        Vector3 difference = PlayerMovement.Pos() - transform.position;

        // Checks they are able to travel through portals
        if (PlayerMovement.CanGoThroughPortals())
        {
            Debug.Log("Telpeoted opaktyer");

            // Rotates the player
            float rotDif = -Quaternion.Angle(transform.rotation, m_OtherManager.transform.rotation);
            rotDif += 180.0f;
            CameraController.Instance().RotatePlayerDirection(new Vector2(0f, rotDif));

            // Tells the player it went through a portal
            PlayerMovement.Instance().WentThroughPortal(rotDif);

            // Teleports the player
            Vector3 offset = Quaternion.Euler(0f, rotDif, 0f) * difference;
            PlayerMovement.SetPos(m_OtherManager.transform.position + offset - new Vector3(0, 1.5f, 0));
        }
    }

    //
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Force teleported player");

        // Changing the state if it is not the player will causes issues
        if (collision.gameObject.CompareTag(PlayerMovement.Object().tag) == false) { return; }

        // Doesnt do any logic to check if the player wanted to use a portal it just forces them

        // Calculates the differemce of some stuff
        Vector3 difference = PlayerMovement.Pos() - transform.position;

        // Rotates the player
        float rotDif = -Quaternion.Angle(transform.rotation, m_OtherManager.transform.rotation);
        rotDif += 180.0f;
        CameraController.Instance().RotatePlayerDirection(new Vector2(0f, rotDif));

        // Tells the player it went through a portal
        PlayerMovement.Instance().WentThroughPortal(rotDif);

        // Teleports the player
        Vector3 offset = Quaternion.Euler(0f, rotDif, 0f) * difference;
        PlayerMovement.SetPos(m_OtherManager.transform.position + offset - new Vector3(0, 1.5f, 0));
    }
}
