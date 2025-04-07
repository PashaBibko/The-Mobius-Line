using System;
using UnityEngine;

[Serializable] public struct RoomEnteranceInfo
{
    [SerializeField] GameObject m_ExitRoom;
    [SerializeField] RoomEnterance m_RoomExit;
    [SerializeField] uint m_ExitID;

    public uint ID => m_ExitID;
    public bool PlayerIsLeaving() => m_RoomExit.IsLeaving();
}

public class RoomController : MonoBehaviour
{
    [Header("")]
    [SerializeField] RoomEnteranceInfo[] m_Enterances;

    bool m_IsMainRoom = false;

    public void SetAsMainRoom()
    {
        m_IsMainRoom = true;

        // Spawn all the rooms
    }

    private void Update()
    {
        foreach (RoomEnteranceInfo info in m_Enterances)
        {
            if (info.PlayerIsLeaving())
            {
                Debug.Log("KDJHSKGJDFHSKGJhk");
            }
        }
    }
}
