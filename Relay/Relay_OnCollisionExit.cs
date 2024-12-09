using System;
using UnityEngine;

public class Relay_OnCollisionExit : MonoBehaviour
{
    public Action<Collision> OnCollisionExitFromRelay;

    [SerializeField]
    protected bool debugMode;

    protected virtual void OnCollisionExit(Collision other)
    {
        OnCollisionExitFromRelay?.Invoke(other);

        if (debugMode)
            Debug.Log(
                $"[Relay_OnCollisionExit : OnCollisionExit ] from: {gameObject.name} CollidedWith: {other.gameObject}",
                other.gameObject
            );
    }
}
