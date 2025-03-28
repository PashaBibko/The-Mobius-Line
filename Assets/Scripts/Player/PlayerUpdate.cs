using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    // Updates the state of the user input
    private void UpdateInput()
    {
        // Calls get axis raw to ignore any uneeded scaling
        m_Input.x = Input.GetAxisRaw("Horizontal");
        m_Input.y = Input.GetAxisRaw("Vertical");

        // Checks wether the jump button has been pressed
        m_JumpKeyPressed = Input.GetKey(m_JumpKey);

        // Checks wehter the slide key is being pressed
        m_SlidingKeyPressed = Input.GetKey(m_SlideKey);
    }

    // Applies drag to the player
    private void ApplyDrag()
    {
        switch (m_State)
        {
            case PlayerState.SLIDING:
                m_Body.drag = m_SlideDrag;
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
        m_Grounded = Physics.Raycast(transform.position, Vector3.down, out m_GroundHit, m_PlayerHeight * 0.5f + 0.3f, m_GroundMask);
        m_OnSlope = Physics.Raycast(transform.position, Vector3.down, out m_SlopeHit, m_PlayerHeight * 0.5f + 0.3f, m_SlopeMask);

        // Checks for walls either side of the player
        m_HitLhsWall = Physics.Raycast(transform.position, m_Orientation.right, out m_LhsWall, m_WallCheckDistance, m_GroundMask);
        m_HitRhsWall = Physics.Raycast(transform.position, -m_Orientation.right, out m_RhsWall, m_WallCheckDistance, m_GroundMask);

        // Checks the player is far enough of the ground to start wall running
        m_IsFarEnoughOffGroundToWallRide = m_GroundHit.distance > m_DistanceOfFloorToWallRide;

        // Updates the state of the user input
        UpdateInput();

        // Applies drag to the player
        ApplyDrag();

        // Displays the speed of the player to the screen
        m_SpeedDisplay.text = "Speed: " + m_Body.velocity.magnitude.ToString("0.00");
    }
}
