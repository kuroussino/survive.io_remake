using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PickableInstance : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI textItemPickUp;
    private GameObject item;
    private SpriteRenderer spriteRenderer;
    
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
    //    GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle.normalized * speedSliding);  
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
        textItemPickUp.text = $"Equip {item.name}";
    }

    /// <summary>
    /// <remarks>REMEMBER TO CHECK IF ITEM IS NULL!</remarks>
    /// </summary>
    public virtual void ActivateUI()
    {
        textItemPickUp.gameObject.SetActive(true);
    }

    /// <summary>
    /// <remarks>REMEMBER TO CHECK IF ITEM IS NULL!</remarks>
    /// </summary>
    public virtual void DeactivateUI()
    {
        textItemPickUp.gameObject.SetActive(false);
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
