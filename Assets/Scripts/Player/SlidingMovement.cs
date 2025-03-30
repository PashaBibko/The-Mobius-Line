using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    // Handles the logic for starting to slide
    private void StartSlide()
    {
        // Shrinks the player to give appearance of sliding
        m_PlayerTransform.localScale = new Vector3(1.0f, m_SlideScaler, 1.0f);

        // Applies a downward force as shrinking the player scale causes them to float
        m_Body.AddForce(Vector3.down * m_Body.mass * 5.0f, ForceMode.Impulse);

        // Applies a boost of a force at the beginning of a slide
        m_TicksOfSlideBoostLeft = 10;
    }

    // Handles the logic for ending the slide
    private void StopSlide()
    {
        // Grows the player back to normal scale
        m_PlayerTransform.localScale = Vector3.one;

        // Removes any of the slide boost that may be left
        m_TicksOfSlideBoostLeft = 0;
    }

    // Function to manage the sliding of the player
    private void UpdateSlidingState()
    {
        // Correctly applies force on slopes
        if (m_OnSlope)
        {
            Vector3 slopeDir = m_StandingOn.normal;
            slopeDir.y = 0.0f - slopeDir.y;
            m_Body.AddForce(slopeDir.normalized * m_SlideSpeed * m_Body.mass * 10, ForceMode.Force);
        }

        // If at the start of a slide provides a boost to the player or if the player is on a slope
        else if (m_TicksOfSlideBoostLeft != 0)
        {
            m_Body.AddForce(m_MoveDir.normalized * m_SlideSpeed * m_Body.mass * 10, ForceMode.Force);
        }

        //m_Body.AddForce(Vector3.down * m_Body.mass * 5.0f, ForceMode.Impulse);
    }
}
