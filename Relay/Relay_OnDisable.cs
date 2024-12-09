using System;
using UnityEngine;

public class Relay_OnDisable : MonoBehaviour
{
    public Action OnDisableFromRelay;

    [SerializeField]
    protected bool debugMode;

    protected virtual void OnDisable()
    {
        OnDisableFromRelay?.Invoke();

        if (debugMode)
            Debug.Log($"[Relay_OnDisable : OnDisable ] from: {gameObject.name} ", gameObject);
    }
}
