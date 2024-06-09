using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemID : Singleton<ItemID>
{
    [SerializeField] List<Sprite> sprites;
    public Sprite GetSprite(int id)
    {
        if (id < 0 && id >= sprites.Count)
            return null;

        return sprites[id];
    }
    public int GetID(Sprite sprite)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprite == sprites[i])
                return i;
        }

        return -1;
    }
}
