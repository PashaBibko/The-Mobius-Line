using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PortalCamera : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Shader m_Shader;

    // Private members //

    PortalManager m_DisplayPortal;
    PortalManager m_CapturePortal;

    RenderTexture m_RenderTexture;
    Material m_RenderMaterial;

    Camera m_Camera;

    Vector3 m_Rot;

    // Initialistion function for the camera
    public void InitCamera(MeshRenderer[] renderers, PortalManager creator, Vector3 rot)
    {
        // Transfers the passed rotation to be stored within the class
        m_Rot = rot;

        // Stores both portals
        m_CapturePortal = creator.Linked();
        m_DisplayPortal = creator;

        // Gets the camera from the component
        m_Camera = gameObject.GetComponent<Camera>();

        // Creates the render texture
        RenderTextureDescriptor descriptor = new(Screen.width, Screen.height);
        m_RenderTexture = new RenderTexture(descriptor);

        // Creates a material from the Cutout shader
        // Needs to be created via code as all the materials have different settings
        m_RenderMaterial = new Material(m_Shader);

        // Links the camera to the mesh renderer
        m_Camera.targetTexture = m_RenderTexture; // Sets it's camera to display to the render texture instead of the screen
        m_RenderMaterial.mainTexture = m_RenderTexture; // Sets the material to use the render texture as it's texture

        foreach (Renderer renderer in renderers)
        {
            renderer.material = m_RenderMaterial;
        }
    }

    void LateUpdate()
    {
        // Gets the offset of the player from the display portal
        Vector3 offset = m_DisplayPortal.PlayerOffset();

        // Translates it to the capture portal and assigns it to the camera position
        Transform t = m_CapturePortal.transform.parent;
        transform.parent.position = (t.position) + (-t.forward * offset.z) + (t.up * offset.y) + (-t.right * offset.x);

        // Calculate angle stuff
        float angle = Quaternion.Angle(m_DisplayPortal.transform.parent.rotation, m_CapturePortal.transform.parent.rotation);
        Quaternion rotDif = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 newCamDir = rotDif * CameraController.Instance().transform.forward;
        Vector3 d = new(0f, m_CapturePortal.CamDif(), 0f);
        transform.parent.eulerAngles = Quaternion.LookRotation(newCamDir, Vector3.up).eulerAngles + m_Rot + d;
    }
}
