using Unity.Netcode;
using UnityEngine;

public class BulletBehaviour : NetworkBehaviour, I_DamageSource
{
    private I_DamageOwner owner;
    private float damage;
    private float speed;
    private Vector3 direction;
    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate() {
        transform.localPosition += transform.up * speed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Method that triggers when it hits something, damaging the player or destroying itself
    /// </summary>
    /// <param name="collision"></param> 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.gameObject.TryGetComponent(out I_Damageable enemy))
        {
            DamageQueryInfo info = new DamageQueryInfo
            {
                damageAmount = damage,
                owner = owner,
                source = this,
            };
            DamageResponseInfo response = enemy.TakeDamage(info);

            if(response.attackAbsorbed)
                Destroy(gameObject);
        }
           
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    /// <param name="range"></param>
    public void SetDataBulletFromWeapon(BulletData data)
    {
        this.damage = data.damage;
        this.speed = data.speed;
        GetComponent<SpriteRenderer>().sprite = data.sprite;
        this.owner = data.source;
        GetComponent<NetworkObject>().Spawn();
        Destroy(gameObject, data.range / speed);
    }
}

public struct BulletData
{
    public I_DamageOwner source;
    public float damage;
    public float speed;
    public float range;
    public Sprite sprite;
}