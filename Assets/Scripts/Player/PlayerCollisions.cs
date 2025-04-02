using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        // Stops it trying to find the normals of portals
        if (other.CompareTag("Portal")) { return; }

        // Else adds it to the list
        m_WallCollisions.Add(other);
    }
}
