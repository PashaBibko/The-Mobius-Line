using UnityEngine;

public class RoomEnterance : MonoBehaviour
{
    [Header("Triggers")]
    [SerializeField] TriggerTracker m_TriggerA;
    [SerializeField] TriggerTracker m_TriggerB;

    bool m_PlayerIsLeaving = false;

    public bool IsLeaving() => m_PlayerIsLeaving;

    private void Update()
    {
        if (m_TriggerB.State() == true && m_TriggerA.State() == false)
        {
            m_PlayerIsLeaving = true;
        }

        else
        {
            m_PlayerIsLeaving = false;
        }
    }
}
