#if _CLIENTLOGIC_
using Battle;
using UnityEngine;

//ÆåÅÌ¸ñ×Ó
public class BoardMono : MonoBehaviour
{
    public Sprite Black;
    public Sprite White;

    private SpriteRenderer spriteRenderer;
    private int index;
    private FixVector2 pos;
    private int colorValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetData(FixVector2 pos, int colorValue, int index)
    {
        this.pos = pos;
        this.index = index;
        this.colorValue = colorValue;

        var txtLevel = transform.Find("TxtLevel").GetComponent<TextMesh>();
        txtLevel.gameObject.SetActive(false);
        transform.Find("Lock").gameObject.SetActive(false);

        if (GameUtils.IsGridLock(index))
        {
            var gridTable = GameData.instance.TableJsonDict["GridConf"];
            var gridData = gridTable[index.ToString()];
            var unlockLevel = gridData["unlockLevel"];
            var pveTable = GameData.instance.TableJsonDict["PveConf"];
            var pveData = pveTable[unlockLevel.ToString()];
            txtLevel.text = $"{pveData["name"].Value}";
            txtLevel.gameObject.SetActive(true);

            transform.Find("Lock").gameObject.SetActive(true);
        }

         spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = pos.ToVector3();
        SetSpriteRenderer(colorValue);
    }

    private void SetSpriteRenderer(int value)
    {
        //Debug.Log(value);
        spriteRenderer.sprite = value == 0 ? White : Black;
    }
}
#endif