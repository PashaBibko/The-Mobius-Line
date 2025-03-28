using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    // Function to make the player jump
    // The function checks wether the player is grounded so external checks are not needed
    private void Jump(bool force = false)
    {
        // Checks wether the player is grounded
        // Can be overriden by passing true to force a jump
        if (m_Grounded || force)
        {
            // Applies an upwards force simulating a jump
            m_Body.AddForce(transform.up * m_JumpForce * m_Body.mass, ForceMode.Impulse);
        }
    }

    // Fixed Update is called once per physics update
    private void FixedUpdate()
    {
        // Works out the new state of the player
        UpdatePlayerState();

        // Calculates the movement direction
        m_MoveDir = (m_Orientation.forward * m_Input.y) + (m_Orientation.right * m_Input.x);

        //
        if (m_OnSlope)
        {
            // Calculates better move direction for sliding
            m_MoveDir = Vector3.ProjectOnPlane(m_MoveDir, m_SlopeHit.normal).normalized;
        }

        // Runs correct update function depending on player state
        switch (m_State)
        {
            case PlayerState.RUNNING:
                // Adds the force to the rigid body
                m_Body.AddForce(m_MoveDir.normalized * m_MoveSpeed * m_Body.mass * 10.0f, ForceMode.Force);

                // Stops player sliding slopes when they don't want to
                if (m_OnSlope)
                { m_Body.useGravity = false; }

                // Non-Slope running requires gravity on
                else
                { m_Body.useGravity = true; }

                break;

            case PlayerState.SLIDING:
                m_Body.useGravity = false; // Disables gravity on slopes

                // Calls correct update function
                UpdateSlidingState();
                break;

            case PlayerState.WALL_RUNNING:
                // Calls correct update function
                UpdateWallRunState();
                m_Body.useGravity = false; // Disables gravity on walls to stop the player sliding off them
                break;
        }

        // Calls the Jump function if the user has pressed jump
        // No grounded checks needed as Jump() function does that internally
        if (m_JumpKeyPressed)
        {
            Jump();
        }

        // Updates the counter for slide boost updates left
        m_TicksOfSlideBoostLeft = (int)Mathf.Clamp(m_TicksOfSlideBoostLeft - 1, 0, Mathf.Infinity);

        // Remvoes tiny ammounts of velocity because it was annoying me
        Vector3 v = m_Body.velocity;
        if (Mathf.Abs(v.x) < 0.1f) { v.x = 0.0f; }
        if (Mathf.Abs(v.y) < 0.1f) { v.y = 0.0f; }
        if (Mathf.Abs(v.z) < 0.1f) { v.z = 0.0f; }
        m_Body.velocity = v;
    }
}
