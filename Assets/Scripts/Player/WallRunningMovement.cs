using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    private void UpdateWallRunState()
    {
        // Calculates the foward direction of the wall
        Vector3 normal = m_HitRhsWall ? m_RhsWall.normal : m_LhsWall.normal;
        Vector3 foward = Vector3.Cross(normal, transform.up);

        // Flips the foward direction if facing the other direction
        if ((m_Orientation.forward - foward).magnitude > (m_Orientation.forward - (-foward)).magnitude)
        { foward = -foward; }

        // Applies the wall running force to the player
        m_Body.AddForce(foward * m_WallRunSpeed * m_Body.mass * 10.0f, ForceMode.Force);

        // Removes any vertical velocity the player may have
        m_Body.velocity = new Vector3(m_Body.velocity.x, 0.0f, m_Body.velocity.z);
    }
}
