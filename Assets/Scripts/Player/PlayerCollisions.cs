using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (m_IsDead)
        {
            return;
        }

        if (other.CompareTag("Finish"))
        {
            m_IsDead = true;
            m_GameCanvas.enabled = false;
            m_CompletedCanvas.enabled = true;

            float timeTaken = Time.time - m_StartTime;

            m_CompletedText.text = "Congratulations you beat 'The Mobius Line'\nYou took " + timeTaken.ToString("0.00") + " seconds\nPress R to restart and try to get a faster time";
        }

        // Stops it trying to find the normals of portals
        if (other.CompareTag("Portal")) { return; }

        // Else adds it to the list
        m_WallCollisions.Add(other);
    }
}
