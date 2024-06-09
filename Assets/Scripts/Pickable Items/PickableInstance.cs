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
    private GameObject prefabItem;
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
    /// 
    /// </summary>
    public void GetItem(Player player) 
    {
        I_Item item = prefabItem.GetComponent<I_Item>();
        if (item == null)
            return;

        if(player.TryCollectItem(item))
            Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void SetPrefabToSpawn(GameObject item, Sprite sprite)
    {
        prefabItem = item;
        spriteRenderer.sprite = sprite;
        textItemPickUp.text = $"Equip {item.name}";
    }

    /// <summary>
    /// <remarks>Public method that needs to be used when the item ist near the player to activate UI.</remarks>
    /// </summary>
    public virtual void ActivateUI()
    {
        textItemPickUp.gameObject.SetActive(true);
    }

    /// <summary>
    /// <remarks>Public method that needs to be used when the item isn't near the player anymore and needs to deactivate UI.</remarks>
    /// </summary>
    public virtual void DeactivateUI()
    {
        textItemPickUp.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;

        GetItem(player);
        ActivateUI();
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        DeactivateUI();
    }
}
