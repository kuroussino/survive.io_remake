using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemID : Singleton<ItemID>
{
    [SerializeField] List<Sprite> sprites;
    public Sprite GetSprite(int id)
    {
        return sprites[id];
    }
}
