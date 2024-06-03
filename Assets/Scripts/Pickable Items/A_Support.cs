using Unity.Netcode;
using UnityEngine;

/// <summary>
/// <para>Base <b>abstract class</b> for support items, like Health Packs, Armor Packs, etc...</para>
/// <remarks> P.S. => For now, i will leave it like this. In case all items have an equip and instant effect, I will change it. </remarks>
/// </summary>
public abstract class A_Support : NetworkBehaviour, I_Item
{
    [Header("Stats and Variables")]
    [Space(6)]
    [Tooltip("Defines whether the item is equippable or has an instant effect")]
    [SerializeField] protected bool isEquippable;

    /// <summary>
    /// Main trigger for checking if a Player has entered the trigger of the GameObject.
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out MonoBehaviour player))
            return;
        ActivateEffect(player);
    }

    /// <summary>
    /// Main effect.
    /// </summary>
    /// <param name="player"></param>
    protected abstract void ActivateEffect(MonoBehaviour player);
}
