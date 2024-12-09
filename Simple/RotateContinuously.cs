using UnityEngine;

public class RotateContinuously : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;

    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * speed);
    }
}
