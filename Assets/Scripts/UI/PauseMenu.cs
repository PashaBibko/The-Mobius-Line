using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Canvas m_Canvas;

    private bool m_Paused = false;

    private void UpdateState()
    {
        m_Canvas.enabled = m_Paused;
        Time.timeScale = m_Paused ? 0.0f : 1.0f;

        Cursor.visible = m_Paused;
        Cursor.lockState = m_Paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Start()
    {
        UpdateState();
    }

    private void Update()
    {
        // Toggles the paused state if the ESC key is pressed //
        if (Input.GetKeyDown(KeyCode.Tab) && PlayerMovement.Instance().IsDead() == false)
        {
            m_Paused = !m_Paused;

            UpdateState();
        }
    }

    public void Unpause()
    {
        m_Paused = false;
        UpdateState();
    }

    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
