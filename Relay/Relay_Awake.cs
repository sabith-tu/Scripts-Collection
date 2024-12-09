using System;
using UnityEngine;

public class Relay_Awake : MonoBehaviour
{
    public Action OnAwakeFromRelay;

    [SerializeField]
    protected bool debugMode;

    protected virtual void Awake()
    {
        OnAwakeFromRelay?.Invoke();

        if (debugMode)
            Debug.Log(
                $"[Relay_OnCollisionEnter : OnCollisionEnter ] from: {gameObject.name} ",
                gameObject
            );
    }
}
