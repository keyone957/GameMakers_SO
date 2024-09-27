using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventChannelListener : MonoBehaviour
{
    
    [SerializeField] private EventChannelSO m_EventChannel;
    [SerializeField] private UnityEvent m_Response;

    private void OnEnable()
    {
        if (m_EventChannel != null)
        {
            m_EventChannel.OnEventRaised += OnEventRaised;
        }
    }

    private void OnDisable()
    {
        if (m_EventChannel != null)
        {
            m_EventChannel.OnEventRaised -= OnEventRaised;
        }
    }

    public void OnEventRaised()
    {
        m_Response.Invoke();
    }
}
