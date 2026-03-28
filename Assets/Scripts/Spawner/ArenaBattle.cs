using UnityEngine;
using UnityEngine.Events;
using System;

public class ArenaBattle : MonoBehaviour
{
    [AddComponentMenu("")]
    private class OnDestroyCallback : MonoBehaviour
    {
        public event Action OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
    }

    public UnityEvent OnMyChildrenDestroy;

    private int _childrenCount;

    private void Start()
    {
        RegisterAllChildren();
    }

    public void RegisterAllChildren()
    {
        _childrenCount = 0;

        foreach (Transform child in transform)
        {
            _childrenCount++;

            var destroyer = child.gameObject.AddComponent<OnDestroyCallback>();
            destroyer.OnDestroyed += OnChildDestroyed;
        }
    }

    private void OnChildDestroyed()
    {
        _childrenCount--;

        if (_childrenCount <= 0)
            MyChildrenDestroy();
    }

    public void MyChildrenDestroy()
    {
        OnMyChildrenDestroy?.Invoke();
    }
}
