using System;
using UnityEngine;

public class Relay_Start : MonoBehaviour
{
    public Action OnStartFromRelay;

    [SerializeField]
    protected bool debugMode;

    protected virtual void Start()
    {
        OnStartFromRelay?.Invoke();

        if (debugMode)
            Debug.Log($"[Relay_Start : Start ] from: {gameObject.name} ", gameObject);
    }
}
