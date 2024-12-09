using System;
using UnityEngine;

public class Relay_OnTriggerExit : MonoBehaviour
{
    public Action<Collider> OnTriggerExitFromRelay;

    [SerializeField]
    protected bool debugMode;

    protected virtual void OnTriggerExit(Collider other)
    {
        OnTriggerExitFromRelay?.Invoke(other);

        if (debugMode)
            Debug.Log(
                $"[Relay_OnTriggerExit : OnTriggerExit ]  from: {gameObject.name} TriggerWith: {other.gameObject}",
                other.gameObject
            );
    }
}
