using UnityEngine;

public enum PlayerState
{
    RUNNING,
    SLIDING,
    WALL_RUNNING
}

public partial class PlayerMovement : MonoBehaviour
{
    private void UpdatePlayerState()
    {
        // Stores previous state
        PlayerState previous = m_State;

        // Works out wether the player's velocity is high enough to slide
        Vector3 vel = m_Body.velocity;
        bool canSlide = !(Mathf.Abs(vel.x) < m_SlideRequiredSpeed && Mathf.Abs(vel.z) < m_SlideRequiredSpeed);

        // Checks if the player is in the wall running state
        if (m_HitLhsWall || m_HitRhsWall)
            { m_State = PlayerState.WALL_RUNNING; }

        // Checks if the player is in the wall riding state
        else if (m_SlidingKeyPressed && (canSlide || m_OnSlope) && m_Grounded)
            { m_State = PlayerState.SLIDING; }

        // Defaults to ruuning
        else { m_State = PlayerState.RUNNING; }

        // Exits early if the state has not changed
        if (previous == m_State)
            { return; }

        // Calls exit function of old state
        switch (previous)
        {
            case PlayerState.SLIDING:
                StopSlide();
                break;

            default:
                break;
        }

        // Calls entry function of new state
        switch (m_State)
        {
            case PlayerState.SLIDING:
                StartSlide();
                break;

            default:
                break;
        }
    }
}
