using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Enter your class explanation here
/// </summary>
public class ItemGetter: Singleton<ItemGetter>
{

    #region Variables & Properties

    [SerializeField] List<GameObject> weaponList;
    [SerializeField] List<GameObject> suppportList;
    [SerializeField] List<ItemReference> itemReferences;

    #endregion

    #region Custom Methods

    public I_Item GetItem()
    {
        return itemReferences[0].value;
    }
    public GameObject GetWeapon<T>(T weapon) where T : A_Weapon
    {
        foreach(var wep in weaponList)
        {
            if(wep.GetType() == typeof(T))
                return wep;
        }
        return null;
    }

    public GameObject GetSupport<T>(T support) where T : A_Support
    {
        foreach (var sup in weaponList)
        {
            if (support.GetType() == typeof(T))
                return sup;
        }
        return null;
    }

    public GameObject GetRandomWeapon()
    {
        int indexRandom = Random.Range(0, weaponList.Count);
        return weaponList[indexRandom];
    }

    public GameObject GetSupportRandom()
    {
        int indexRandom = Random.Range(0, suppportList.Count);
        return suppportList[indexRandom];
    }

    public GameObject GetRandomItem()
    {
        List<GameObject> allItemsList = weaponList;
        allItemsList.AddRange(suppportList);
        return allItemsList[Random.Range(0, allItemsList.Count)];
    }
    #endregion

}
