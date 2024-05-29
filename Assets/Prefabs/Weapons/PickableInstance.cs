using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PickableInstance : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private I_Item item;

    /// <summary>
    /// 
    /// </summary>
    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 
    /// </summary>
    protected void OnDestroy()
    {
        DeactivateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void SetPrefabToSpawn(I_Item item, Sprite sprite)
    {
        this.item = item;
        spriteRenderer.sprite = sprite;
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
        print($"DeEquip {go.name}");
    }
}
