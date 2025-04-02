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
    [SerializeField] Transform m_Pos;
    [SerializeField] Transform m_Rot;

    PortalManager m_OtherManager;
    PortalCamera m_PortalCamera;

    public PortalManager Linked() => m_OtherManager;

    public Vector3 pos => transform.parent.position;
    public Quaternion rot => transform.parent.rotation;

    public void SetPos(Vector3 v) => transform.parent.position = v;

    public Vector3 PlayerOffset() => m_PlayerPoint.position;

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
        m_PortalCamera.InitCamera(m_PortalRenderer, this);
    }

    // Updates is called every frame
    void Update()
    {
        m_PlayerPoint.position = CameraController.Instance().transform.position;
    }

    public Vector3 TranslateOffsetToSpace(Vector3 pos, Vector3 euler)
    {
        m_Rot.localEulerAngles = euler;
        m_Pos.localPosition = pos;

        return m_Pos.position;
    }
}
