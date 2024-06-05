using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Returns an <c>I_Item</c> object requested by other Classes.
/// <para> It can be a <c>A_Weapon</c> or a <c>A_Support</c></para>
/// </summary>
public class ItemGetter: Singleton<ItemGetter>
{
    #region Variables & Properties

    [SerializeField] List<A_Weapon> weaponList;
    [SerializeField] List<A_Support> suppportList;


    #endregion

    #region Custom Methods

    /// <summary>
    ///  Returns a specified <paramref name"weapon"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public A_Weapon GetWeapon<T>(T weapon) where T : A_Weapon
    {
        foreach(var wep in weaponList)
        {
            if(wep.GetType() == typeof(T))
                return wep;
        }
        return null;
    }
 
    /// <summary>
    /// Returns a specified <paramref name"support"/> item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="support"></param>
    /// <returns></returns>
    public A_Support GetSupport<T>(T support) where T : A_Support
    {
        foreach (var sup in suppportList)
        {
            if (support.GetType() == typeof(T))
                return sup;
        }
        return null;
    }

    /// <summary>
    /// Returns a random item from the <c>A_Weapon</c> Collection.
    /// </summary>
    /// <returns></returns>
    public A_Weapon GetRandomWeapon()
    {
        int indexRandom = Random.Range(0, weaponList.Count);
        return weaponList[indexRandom];
    }

    /// <summary>
    /// Returns a random item from the <c>A_Support</c> Collection.
    /// </summary>
    /// <returns></returns>
    public A_Support GetSupportRandom()
    {
        int indexRandom = Random.Range(0, suppportList.Count);
        return suppportList[indexRandom];
    }

    /// <summary>
    /// Returns a random item from the <c>A_Weapon</c> and <c>A_Support</c> Collections
    /// </summary>
    /// <returns></returns>
    public I_Item GetRandomItem()
    {
        List<I_Item> allItemsList = new List<I_Item>(weaponList);
        allItemsList.AddRange(suppportList);
        return allItemsList[Random.Range(0, allItemsList.Count)];
    }
    #endregion

}
