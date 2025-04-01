using System.ComponentModel;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject m_OtherPortal;

    [Header("Set References")]
    [SerializeField] GameObject m_CameraPrefab;
    [SerializeField] MeshRenderer m_PortalRenderer;
    [SerializeField] Transform m_CapturePoint;

    PortalManager m_OtherManager;
    PortalCamera m_PortalCamera;

    public PortalManager Linked() => m_OtherManager;

    public Vector3 pos => transform.parent.position;
    public Quaternion rot => transform.parent.rotation;

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
    public Vector3 TranslateOffset(Vector3 offset)
    {
        m_CapturePoint.localPosition = offset;
        return m_CapturePoint.position;
    }
}
