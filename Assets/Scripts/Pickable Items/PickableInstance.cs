using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PickableInstance : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject item;

    [SerializeField] private float speedSliding;
    /// <summary>
    /// 
    /// </summary>
    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();
    //    StartCoroutine(SlideCoroutine());
    //}

    private void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle.normalized * speedSliding);
    }

    /// <summary>
    /// Should be calling the implementation of a method of the I_Item interface, because every item must know how to behave when equipped.
    /// </summary>
    public void GetItem() 
    {
        print("yo ecco sono nell'inventario");
        Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void SetPrefabToSpawn(GameObject item, Sprite sprite)
    { 
        this.item = item.gameObject;
        spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// <remarks>REMEMBER TO CHECK IF ITEM IS NULL!</remarks>
    /// </summary>
    public virtual void ActivateUI()
    {
        if (item == null)
            return;
        print($"Equip {item.name}");
    }

    /// <summary>
    /// <remarks>REMEMBER TO CHECK IF ITEM IS NULL!</remarks>
    /// </summary>
    public virtual void DeactivateUI()
    {
        if (item == null)
            return;
        print($"Out of {item.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        ActivateUI();
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        DeactivateUI();
    }
}
