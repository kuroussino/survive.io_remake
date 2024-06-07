using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private I_DamageOwner source;
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
        if (collision.gameObject.TryGetComponent(out I_Damageable enemy))
        {
            DamageQueryInfo info = new DamageQueryInfo
            {
                damageAmount = damage,
                source = source,
            };
            enemy.TakeDamage(info);
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
        this.source = data.source;
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