using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    bool GetNormalOfClosestCollider(out Vector3 normal)
    {
        Debug.Log(m_WallCollisions.Count);

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

        // Flips the foward direction if facing the other direction
        if ((m_Orientation.forward - foward).magnitude > (m_Orientation.forward - (-foward)).magnitude)
        { foward = -foward; }

        // Applies the wall running force to the player
        m_Body.AddForce(foward * m_WallRunSpeed * m_Body.mass * 10.0f, ForceMode.Force);

        // Removes any vertical velocity the player may have
        m_Body.velocity = new Vector3(m_Body.velocity.x, 0.0f, m_Body.velocity.z);
    }
}
