using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTracker : MonoBehaviour
{
    private bool m_IsTriggered = false;

    public bool State() => m_IsTriggered;

    private void OnTriggerEnter(Collider other) => m_IsTriggered = true;
    private void OnTriggerExit(Collider other) => m_IsTriggered = false;
}
