using UnityEngine;

public abstract class CloseCombatWeapon : BaseWeapon
{
    [Header("Weapon arguments")]
    public Collider2D attackCollider;
    public LayerMask affectedMasks;
    public float damage;
    public float offset;
    public float defaultCoolDown;

    [Header("Private arguments")]
    private float coolDown;
    private ContactFilter2D _filter;
    private readonly Collider2D[] _hitResults = new Collider2D[32];

    void Awake()
    {
        _filter = new ContactFilter2D { useTriggers = true };
        if (affectedMasks != 0)
            _filter.SetLayerMask(affectedMasks);
    }

    private new void Update()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
            if (coolDown < 0) coolDown = 0;
        }
    }

    public override void ShootWeapon(Transform player, Vector2 dir)
    {
        if (coolDown > 0)
            return;
        if (!attackCollider)
        {
            Debug.LogWarning($"{name}: attackCollider not assigned!");
            return;
        }
        int hitCount = Physics2D.OverlapCollider(attackCollider, _filter, _hitResults);
        if (hitCount > 0)
            MeleeAttack(_hitResults, hitCount);
        coolDown = defaultCoolDown;

    }

    protected abstract void MeleeAttack(Collider2D[] targets, int targetsCount);

    private void OnDrawGizmosSelected()
    {
        if (attackCollider)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackCollider.bounds.center, attackCollider.bounds.size);
        }
    }
}
