using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventChannelListener : MonoBehaviour
{
    
    [SerializeField] private EventChannelSO _eventChannel;
    [SerializeField] private UnityEvent _response;

    private void OnEnable()
    {
        if (_eventChannel != null)
        {
            _eventChannel.OnEventRaised += OnEventRaised;
        }
    }

    private void OnDisable()
    {
        if (_eventChannel != null)
        {
            _eventChannel.OnEventRaised -= OnEventRaised;
        }
    }

    public void OnEventRaised()
    {
        _response.Invoke();
    }
}
