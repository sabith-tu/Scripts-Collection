using System;
using UnityEngine;

public class Relay_OnDestroy : MonoBehaviour
{
    public Action OnDestroyFromRelay;

    [SerializeField]
    protected bool debugMode;

    protected virtual void OnDestroy()
    {
        OnDestroyFromRelay?.Invoke();

        if (debugMode)
            Debug.Log($"[Relay_OnDestroy : OnDestroy ] from: {gameObject.name} ", gameObject);
    }
}
