using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    // Updates the state of the user input
    private void UpdateInput()
    {
        // Calls get axis raw to ignore any uneeded scaling
        m_Input.x = Input.GetAxisRaw("Horizontal");
        m_Input.y = Input.GetAxisRaw("Vertical");

        // Updates key press states
        m_JumpKeyPressed = Input.GetKey(m_JumpKey);
        m_SlidingKeyPressed = Input.GetKey(m_SlideKey);
        m_WallRunKeyPressed = Input.GetKey(m_WallRunKey);
    }

    // Applies drag to the player
    private void ApplyDrag()
    {
        switch (m_State)
        {
            case PlayerState.SLIDING:
                m_Body.drag = m_SlideDrag;
                break;

            case PlayerState.WALL_RUNNING:
                m_Body.drag = m_WallRideDrag;
                break;

            default:
                // Applies different drag depending on if the player is on the ground or not
                if (m_Grounded)
                { m_Body.drag = m_GroundDrag; }
                else
                { m_Body.drag = m_AirDrag; }

                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Performs raycasts to see what the player is standing on
        m_Grounded = Physics.Raycast(transform.position, Vector3.down, out m_StandingOn, m_PlayerHeight * 0.5f + 0.3f, m_GroundMask);
        m_OnSlope = m_StandingOn.normal != new Vector3(0.0f, 1.0f, 0.0f) && m_Grounded;

        // Checks the player is far enough of the ground to start wall running
        m_IsFarEnoughOffGroundToWallRide = m_StandingOn.distance > m_DistanceOfFloorToWallRide;

        // Updates the state of the user input
        UpdateInput();

        // Applies drag to the player
        ApplyDrag();

        // Displays the speed of the player to the screen
        m_SpeedDisplay.text = "Speed: " + new Vector3(m_Body.velocity.x, 0.0f, m_Body.velocity.z).magnitude.ToString("0.00") + " m/s";
    }
}
