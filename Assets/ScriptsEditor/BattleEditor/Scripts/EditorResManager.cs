using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BattleEditor
{
    public class EditorResManager : MonoBehaviour
    {
        public static EditorResManager Instance;
        
        private Dictionary<string, SoliderData> soliderDataDict = new Dictionary<string, SoliderData>();
        private Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
        private Dictionary<string, BattleSoliderData> battleSoliderDataDict = new Dictionary<string, BattleSoliderData>();

        public void Init()
        {
            Instance = this;
            InitSoliderData();
            InitBattleSoliderData();
            InitPVEData();
        }

        private void InitBattleSoliderData()
        {
            var datas = GameData.instance.TableJsonDict["SoliderBattleConf"];
            battleSoliderDataDict.Clear();
            foreach (var d in datas)
            {
                BattleSoliderData soliderData = new BattleSoliderData();
                soliderData.cfgId = d.Value["cfgId"].AsInt;
                //troopsData.name = d.Value["name"].ToString();
                soliderData.name = "t0";
                //troopsData.model = d.Value["model"].ToString();
                soliderData.model = "GPUEntity";
                soliderData.level = d.Value["level"].AsInt;
                soliderData.radius = (Fix64)d.Value["radius"].AsInt;
                soliderData.moveSpeed = (Fix64)d.Value["moveSpeed"].AsInt;
                soliderData.maxHp = (Fix64)d.Value["maxHp"].AsInt;
                soliderData.atk = (Fix64)d.Value["atk"].AsInt;
                soliderData.count = d.Value["count"];
                soliderData.atkSpeed = (Fix64)d.Value["atkSpeed"].AsInt;
                soliderData.atkRange = (Fix64)d.Value["atkRange"].AsInt;
                soliderData.inAtkRange = (Fix64)d.Value["inAtkRange"].AsInt;
                soliderData.soldierType = d.Value["soldierType"].AsInt;
                soliderData.atkSkillShowPos = GameUtils.JsonPosToV2(d.Value["atkSkillShowPos"]);
                soliderData.atkSkillId = d.Value["atkSkillCfgId"].AsInt;
                soliderData.center = GameUtils.JsonPosToV2(d.Value["center"]);
                soliderData.atkReadyTime = d.Value["atkReadyTime"].AsInt;
                soliderData.deadSkillId = d.Value["deadSkillCfgId"].AsInt;
                soliderData.bornSkillId = d.Value["bornSkillCfgId"].AsInt;
                battleSoliderDataDict.Add(d.Key, soliderData);
            }
            Debug.Log(soliderDataDict.Count);
        }

        public Dictionary<string, BattleSoliderData> GetAllBattleSoliderData()
        {
            return battleSoliderDataDict;
        }
        public BattleSoliderData GetBattleSoliderData(int cfgid, int level)
        {
            return battleSoliderDataDict.FirstOrDefault(t => t.Value.cfgId == cfgid && t.Value.level == level).Value;
        }

        private void InitPVEData()
        {
             
        }

        private void InitSoliderData()
        {
            var datas = GameData.instance.TableJsonDict["SoliderConf"];
            soliderDataDict.Clear();
            foreach (var d in datas)
            {
                SoliderData soliderData = new SoliderData();
                soliderData.index = d.Value["index"];
                soliderData.cfgId = d.Value["cfgId"];
                soliderData.race = d.Value["race"];
                soliderData.name = d.Value["name"];
                soliderData.model = d.Value["model"];
                soliderData.languageNameID = d.Value["languageNameID"];
                soliderData.desc = d.Value["desc"];
                soliderData.icon = d.Value["icon"];
                soliderData.soliderBattleCfgId = d.Value["soliderBattleCfgId"];
                soliderData.cost = d.Value["cost"].AsInt;
                soliderData.maximum = d.Value["maximum"].AsInt;
                soliderData.showAttribute = "";

                soliderDataDict.Add(d.Key, soliderData);

            }
            Debug.Log(soliderDataDict.Count);
        }

        public Dictionary<string, SoliderData> GetAllBaseSoliderData()
        {
            return soliderDataDict;
        }
        public SoliderData GetBaseSoliderData(int cfgid, int level)
        {
            return soliderDataDict[cfgid.ToString()];
        }

        public Sprite GetSprite(string name)
        {
            Sprite sprite;
            if (!spriteDict.TryGetValue(name, out sprite))
            {
                string filePath = "Assets/ScriptsEditor/BattleEditor/Res/BEIcon/" + name + ".png";
                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D texture = new Texture2D(512, 512);
                texture.LoadImage(fileData);

                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            return sprite;
        }

    }
}
