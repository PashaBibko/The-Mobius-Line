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

    // Initialistion function for the camera
    public void InitCamera(MeshRenderer renderer, PortalManager creator)
    {
        //
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
        renderer.material = m_RenderMaterial; // Set's the renderer to use the material
    }

    // Update is called every frame
    void Update()
    {
        // Calculates the player distance from where the portal will be rendererd
        Vector3 offset = CameraController.Instance().transform.position - m_DisplayPortal.pos;
        transform.parent.position = m_CapturePortal.TranslateOffset(offset);
    }
}
