using UnityEngine;

/// <summary>
/// <b>Base abstract class</b> for support items, like Health Packs, Armor Packs, etc...
/// </summary>
public abstract class A_Support : MonoBehaviour, I_Item
{
    [SerializeField] private bool isEquippable;
}
