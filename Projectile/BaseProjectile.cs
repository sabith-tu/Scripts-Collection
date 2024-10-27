using GeNa.Core;
using SABI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BaseProjectile : MonoBehaviour
{
    protected new Rigidbody rigidbody;

    [SerializeField]
    protected bool shootOnStart = true;

    [SerializeField]
    private GameObject MainProjectileObject;

    [SerializeField]
    private ParticleSystem vfx_spawn,
        vfx_hit;

    [SerializeField]
    private float force = 10;

    [SerializeField]
    private AudioClip audio_spawn,
        audio_hit;

    void Awake() => rigidbody = GetComponent<Rigidbody>();

    void Start()
    {
        if (shootOnStart)
            Shoot(transform.forward * force);
    }

    [ContextMenu("Shoot")]
    protected void Shoot(Vector3 direction)
    {
        rigidbody.linearVelocity = direction;
        if (audio_spawn)
            AudioManager.Instence.Play(audio_spawn);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider == gameObject.GetComponent<Collider>())
            return;
        if (MainProjectileObject)
            MainProjectileObject.SetActive(false);
        if (audio_hit)
            AudioManager.Instence.Play(audio_hit);
        if (vfx_hit)
        {
            vfx_hit.Play();
            this.DelayedExecution(vfx_hit.main.duration, () => this.DestroyGameObject());
        }

        Collider collider = GetComponent<Collider>();
    }
}
