
using Battle;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimEditor : MonoBehaviour
{
    public Dropdown DropdownRole;
    public Dropdown DropdownAnim;
    public InputField InputField;

    private MaterialPropertyBlock Block;
    public GameObject Go;
    public Renderer Mesh;

    private float AnimSpeed;

    private float time;
    private int startFrame;
    private int endFrame;
    private float startTime;
    private float endTime;

    // Start is called before the first frame update
    private Dictionary<string, animJson> animDict = new Dictionary<string, animJson>();

    void Start()
    {
        Block = new MaterialPropertyBlock();
        Mesh = Go.GetComponent<Renderer>();
        Mesh.GetPropertyBlock(Block);
        time = 0;

        var v3 = Mesh.material.GetVector("_Sequence");
        //GameData.instance.FrameRow = (int)v3.x;
        //GameData.instance.FrameColumn = (int)v3.y;
        //GameData.instance.FrameColumnStep = 1f / GameData.instance.FrameColumn;

        var datas = GameData.instance.TableJsonDict["AnimConf"];
        DropdownRole.ClearOptions();
        foreach (var d in datas)
        {
            animJson animData = new animJson();
            animData.idleAnims = d.Value["idleAnims"].ToString();
            animData.moveAnims = d.Value["moveAnims"].ToString();
            animData.atkAnims = d.Value["atkAnims"].ToString();
            animData.idleSpeed = d.Value["idleSpeed"];
            animDict.Add(d.Key, animData);
            DropdownRole.options.Add(new Dropdown.OptionData(d.Key));
        }

        var key = DropdownRole.options[0].text;
        var data = animDict[key];
        AnimSpeed = data.idleSpeed / 1000;
        InputField.onEndEdit.AddListener((value) =>
        {
            AnimSpeed = float.Parse(value);
        });

        DropdownRole.onValueChanged.AddListener(DropdownRoleChange);
        DropdownRoleChange(0);;

    } 

    private void DropdownRoleChange(int i)
    {
        var key = DropdownRole.options[i].text;
        var animData = animDict[key];
        DropdownAnim.ClearOptions();
        DropdownAnim.options.Add(new Dropdown.OptionData(animData.idleAnims));
        DropdownAnim.options.Add(new Dropdown.OptionData(animData.moveAnims));
        DropdownAnim.options.Add(new Dropdown.OptionData(animData.atkAnims));

        DropdownAnim.onValueChanged.RemoveAllListeners();
        DropdownAnim.onValueChanged.AddListener(DropdownAnimChange);

        DropdownAnimChange(0);

        //DropdownAnim.value = 0;
        DropdownAnim.RefreshShownValue();
    }


    private void DropdownAnimChange(int i)
    {
        var value = DropdownAnim.options[i].text;
        var datas = JSONNode.Parse(value);
        startFrame = datas[0];
        endFrame = datas[1];

        //startFrame = startFrame == 0 ? 0 : startFrame - 1;
        endFrame += 1;

        //var startRow = startFrame / GameData.instance.FrameColumn;
        //var startCol = startFrame % GameData.instance.FrameColumn;

        //var endRow = endFrame / GameData.instance.FrameColumn;
        //var endCol = endFrame % GameData.instance.FrameColumn;

        //var startColStep = startCol * GameData.instance.FrameColumnStep;
        //var endColStep = endCol * GameData.instance.FrameColumnStep;

        //startTime = startRow + startColStep;
        //endTime = endRow + endColStep;

        time = startTime;
    }

    public void SetMaterialFloat(string name, float value)
    { 
        Block.SetFloat(name, value);
        Mesh.SetPropertyBlock(Block);
    }

    // Update is called once per frame
    void Update()
    {
        SetMaterialFloat("_FrameTime", time);
        time += Time.deltaTime * AnimSpeed;

        if (time > endTime)
        {
            time = startTime;
        }
    }
}
