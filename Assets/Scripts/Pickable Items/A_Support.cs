using Unity.Netcode;
using UnityEngine;

/// <summary>
/// <para>Base <b>abstract class</b> for support items, like Health Packs, Armor Packs, etc...</para>
/// <remarks> P.S. => For now, i will leave it like this. In case all items have an equip and instant effect, I will change it./n </remarks>
/// <para><b> This type of <c>I_Item</c> is not instantiable, it just serves as a prefab to refer when indicating some data. </b></para>
/// </summary>
public abstract class A_Support : MonoBehaviour, I_Item
{
    [Header("Stats and Variables")]
    [Space(6)]
    [Tooltip("Defines whether the item is equippable or has an instant effect")]
    [SerializeField] protected bool isEquippable;
    [SerializeField] protected Sprite supportSprite;

    /// <summary>
    /// Main trigger for checking if a Player has entered the trigger of the GameObject.
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player))
            return;
        ActivateEffect(player);
    }

    /// <summary>
    /// Main effect.
    /// </summary>
    /// <param name="player"></param>
    protected abstract void ActivateEffect(Player player);

    public Sprite GetSpriteItem()
    {
        return supportSprite;
    }
}
