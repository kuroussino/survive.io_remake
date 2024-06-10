using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;

public class ItemID : Singleton<ItemID>
{
    [SerializeField] List<Sprite> sprites;
    [SerializeField] List<A_Weapon> weapons;
    public Sprite GetSpriteItem<T>(int id)
    {
        return GetGenericItem(sprites,id);
    }
    public int GetSpriteID(Sprite sprite)
    {
        return GetGenericID(sprites, sprite);
    }
    public A_Weapon GetWeaponItem<T>(int id)
    {
        return GetGenericItem(weapons, id);
    }
    public int GetWeaponID(A_Weapon sprite)
    {
        return GetGenericID(weapons, sprite);
    }


    T GetGenericItem<T>(List<T> list, int id)
    {
        if (id < 0 && id >= list.Count)
            return default(T);

        return list[id];
    }
    int GetGenericID<T>(List<T> list, T item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (item.Equals(list[i]))
                return i;
        }
        return -1;
    }
}
