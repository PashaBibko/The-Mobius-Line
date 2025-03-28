using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    bool GetNormalOfClosestCollider(out Vector3 normal)
    {
        float dist = Mathf.Infinity;
        Collider closest = null;

        foreach (Collider collision in m_WallCollisions)
        { 
            Vector3 pos = collision.ClosestPoint(transform.position);
            Vector3 dif = transform.position - pos;

            float distance = dif.magnitude;

            dist = Mathf.Min(dist, distance);

            if (dist == distance)
            {
                closest = collision;
            }
        }

        if (dist > m_WallCheckDistance)
        {
            normal = Vector3.zero;
            return false;
        }

        Vector3 point = closest.ClosestPoint(transform.position);
        Vector3 dir = point - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir.normalized, out hit, (m_WallCheckDistance * 2.0f) + 1.0f, m_GroundMask))
        {
            normal = hit.normal;
            return true;
        }

        else
        {
            Debug.LogError("SOMETHING WENT WRONG");
            normal = Vector3.zero;
            return false;
        }
    }

    private void UpdateWallRunState()
    {
        // Calculates the foward direction of the wall
        Vector3 foward = Vector3.Cross(m_WallNormal, transform.up);

        if (m_FirstFrameWallRiding == true || m_LastWallNormal != m_WallNormal)
        {
            // Resets the tracker
            m_FirstFrameWallRiding = false;

            // Calculates it should flip the direction of the wall riding depending on the direction the player is looking
            bool flip = (m_Orientation.forward - foward).magnitude > (m_Orientation.forward - (-foward)).magnitude;

            // Stores it for all the other frames of the current wall ride
            m_FlippedWallRideDirectionFirstFrame = flip;
        }

        // Flips the direction if it did the first frame to stop the direction changing mid wall ride
        if (m_FlippedWallRideDirectionFirstFrame == true)
            { foward = -foward; }

        // Applies the wall running force to the player
        m_Body.AddForce(foward * m_WallRunSpeed * m_Body.mass * 10.0f, ForceMode.Force);

        // Removes any vertical velocity the player may have
        m_Body.velocity = new Vector3(m_Body.velocity.x, 0.0f, m_Body.velocity.z);

        // Sets the last wall normal to the current normal for later use
        m_LastWallNormal = m_WallNormal;
    }
}
