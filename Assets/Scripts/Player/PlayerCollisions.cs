using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        m_WallCollisions.Add(other);
    }
}
