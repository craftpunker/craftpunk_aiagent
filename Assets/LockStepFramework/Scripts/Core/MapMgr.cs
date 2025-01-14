#if _CLIENTLOGIC_

using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class MapMgr : Singleton<MapMgr>
    {
        private SpriteRenderer leftSR;
        private SpriteRenderer rightSR;

        public Dictionary<string, Sprite> MapDict = new Dictionary<string, Sprite>();
        private List<string> mapNames = new List<string>()
        {
            "Scene_right_1.1", "Scene_right_1.2", "Scene_right_1.3", "Scene_right_1.4", "Scene_right_2", "Scene_right_4"
        };

        public void Init()
        {
            foreach (var mapName in mapNames) 
            {
                ResMgr.instance.LoadAssetAsync<Sprite>(mapName, (sprite) =>
                {
                    MapDict.Add(mapName, sprite);
                });
            }

            leftSR = GameObject.Find("World/Map/Left").GetComponent<SpriteRenderer>();
            rightSR = GameObject.Find("World/Map/Right").GetComponent<SpriteRenderer>();
        }

        //lr=0 ，lr=1 
        public void ChangeMap(int lr, string mapName)
        {
            if (MapDict.TryGetValue(mapName, out Sprite sprite))
            {
                if (lr == 0)
                {
                    leftSR.sprite = sprite;
                }
                else
                {
                    rightSR.sprite = sprite;
                }
            }
        }

    }
}

#endif