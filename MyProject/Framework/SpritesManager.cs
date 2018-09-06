using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SpritesManager : Singleton<SpritesManager>
{
    List<Dictionary<string, Sprite>> m_sprites = new List<Dictionary<string, Sprite>>();

    public void StartGame()
    {
        for (int index = 0; index < GamePath.asset.atlas.Length; ++index)
        {
            m_sprites.Add(LoadSprites(GamePath.asset.atlas[index]));            
        }
    }
    public void StopGame() { }
    public Sprite Find(string name)
    {
        for (int index = 0; index < m_sprites.Count; ++index)
        {
            var sprites = m_sprites[index];
            Sprite sprite;
            if (sprites.TryGetValue(name, out sprite)) { return sprite; }
        }
        return null;
    }
    static Dictionary<string, Sprite> LoadSprites(string spriteAtlas)
    {
        Dictionary<string, Sprite> result = new Dictionary<string, Sprite>();
        var sprites = BundleManager.Instance.LoadUnitySprites(spriteAtlas);
        for (int index = 0; index < sprites.Length; ++index)
        {
            var sprite = sprites[index] as Sprite;
            if (sprite != null)
            {
                int splitIndex = sprite.name.LastIndexOf('.');
                if (splitIndex >= 0)
                {
                    result.Add(sprite.name.Substring(0, splitIndex), sprite);
                }
                else { result.Add(sprite.name, sprite); }
            }
        }
        return result;
    }
}