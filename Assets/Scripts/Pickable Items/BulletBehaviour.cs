using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
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
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
           
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    /// <param name="range"></param>
    public void SetDataBulletFromWeapon(float damage, float speed, float range, Sprite sprite)
    {
        this.damage = damage;
        this.speed = speed;
        GetComponent<SpriteRenderer>().sprite = sprite;
        Destroy(gameObject, range / speed);
    }
}
