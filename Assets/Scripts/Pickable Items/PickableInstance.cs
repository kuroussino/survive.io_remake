using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PickableInstance : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public I_Item item;

    /// <summary>
    /// 
    /// </summary>
    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Input with PlayerInventory
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
    public void SetPrefabToSpawn(I_Item item)
    {
        this.item = item;
    }

    /// <summary>
    /// <remarks>REMEMBER TO CHECK IF ITEM IS NULL!</remarks>
    /// </summary>
    public virtual void ActivateUI()
    {
        if (item == null)
            return;
        var go = item as MonoBehaviour;
        if (go == null)
            return;
        print($"Equip {go.name}");
    }

    /// <summary>
    /// <remarks>REMEMBER TO CHECK IF ITEM IS NULL!</remarks>
    /// </summary>
    public virtual void DeactivateUI()
    {
        if (item == null)
            return;
        var go = item as MonoBehaviour;
        if (go == null)
            return;
        print($"Out of {go.name}");
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
