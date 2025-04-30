using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int m_TransitionFrames;

    [Header("References")]
    [SerializeField] Camera m_Camera;

    [Header("Canvases")]
    [SerializeField] Canvas m_StartCanvas;
    [SerializeField] Canvas m_OptionsCanvas;
    [SerializeField] Canvas m_ControlsCanvas;

    [Header("Options References")]
    [SerializeField] Text m_SensText;
    [SerializeField] Slider m_SensitivitySlider;

    public static float sens = 100.0f;

    private void Start()
    {
        m_StartCanvas.enabled = true;

        m_ControlsCanvas.enabled = false;
        m_OptionsCanvas.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StartGame() => SceneManager.LoadScene(1);
    public void OptionsMenu()
    {
        m_StartCanvas.enabled = false;
        m_ControlsCanvas.enabled = false;

        m_OptionsCanvas.enabled = true;
    }

    public void ControlsMenu()
    {
        m_StartCanvas.enabled = false;
        m_OptionsCanvas.enabled = false;

        m_ControlsCanvas.enabled = true;
    }
    public void StartMenu()
    {
        m_ControlsCanvas.enabled = false;
        m_OptionsCanvas.enabled = false;

        m_StartCanvas.enabled = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            StartMenu();
        }

        sens = m_SensitivitySlider.value;
        m_SensText.text = sens.ToString();
    }
}
