#if _CLIENTLOGIC_
using Battle;
using System.Collections.Generic;
using UnityEngine;

public class BoardMgr : Singleton<BoardMgr>
{
    private Dictionary<int, BoardMono> boardMonos = new Dictionary<int, BoardMono>();

    public void Init()
    {

    }

    public void ShowBoard(bool value)
    {
        if (value)
        {
            var globalConf = GameData.instance.TableJsonDict["GlobalConf"];
            var gridConf = GameData.instance.TableJsonDict["GridConf"];

            var boardCount = gridConf.Count;
            //int rows = globalConf["gridPerRow"]["intValue"];
            //int cols = boardCount / rows;

            int cols = globalConf["gridPerRow"]["intValue"];
            int rows = boardCount / cols;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int index = row * cols + col + 1;
                    var data = gridConf[index.ToString()];
                    FixVector2 pos = GameUtils.JsonPosToV2(data["center"]);
                    int i = col;
                    int j = row;
                    ResMgr.instance.LoadGameObjectAsync("Board", (go) =>
                    {
                        var mono = go.GetComponent<BoardMono>();
                        int colorValue = 1;
                        if ((i + j) % 2 == 0)
                            colorValue = 0;

                        go.name = index.ToString();
                        mono.SetData(pos, colorValue, index);
                        boardMonos.Add(index, mono);
                    });
                }
            }
        }
        else
        {
            foreach (var item in boardMonos)
            {
                ResMgr.instance.ReleaseGameObject(item.Value.gameObject);
            }

            boardMonos.Clear();
        }
    }

    public void Release()
    {
        boardMonos.Clear();
    }
}
#endif