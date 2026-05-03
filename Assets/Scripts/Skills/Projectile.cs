using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    float   damage;
    float   speed;
    int     enemyLayer;
    bool    isMagic;

    public void Initialize(float dmg, float spd, int layer, bool magic = false)
    {
        damage     = dmg;
        speed      = spd;
        enemyLayer = layer;
        isMagic    = magic;
        Destroy(gameObject, 5f);
    }

    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        var enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null && !enemy.IsDead)
        {
            enemy.TakeDamage(damage);
            FloatingDamageSpawner.Spawn(
                transform.position,
                damage,
                isMagic ? DamageType.Magic : DamageType.Physical
            );
        }
        Destroy(gameObject);
    }
}
