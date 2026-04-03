using UnityEngine;
using UnityEngine.Events;
using System;

public class ArenaBattle : MonoBehaviour
{
    [AddComponentMenu("")]
    private class OnDisableCallback : MonoBehaviour
    {
        public event Action OnDefeated;
        private bool _hasBeenActivated;

        private void OnEnable()
        {
            _hasBeenActivated = true;
        }

        private void OnDisable()
        {
            if (_hasBeenActivated)
                OnDefeated?.Invoke();
        }
    }

    public UnityEvent OnMyChildrenDestroy;
    private int _childrenCount;
    private bool _eventFired;

    private void Start()
    {
        RegisterAllChildren();
    }

    public void RegisterAllChildren()
    {
        _childrenCount = 0;
        _eventFired = false;

        foreach (Transform child in transform)
        {
            _childrenCount++;
            var callback = child.gameObject.AddComponent<OnDisableCallback>();
            callback.OnDefeated += OnChildDefeated;
        }
    }

    private void OnChildDefeated()
    {
        _childrenCount--;
        if (_childrenCount <= 0)
            TriggerVictory();
    }

    private void TriggerVictory()
    {
        if (_eventFired) return;
        _eventFired = true;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        OnMyChildrenDestroy?.Invoke();
    }
}