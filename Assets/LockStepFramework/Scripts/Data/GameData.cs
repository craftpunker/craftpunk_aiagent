
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;

#if _CLIENTLOGIC_
using UnityEngine;

#endif
namespace Battle
{
    public enum PlayerGroup
    {
        None = -1,
        Player = 0, 
        Enemy = 1,
    }

    public enum BattleScene
    {
        None = 0,
        Loading = 0,
        Ready = 1, 
        Battle = 2,
        End = 3,
    }

    public class GameData : Singleton<GameData>
    {
        public JSONObject PlayerBattleJsonObj;
        public JSONObject EnemyBattleJsonObj;

        public JSONObject PlayerBattleJsonObjTemp; //，


        public string FirstPlayerBattleJson = "{\"data\":{\"uid\":10203,\"battleInfo\":{\"score\":0,\"map\":{\"areaSize\":[13500,4500,-11000],\"cfgId\":0},\"winRate\":0,\"skills\":{\"1001410\":{\"stringArgs\":{},\"cfgId\":1001410,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1},\"7310301\":{\"stringArgs\":{},\"cfgId\":7310301,\"type\":7,\"animCfgIds\":[30008,30009],\"skillEffectCfgIds\":[7110301],\"intArgs\":[1400,-1000,2000,2000,3000],\"targetGroup\":1},\"1000310\":{\"stringArgs\":{},\"cfgId\":1000310,\"type\":3,\"animCfgIds\":{},\"skillEffectCfgIds\":[10002000],\"intArgs\":[1200,-1000,1000],\"targetGroup\":1},\"7410401\":{\"stringArgs\":{},\"cfgId\":7410401,\"type\":6,\"animCfgIds\":[30012,30013,30014],\"skillEffectCfgIds\":[7110401],\"intArgs\":[1250,-1000,16000,1000],\"targetGroup\":1},\"1000300\":{\"stringArgs\":{},\"cfgId\":1000300,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[30001000],\"intArgs\":[0],\"targetGroup\":0},\"7430101\":{\"stringArgs\":{},\"cfgId\":7430101,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[25003000],\"intArgs\":[1000],\"targetGroup\":1},\"1000510\":{\"stringArgs\":{},\"cfgId\":1000510,\"type\":8,\"animCfgIds\":[30027],\"skillEffectCfgIds\":[10001000],\"intArgs\":[15000],\"targetGroup\":1},\"7110101\":{\"stringArgs\":{},\"cfgId\":7110101,\"type\":6,\"animCfgIds\":[30020,0,30021],\"skillEffectCfgIds\":[7110101],\"intArgs\":[2000,-1000,30000,20000],\"targetGroup\":1},\"1000210\":{\"stringArgs\":{},\"cfgId\":1000210,\"type\":2,\"animCfgIds\":[30002],\"skillEffectCfgIds\":[10001000],\"intArgs\":[12000,7500,300,1000],\"targetGroup\":1},\"1000110\":{\"stringArgs\":{},\"cfgId\":1000110,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1}},\"skillBuffs\":{},\"summons\":{},\"name\":\"EvronFenor\",\"cards\":{\"7410401\":{\"cfgId\":7410401,\"soldierJob\":[[0]],\"cost\":5000,\"range\":2200,\"skillCfgId\":7410401,\"targetType\":1,\"pos\":3},\"7110101\":{\"cfgId\":7110101,\"soldierJob\":[[0]],\"cost\":4000,\"range\":3400,\"skillCfgId\":7110101,\"targetType\":1,\"pos\":1},\"7310301\":{\"cfgId\":7310301,\"soldierJob\":[[0]],\"cost\":5000,\"range\":2500,\"skillCfgId\":7310301,\"targetType\":1,\"pos\":2},\"7430101\":{\"cfgId\":7430101,\"soldierJob\":[[0]],\"cost\":7000,\"range\":2500,\"skillCfgId\":7430101,\"targetType\":0,\"pos\":4}},\"costInit\":0,\"uid\":10320,\"maxTick\":300,\"skillEffects\":{\"7110301\":{\"skillBuffCfgId\":0,\"cfgId\":7110301,\"type\":10007,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[500000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"7110401\":{\"skillBuffCfgId\":0,\"cfgId\":7110401,\"type\":10007,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[1500000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"25003000\":{\"skillBuffCfgId\":0,\"cfgId\":25003000,\"type\":25003,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[2000,1500,3000,2000,500],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"30001000\":{\"skillBuffCfgId\":0,\"cfgId\":30001000,\"type\":30001,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[9999000],\"animCfgId\":0,\"skillBuffStack\":1,\"entityIndex\":0},\"7110101\":{\"skillBuffCfgId\":0,\"cfgId\":7110101,\"type\":10007,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[300000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"10002000\":{\"skillBuffCfgId\":0,\"cfgId\":10002000,\"type\":10002,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":10001000,\"frequency\":0,\"args\":[1200,1600,400],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"10001000\":{\"skillBuffCfgId\":0,\"cfgId\":10001000,\"type\":10001,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[1000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0}},\"troops\":{\"10616\":{\"cfgId\":10014,\"level\":25,\"id\":10616,\"pos\":[-9140,3400],\"name\":\"Wolf\",\"atkSkillCfgId\":1001410,\"animCfgId\":100140,\"deadSkillCfgId\":0,\"grid\":3,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":1700000,\"radius\":110,\"atk\":206000,\"atkReadyTime\":200,\"atkSpeed\":300,\"moveSpeed\":4860,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10618\":{\"cfgId\":10002,\"level\":24,\"id\":10618,\"pos\":[-9140,-6680],\"name\":\"Archer\",\"atkSkillCfgId\":1000210,\"animCfgId\":100020,\"deadSkillCfgId\":0,\"grid\":18,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":1188000,\"radius\":271,\"atk\":330000,\"atkReadyTime\":200,\"atkSpeed\":1200,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":2,\"count\":9,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10559\":{\"cfgId\":10001,\"level\":24,\"id\":10559,\"pos\":[-6860,3400],\"name\":\"Footman\",\"atkSkillCfgId\":1000110,\"animCfgId\":100010,\"deadSkillCfgId\":0,\"grid\":4,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":5280000,\"radius\":270,\"atk\":330000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10625\":{\"cfgId\":10002,\"level\":23,\"id\":10625,\"pos\":[-11420,-6680],\"name\":\"Archer\",\"atkSkillCfgId\":1000210,\"animCfgId\":100020,\"deadSkillCfgId\":0,\"grid\":17,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":1152000,\"radius\":271,\"atk\":320000,\"atkReadyTime\":200,\"atkSpeed\":1200,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":2,\"count\":9,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10614\":{\"cfgId\":10005,\"level\":20,\"id\":10614,\"pos\":[-6860,40],\"name\":\"Crossbowman\",\"atkSkillCfgId\":1000510,\"animCfgId\":100050,\"deadSkillCfgId\":0,\"grid\":9,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":2175000,\"radius\":270,\"atk\":1078000,\"atkReadyTime\":200,\"atkSpeed\":1800,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":12000,\"soldierType\":2,\"count\":6,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10556\":{\"cfgId\":10001,\"level\":27,\"id\":10556,\"pos\":[-6860,-3320],\"name\":\"Footman\",\"atkSkillCfgId\":1000110,\"animCfgId\":100010,\"deadSkillCfgId\":0,\"grid\":14,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":5760000,\"radius\":270,\"atk\":360000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10624\":{\"cfgId\":10002,\"level\":23,\"id\":10624,\"pos\":[-11420,-3320],\"name\":\"Archer\",\"atkSkillCfgId\":1000210,\"animCfgId\":100020,\"deadSkillCfgId\":0,\"grid\":12,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":1152000,\"radius\":271,\"atk\":320000,\"atkReadyTime\":200,\"atkSpeed\":1200,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":2,\"count\":9,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10626\":{\"cfgId\":10011,\"level\":20,\"id\":10626,\"pos\":[-4580,40],\"name\":\"Guardian\",\"atkSkillCfgId\":1001100,\"animCfgId\":100110,\"deadSkillCfgId\":0,\"grid\":10,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":8700000,\"radius\":270,\"atk\":870000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":6,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10627\":{\"cfgId\":10011,\"level\":19,\"id\":10627,\"pos\":[-4580,-6680],\"name\":\"Guardian\",\"atkSkillCfgId\":1001100,\"animCfgId\":100110,\"deadSkillCfgId\":0,\"grid\":20,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":8400000,\"radius\":270,\"atk\":840000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":6,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10558\":{\"cfgId\":10001,\"level\":24,\"id\":10558,\"pos\":[-6860,-10040],\"name\":\"Footman\",\"atkSkillCfgId\":1000110,\"animCfgId\":100010,\"deadSkillCfgId\":0,\"grid\":24,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":5280000,\"radius\":270,\"atk\":330000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10619\":{\"cfgId\":10002,\"level\":25,\"id\":10619,\"pos\":[-9140,40],\"name\":\"Archer\",\"atkSkillCfgId\":1000210,\"animCfgId\":100020,\"deadSkillCfgId\":0,\"grid\":8,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":1224000,\"radius\":271,\"atk\":340000,\"atkReadyTime\":200,\"atkSpeed\":1200,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":2,\"count\":9,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10557\":{\"cfgId\":10001,\"level\":26,\"id\":10557,\"pos\":[-9140,-3320],\"name\":\"Footman\",\"atkSkillCfgId\":1000110,\"animCfgId\":100010,\"deadSkillCfgId\":0,\"grid\":13,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":5600000,\"radius\":270,\"atk\":350000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10620\":{\"cfgId\":10002,\"level\":24,\"id\":10620,\"pos\":[-11420,40],\"name\":\"Archer\",\"atkSkillCfgId\":1000210,\"animCfgId\":100020,\"deadSkillCfgId\":0,\"grid\":7,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":1188000,\"radius\":271,\"atk\":330000,\"atkReadyTime\":200,\"atkSpeed\":1200,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":2,\"count\":9,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10613\":{\"cfgId\":10005,\"level\":20,\"id\":10613,\"pos\":[-6860,-6680],\"name\":\"Crossbowman\",\"atkSkillCfgId\":1000510,\"animCfgId\":100050,\"deadSkillCfgId\":0,\"grid\":19,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":2175000,\"radius\":270,\"atk\":1078000,\"atkReadyTime\":200,\"atkSpeed\":1800,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":12000,\"soldierType\":2,\"count\":6,\"job\":2,\"atkSkillShowPos\":[-100,250]},\"10533\":{\"cfgId\":10003,\"level\":20,\"id\":10533,\"pos\":[-4580,-3320],\"name\":\"Barbarian\",\"atkSkillCfgId\":1000310,\"animCfgId\":100030,\"deadSkillCfgId\":0,\"grid\":15,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1000300,\"maxHp\":32250000,\"radius\":675,\"atk\":853000,\"atkReadyTime\":660,\"atkSpeed\":1700,\"moveSpeed\":2000,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":1250,\"soldierType\":0,\"count\":1,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10617\":{\"cfgId\":10014,\"level\":24,\"id\":10617,\"pos\":[-9140,-10040],\"name\":\"Wolf\",\"atkSkillCfgId\":1001410,\"animCfgId\":100140,\"deadSkillCfgId\":0,\"grid\":23,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":1650000,\"radius\":110,\"atk\":200000,\"atkReadyTime\":200,\"atkSpeed\":300,\"moveSpeed\":4860,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]}},\"power\":50950,\"avatar\":\"\",\"costRecover\":1000,\"costMax\":10000}}}";
        public string FirstEnemyBattleJson = "{\"data\":{\"battleType\":\"guideBattle\",\"enemyBattleInfo\":{\"score\":0,\"map\":{\"areaSize\":[13500,4500,-11000],\"cfgId\":0},\"winRate\":0,\"skills\":{\"7310301\":{\"stringArgs\":{},\"cfgId\":7310301,\"type\":7,\"animCfgIds\":[30008,30009],\"skillEffectCfgIds\":[7110301],\"intArgs\":[1400,-1000,2000,2000,3000],\"targetGroup\":1},\"1000710\":{\"stringArgs\":{},\"cfgId\":1000710,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1},\"1000411\":{\"stringArgs\":{},\"cfgId\":1000411,\"type\":3,\"animCfgIds\":[30003],\"skillEffectCfgIds\":[10001005],\"intArgs\":[1200,-1000,1000],\"targetGroup\":1},\"1000610\":{\"stringArgs\":{},\"cfgId\":1000610,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1},\"1000300\":{\"stringArgs\":{},\"cfgId\":1000300,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[30001000],\"intArgs\":[0],\"targetGroup\":0},\"7430101\":{\"stringArgs\":{},\"cfgId\":7430101,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[25003000],\"intArgs\":[1000],\"targetGroup\":1},\"1000600\":{\"stringArgs\":{},\"cfgId\":1000600,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[25001000],\"intArgs\":[0],\"targetGroup\":0},\"1001410\":{\"stringArgs\":{},\"cfgId\":1001410,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1},\"1001200\":{\"stringArgs\":{},\"cfgId\":1001200,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[25002001],\"intArgs\":[0],\"targetGroup\":0},\"1000310\":{\"stringArgs\":{},\"cfgId\":1000310,\"type\":3,\"animCfgIds\":{},\"skillEffectCfgIds\":[10002000],\"intArgs\":[1200,-1000,1000],\"targetGroup\":1},\"1000910\":{\"stringArgs\":{},\"cfgId\":1000910,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1},\"1001010\":{\"stringArgs\":{},\"cfgId\":1001010,\"type\":2,\"animCfgIds\":[30002],\"skillEffectCfgIds\":[10001000],\"intArgs\":[12000,7500,300,1000],\"targetGroup\":1},\"1001210\":{\"stringArgs\":{},\"cfgId\":1001210,\"type\":1,\"animCfgIds\":{},\"skillEffectCfgIds\":[10001000],\"intArgs\":[1000],\"targetGroup\":1},\"1000810\":{\"stringArgs\":{},\"cfgId\":1000810,\"type\":2,\"animCfgIds\":[30002],\"skillEffectCfgIds\":[10001000],\"intArgs\":[12000,7500,300,1000],\"targetGroup\":1},\"7110101\":{\"stringArgs\":{},\"cfgId\":7110101,\"type\":6,\"animCfgIds\":[30020,0,30021],\"skillEffectCfgIds\":[7110101],\"intArgs\":[2000,-1000,30000,20000],\"targetGroup\":1},\"1000410\":{\"stringArgs\":{},\"cfgId\":1000410,\"type\":8,\"animCfgIds\":[30026],\"skillEffectCfgIds\":[10001004],\"intArgs\":[8000],\"targetGroup\":1},\"7410401\":{\"stringArgs\":{},\"cfgId\":7410401,\"type\":6,\"animCfgIds\":[30012,30013,30014],\"skillEffectCfgIds\":[7110401],\"intArgs\":[1250,-1000,16000,1000],\"targetGroup\":1}},\"skillBuffs\":{\"10100\":{\"lifeTime\":5000,\"cfgId\":10100,\"skillEffectCfgIds\":[10004000]}},\"summons\":{},\"name\":\"EvronFenor\",\"cards\":{\"7410401\":{\"cfgId\":7410401,\"soldierJob\":[[0]],\"cost\":5000,\"range\":2200,\"skillCfgId\":7410401,\"targetType\":1,\"pos\":3},\"7110101\":{\"cfgId\":7110101,\"soldierJob\":[[0]],\"cost\":4000,\"range\":3400,\"skillCfgId\":7110101,\"targetType\":1,\"pos\":1},\"7310301\":{\"cfgId\":7310301,\"soldierJob\":[[0]],\"cost\":5000,\"range\":2500,\"skillCfgId\":7310301,\"targetType\":1,\"pos\":2},\"7430101\":{\"cfgId\":7430101,\"soldierJob\":[[0]],\"cost\":7000,\"range\":2500,\"skillCfgId\":7430101,\"targetType\":0,\"pos\":4}},\"costInit\":0,\"uid\":10320,\"maxTick\":300,\"skillEffects\":{\"7110301\":{\"skillBuffCfgId\":0,\"cfgId\":7110301,\"type\":10007,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[500000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"10004000\":{\"skillBuffCfgId\":0,\"cfgId\":10004000,\"type\":10004,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[9000],\"animCfgId\":0,\"skillBuffStack\":1,\"entityIndex\":0},\"10001000\":{\"skillBuffCfgId\":0,\"cfgId\":10001000,\"type\":10001,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[1000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"25003000\":{\"skillBuffCfgId\":0,\"cfgId\":25003000,\"type\":25003,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[2000,1500,3000,2000,500],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"25001000\":{\"skillBuffCfgId\":0,\"cfgId\":25001000,\"type\":25001,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[2000,2000,20000],\"animCfgId\":30024,\"skillBuffStack\":0,\"entityIndex\":0},\"10002000\":{\"skillBuffCfgId\":0,\"cfgId\":10002000,\"type\":10002,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":10001000,\"frequency\":0,\"args\":[1200,1600,400],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"25002001\":{\"skillBuffCfgId\":0,\"cfgId\":25002001,\"type\":25002,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":10003000,\"frequency\":0,\"args\":{},\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"10001005\":{\"skillBuffCfgId\":0,\"cfgId\":10001005,\"type\":10001,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[1000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"10003000\":{\"skillBuffCfgId\":10100,\"cfgId\":10003000,\"type\":10003,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":{},\"animCfgId\":30004,\"skillBuffStack\":0,\"entityIndex\":0},\"10001004\":{\"skillBuffCfgId\":0,\"cfgId\":10001004,\"type\":10001,\"skillCfgId\":1000411,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[0],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"7110101\":{\"skillBuffCfgId\":0,\"cfgId\":7110101,\"type\":10007,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[300000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"7110401\":{\"skillBuffCfgId\":0,\"cfgId\":7110401,\"type\":10007,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":0,\"args\":[1500000],\"animCfgId\":0,\"skillBuffStack\":0,\"entityIndex\":0},\"30001000\":{\"skillBuffCfgId\":0,\"cfgId\":30001000,\"type\":30001,\"skillCfgId\":0,\"stringArgs\":{},\"skillEffectCfgId\":0,\"frequency\":-1000,\"args\":[9999000],\"animCfgId\":0,\"skillBuffStack\":1,\"entityIndex\":0}},\"troops\":{\"10608\":{\"cfgId\":10010,\"level\":50,\"id\":10608,\"pos\":[-9140,-3320],\"name\":\"Elf Archer\",\"atkSkillCfgId\":1001010,\"animCfgId\":100100,\"deadSkillCfgId\":0,\"grid\":13,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":1416000,\"radius\":271,\"atk\":708000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":9,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10607\":{\"cfgId\":10010,\"level\":50,\"id\":10607,\"pos\":[-9140,3400],\"name\":\"Elf Archer\",\"atkSkillCfgId\":1001010,\"animCfgId\":100100,\"deadSkillCfgId\":0,\"grid\":3,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":1416000,\"radius\":271,\"atk\":708000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":9,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10566\":{\"cfgId\":10008,\"level\":50,\"id\":10566,\"pos\":[-9140,-6680],\"name\":\"Bonebow\",\"atkSkillCfgId\":1000810,\"animCfgId\":100080,\"deadSkillCfgId\":0,\"grid\":18,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":3186000,\"radius\":271,\"atk\":472000,\"atkReadyTime\":200,\"atkSpeed\":1100,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":9,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10568\":{\"cfgId\":10008,\"level\":50,\"id\":10568,\"pos\":[-9140,40],\"name\":\"Bonebow\",\"atkSkillCfgId\":1000810,\"animCfgId\":100080,\"deadSkillCfgId\":0,\"grid\":8,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":3186000,\"radius\":271,\"atk\":472000,\"atkReadyTime\":200,\"atkSpeed\":1100,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":9,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10574\":{\"cfgId\":10014,\"level\":40,\"id\":10574,\"pos\":[-6860,-6680],\"name\":\"Wolf\",\"atkSkillCfgId\":1001410,\"animCfgId\":100140,\"deadSkillCfgId\":0,\"grid\":19,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":2450000,\"radius\":110,\"atk\":296000,\"atkReadyTime\":200,\"atkSpeed\":300,\"moveSpeed\":4860,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10533\":{\"cfgId\":10003,\"level\":40,\"id\":10533,\"pos\":[-4580,-3320],\"name\":\"Barbarian\",\"atkSkillCfgId\":1000310,\"animCfgId\":100030,\"deadSkillCfgId\":0,\"grid\":15,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1000300,\"maxHp\":62250000,\"radius\":675,\"atk\":1593000,\"atkReadyTime\":660,\"atkSpeed\":1700,\"moveSpeed\":2000,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":1250,\"soldierType\":0,\"count\":1,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10628\":{\"cfgId\":10012,\"level\":30,\"id\":10628,\"pos\":[-13700,40],\"name\":\"Assassin\",\"atkSkillCfgId\":1001210,\"animCfgId\":100120,\"deadSkillCfgId\":0,\"grid\":6,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1001200,\"maxHp\":2381000,\"radius\":270,\"atk\":1176000,\"atkReadyTime\":200,\"atkSpeed\":1000,\"moveSpeed\":4320,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":6,\"job\":5,\"atkSkillShowPos\":[0,0]},\"10565\":{\"cfgId\":10004,\"level\":40,\"id\":10565,\"pos\":[-11420,-6680],\"name\":\"Fire Baller\",\"atkSkillCfgId\":1000410,\"animCfgId\":100040,\"deadSkillCfgId\":0,\"grid\":17,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":8800000,\"radius\":271,\"atk\":2940000,\"atkReadyTime\":200,\"atkSpeed\":1800,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":1,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10563\":{\"cfgId\":10006,\"level\":30,\"id\":10563,\"pos\":[-11420,-10040],\"name\":\"Cavalry\",\"atkSkillCfgId\":1000610,\"animCfgId\":100060,\"deadSkillCfgId\":0,\"grid\":22,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1000600,\"maxHp\":13260000,\"radius\":450,\"atk\":980000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":5400,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":1200,\"soldierType\":0,\"count\":6,\"job\":3,\"atkSkillShowPos\":[0,0]},\"10630\":{\"cfgId\":10009,\"level\":50,\"id\":10630,\"pos\":[-6860,3400],\"name\":\"Elf Blade\",\"atkSkillCfgId\":1000910,\"animCfgId\":100090,\"deadSkillCfgId\":0,\"grid\":4,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":7552000,\"radius\":270,\"atk\":708000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10567\":{\"cfgId\":10010,\"level\":50,\"id\":10567,\"pos\":[-9140,-10040],\"name\":\"Elf Archer\",\"atkSkillCfgId\":1001010,\"animCfgId\":100100,\"deadSkillCfgId\":0,\"grid\":23,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":1416000,\"radius\":271,\"atk\":708000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":9,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10562\":{\"cfgId\":10014,\"level\":40,\"id\":10562,\"pos\":[-6860,40],\"name\":\"Wolf\",\"atkSkillCfgId\":1001410,\"animCfgId\":100140,\"deadSkillCfgId\":0,\"grid\":9,\"inAtkRange\":0,\"prefab\":\"GPUEntity_a\",\"bornSkillCfgId\":0,\"maxHp\":2450000,\"radius\":110,\"atk\":296000,\"atkReadyTime\":200,\"atkSpeed\":300,\"moveSpeed\":4860,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10633\":{\"cfgId\":10007,\"level\":50,\"id\":10633,\"pos\":[-4580,-6680],\"name\":\"Boneguard\",\"atkSkillCfgId\":1000710,\"animCfgId\":100070,\"deadSkillCfgId\":0,\"grid\":20,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":11328000,\"radius\":270,\"atk\":472000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10629\":{\"cfgId\":10012,\"level\":30,\"id\":10629,\"pos\":[-13700,-6680],\"name\":\"Assassin\",\"atkSkillCfgId\":1001210,\"animCfgId\":100120,\"deadSkillCfgId\":0,\"grid\":16,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1001200,\"maxHp\":2381000,\"radius\":270,\"atk\":1176000,\"atkReadyTime\":200,\"atkSpeed\":1000,\"moveSpeed\":4320,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":6,\"job\":5,\"atkSkillShowPos\":[0,0]},\"10564\":{\"cfgId\":10006,\"level\":30,\"id\":10564,\"pos\":[-11420,3400],\"name\":\"Cavalry\",\"atkSkillCfgId\":1000610,\"animCfgId\":100060,\"deadSkillCfgId\":0,\"grid\":2,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1000600,\"maxHp\":13260000,\"radius\":450,\"atk\":980000,\"atkReadyTime\":200,\"atkSpeed\":1500,\"moveSpeed\":5400,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":1200,\"soldierType\":0,\"count\":6,\"job\":3,\"atkSkillShowPos\":[0,0]},\"10631\":{\"cfgId\":10009,\"level\":50,\"id\":10631,\"pos\":[-6860,-10040],\"name\":\"Elf Blade\",\"atkSkillCfgId\":1000910,\"animCfgId\":100090,\"deadSkillCfgId\":0,\"grid\":24,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":7552000,\"radius\":270,\"atk\":708000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10602\":{\"cfgId\":10004,\"level\":40,\"id\":10602,\"pos\":[-11420,40],\"name\":\"Fire Baller\",\"atkSkillCfgId\":1000410,\"animCfgId\":100040,\"deadSkillCfgId\":0,\"grid\":7,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":8800000,\"radius\":271,\"atk\":2940000,\"atkReadyTime\":200,\"atkSpeed\":1800,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":12000,\"soldierType\":0,\"count\":1,\"job\":2,\"atkSkillShowPos\":[0,0]},\"10598\":{\"cfgId\":10007,\"level\":50,\"id\":10598,\"pos\":[-4580,40],\"name\":\"Boneguard\",\"atkSkillCfgId\":1000710,\"animCfgId\":100070,\"deadSkillCfgId\":0,\"grid\":10,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":11328000,\"radius\":270,\"atk\":472000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]},\"10576\":{\"cfgId\":10012,\"level\":30,\"id\":10576,\"pos\":[-11420,-3320],\"name\":\"Assassin\",\"atkSkillCfgId\":1001210,\"animCfgId\":100120,\"deadSkillCfgId\":0,\"grid\":12,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":1001200,\"maxHp\":2381000,\"radius\":270,\"atk\":1176000,\"atkReadyTime\":200,\"atkSpeed\":1000,\"moveSpeed\":4320,\"center\":[0,0],\"atkBackswing\":500,\"atkRange\":900,\"soldierType\":0,\"count\":6,\"job\":5,\"atkSkillShowPos\":[0,0]},\"10632\":{\"cfgId\":10009,\"level\":50,\"id\":10632,\"pos\":[-6860,-3320],\"name\":\"Caesar\",\"atkSkillCfgId\":1000910,\"animCfgId\":100090,\"deadSkillCfgId\":0,\"grid\":14,\"inAtkRange\":0,\"prefab\":\"GPUEntity\",\"bornSkillCfgId\":0,\"maxHp\":7552000,\"radius\":270,\"atk\":708000,\"atkReadyTime\":200,\"atkSpeed\":1400,\"moveSpeed\":3200,\"center\":[0,0],\"atkBackswing\":150,\"atkRange\":900,\"soldierType\":0,\"count\":9,\"job\":1,\"atkSkillShowPos\":[0,0]}},\"power\":126250,\"avatar\":\"\",\"costRecover\":1000,\"costMax\":10000}}}";

        public Fix64 _FixFrameLen = (Fix64)0.1;
        public Fix64 _CumTime;
        public int _UGameLogicFrame;
        public int EndStep; //

        public bool IsOpenBlood;
        //public int _MaxGameLogicFrame; //

        //Z
        public FixVector3 _Up = new FixVector3(0, 1, 0);
        public FixVector3 _Right = new FixVector3(1, 0, 0);
        public FixVector3 _Hight = new FixVector3(0, 0, -1);

        public Fix64 HexagonOutWidthd2 = (Fix64)0.75f; // / 2
        public Fix64 TroosLine = (Fix64)3; //
        public Fix64 TroosColumn = (Fix64)3; //
        public Fix64 EntityInterval = (Fix64)0.7;
       // public Fix64 MapMaxX = new Fix64(15);
        public int MaxNeighbors = 5; //
        public FixVector3 AreaSize; //
        public Fix64 MaxFlagRange = (Fix64)2; //

        public float RightMapHight = 0; //
        //3400;40;-3320;-6680;-10040
        public List<Fix64> gridYs = new List<Fix64>() {(Fix64)3400, (Fix64)40, -(Fix64)3320, -(Fix64)6680, -(Fix64)10040 };
        //public Fix64 visionDistance;

        //------------------------------
        //public int FrameRow = 5; //
        //public int FrameColumn = 5;//
        //public float FrameColumnStep; // 1 / FrameColumn
        //--------------------------------------------
        public BattleData PlayerBattleData;
        public BattleData EnemyBattleData;
        public Dictionary<int, OpFrame> OpFramesDict = new Dictionary<int, OpFrame>(); //
        public string DiscordPlayerId;
        public string DiscordPlayerName;
        public string DiscordIconURL;
        //public Sprite DiscordIcon;
        //---------------------------------------------
        public Dictionary<string, JSONNode> TableJsonDict = new Dictionary<string, JSONNode>();

        //public Dictionary<>
        //-----------------:-------------------------
        public JSONObject UserData;
        public JSONObject PermanentData;
        public JSONObject DailyData;
        public JSONObject WeeklyData;
        public JSONObject MailsData;
        public JSONObject RecordsData;
        public int ServerTime; //
        public float UerrLoginTime; //

        public int OppsId; //ID
        //public int CurDeploy; //
        //--------------------------------------

        //public Fix64 timeStep_ = (Fix64)0.1;

        public SRandom sRandom = new SRandom();

        //public List<EntityBase> SoldierList = new List<EntityBase>();

        // ---------------------------------------------------------------------------
        public SortedDictionary<int, SkillData> SkillDict = new SortedDictionary<int, SkillData>();
        public SortedDictionary<int, SkillEffectData> SkillEffectDict = new SortedDictionary<int, SkillEffectData>();
        public SortedDictionary<int, BuffData> BuffDict = new SortedDictionary<int, BuffData>();
        public SortedDictionary<int, CardData> CardDict = new SortedDictionary<int, CardData>();
        public SortedDictionary<int, TroopsData> SummonDict = new SortedDictionary<int, TroopsData>();
#if _CLIENTLOGIC_
        public Dictionary<int, TroopsMono> TroopsMonos = new Dictionary<int, TroopsMono>();
        //public Dictionary<int, TroopRangeMono> TroopRangeMono = new Dictionary<int, TroopRangeMono>();

        public Color CorpRed = new Color(1, 0, 0, 0.3f);
        public Color CorpWhite = new Color(1, 1, 1, 0.3f);
#endif
        public BattleScene BattleScene;

        //---------------------
        public static string GPUEntity = "GPUEntity";
        public static string GPUEffect = "GPUEffect";
        public static float LoadingValue = 0;

        //-------------UI----------------
#if _CLIENTLOGIC_
        public TroopsMono SelectedTroop;
#endif

        public List<string> StepMd5 = new List<string>();
        //public List<EntityBase> DragSoldierList = new List<EntityBase>(); //

        public void Init()
        {
            UserData = new JSONObject();
            PermanentData = new JSONObject();
            DailyData = new JSONObject();
            WeeklyData = new JSONObject();
            MailsData = new JSONObject();
            RecordsData = new JSONObject();

            StepMd5.Clear();
        }

        public BattleData DeserializeObjectPlayerBattleData(JSONObject jsonObj)
        {
            BattleData battleData = ClassPool.instance.Pop<BattleData>();
            battleData.Release();

            if (jsonObj == null)
                return battleData;

            JSONNode JsNode = null;

            if (jsonObj["data"] != null)
            {
                JsNode = jsonObj["data"]["battleInfo"];

                if (JsNode == null)
                    JsNode = jsonObj["data"]["enemyBattleInfo"];
            }
            else
            {
                JsNode = jsonObj;
            }

            if (JsNode == null)
                return null;

            //var JsNode = JSONNode.Parse(json);
            //var JsNode = jsonObj["data"]["battleInfo"];

            //if (JsNode == null)
            //    JsNode = jsonObj["data"]["enemyBattleInfo"];

            battleData.uid = jsonObj["uid"].AsInt;
            battleData.name = JsNode["name"];
            battleData.winRate = JsNode["winRate"].AsInt;
            battleData.avatar = JsNode["avatar"];
            battleData.score = JsNode["score"].AsInt;
            battleData.costInit = new Fix64(JsNode["costInit"].AsInt) / 1000;
            battleData.costMax = new Fix64(JsNode["costMax"].AsInt) / 1000;
            battleData.costRecover = new Fix64(JsNode["costRecover"].AsInt) / 1000;
            battleData.maxTick = new Fix64(JsNode["maxTick"]);

            var mapData = JsNode["map"];
            //map\":{\"areaSize\":\"[12500,4500,-14000]\",\"cfgId\":0
            battleData.mapData = ClassPool.instance.Pop<MapData>();
            battleData.mapData.cfgId = mapData["cfgId"];
            battleData.mapData.areaSize = GameUtils.JsonPosToV3(mapData["areaSize"]);
            var troops = JsNode["troops"];
            foreach (var troop in troops)
            {
                var troopsData = ClassPool.instance.Pop<TroopsData>();
                troopsData.cfgId = troop.Value["cfgId"].AsInt;
                troopsData.id = troop.Value["id"].AsInt;
                troopsData.name = troop.Value["name"];
                troopsData.prefab = troop.Value["prefab"];
                troopsData.level = troop.Value["level"].AsInt;
                troopsData.radius = new Fix64(troop.Value["radius"].AsInt) / 1000;
                troopsData.moveSpeed = new Fix64(troop.Value["moveSpeed"].AsInt) / 1000;
                troopsData.maxHp = new Fix64(troop.Value["maxHp"].AsInt) / 1000;
                troopsData.atk = new Fix64(troop.Value["atk"].AsInt) / 1000;
                troopsData.count = troop.Value["count"].AsInt;
                troopsData.atkSpeed = new Fix64(troop.Value["atkSpeed"].AsInt) / 1000;
                troopsData.atkRange = new Fix64(troop.Value["atkRange"].AsInt) / 1000;
                troopsData.inAtkRange = new Fix64(troop.Value["inAtkRange"].AsInt) / 1000;
                troopsData.soldierType = troop.Value["soldierType"].AsInt;
                troopsData.atkSkillShowPos = GameUtils.JsonPosToV3(troop.Value["atkSkillShowPos"]);
                troopsData.atkSkillCfgId = troop.Value["atkSkillCfgId"].AsInt;
                troopsData.center = GameUtils.JsonPosToV3(troop.Value["center"]);
                troopsData.atkReadyTime = new Fix64(troop.Value["atkReadyTime"].AsInt) / 1000;
                troopsData.atkBackswing = new Fix64(troop.Value["atkBackswing"].AsInt) / 1000;
                troopsData.deadSkillCfgId = troop.Value["deadSkillCfgId"].AsInt;
                troopsData.bornSkillCfgId = troop.Value["bornSkillCfgId"].AsInt;
                troopsData.pos = GameUtils.JsonPosToV3(troop.Value["pos"]);
                troopsData.animCfgId = troop.Value["animCfgId"].AsInt;
                troopsData.freeSeekDis = new Fix64(7);//new Fix64(troop.Value["freeSeekDis"].AsInt) / 1000;
                troopsData.grid = troop.Value["grid"].AsInt;
                troopsData.Job = troop.Value["job"].AsInt;
                troopsData.SoldierFlagId = GameUtils.GetId();

                battleData.troops.Add(troopsData.id, troopsData);
            }

            var skills = JsNode["skills"];
            foreach (var skill in skills)
            {
                int cfgId = skill.Value["cfgId"].AsInt;
                if (!SkillDict.ContainsKey(cfgId))
                {
                    var skillData = ClassPool.instance.Pop<SkillData>();
                    skillData.cfgId = cfgId;
                    skillData.type = (SkillType)skill.Value["type"].AsInt;
                    skillData.targetGroup = (PlayerGroup)skill.Value["targetGroup"].AsInt;
                    skillData.skillEffectCfgIds = GameUtils.JsonToIds(skill.Value["skillEffectCfgIds"]);
                    skillData.fix64Args = GameUtils.JsonToFix64s(skill.Value["intArgs"]);
                    skillData.stringArgs = GameUtils.JsonToStrs(skill.Value["stringArgs"]);
                    skillData.animCfgIds = GameUtils.JsonToIds(skill.Value["animCfgIds"]);
                    SkillDict.Add(cfgId, skillData);
                }
            }

            var skillEffects = JsNode["skillEffects"];
            foreach (var skillEffect in skillEffects)
            {
                int cfgId = skillEffect.Value["cfgId"].AsInt;
                if (!SkillEffectDict.ContainsKey(cfgId))
                {
                    var skillEffectData = ClassPool.instance.Pop<SkillEffectData>();
                    skillEffectData.cfgId = cfgId;
                    skillEffectData.type = (SkillEffectType)skillEffect.Value["type"].AsInt;
                    skillEffectData.skillBuffStack = (BuffStack)skillEffect.Value["skillBuffStack"].AsInt;
                    skillEffectData.fix64Args = GameUtils.JsonToFix64s(skillEffect.Value["args"]);
                    skillEffectData.frequency = new Fix64(skillEffect.Value["frequency"].AsInt);
                    skillEffectData.skillEffectCfgId = skillEffect.Value["skillEffectCfgId"].AsInt;
                    skillEffectData.stringArgs = GameUtils.JsonToStrs(skillEffect.Value["stringArgs"]);
                    skillEffectData.skillBuffCfgId = skillEffect.Value["skillBuffCfgId"].AsInt;
                    skillEffectData.entityIndex = skillEffect.Value["entityIndex"].AsInt;
                    skillEffectData.skillCfgId = skillEffect.Value["skillCfgId"].AsInt;
                    skillEffectData.animCfgId = skillEffect.Value["animCfgId"].AsInt;
                    SkillEffectDict.Add(cfgId, skillEffectData);
                }
            }

            var buffs = JsNode["skillBuffs"];
            foreach (var buff in buffs)
            {
                int cfgId = buff.Value["cfgId"].AsInt;
                if (!BuffDict.ContainsKey(cfgId))
                {
                    var buffData = ClassPool.instance.Pop<BuffData>();
                    buffData.cfgId = buff.Value["cfgId"].AsInt;
                    buffData.lifeTime = new Fix64(buff.Value["lifeTime"].AsInt) / 1000;
                    buffData.skillEffectCfgIds = GameUtils.JsonToIds(buff.Value["skillEffectCfgIds"]);
                    BuffDict.Add(cfgId, buffData);
                }
            }

            var cards = JsNode["cards"];
            foreach (var card in cards)
            {
                int cfgId = card.Value["cfgId"].AsInt;
                var cardData = ClassPool.instance.Pop<CardData>();
                cardData.CfgId = card.Value["cfgId"].AsInt;
                cardData.Name = card.Value["name"].ToString();
                cardData.Cost = new Fix64(card.Value["cost"].AsInt) / 1000;
                cardData.SoldierJob = GameUtils.JsonToIds(card.Value["soldierJob"]);
                cardData.TargetType = card.Value["targetType"].AsInt;
                cardData.Range = new Fix64(card.Value["range"].AsInt) / 1000;
                cardData.skillCfgId = card.Value["skillCfgId"].AsInt;
                battleData.cards.Add(cfgId, cardData);

                if (!CardDict.ContainsKey(cfgId))
                {
                    CardDict.Add(cfgId, cardData);
                }
            }

            var summons = JsNode["summons"];
            foreach (var troop in summons)
            {
                var troopsData = ClassPool.instance.Pop<TroopsData>();
                troopsData.cfgId = troop.Value["cfgId"].AsInt;
                troopsData.id = troop.Value["id"].AsInt;
                troopsData.name = troop.Value["name"];
                troopsData.prefab = troop.Value["prefab"];
                troopsData.level = troop.Value["level"].AsInt;
                troopsData.radius = new Fix64(troop.Value["radius"].AsInt) / 1000;
                troopsData.moveSpeed = new Fix64(troop.Value["moveSpeed"].AsInt) / 1000;
                troopsData.maxHp = new Fix64(troop.Value["maxHp"].AsInt) / 1000;
                troopsData.atk = new Fix64(troop.Value["atk"].AsInt) / 1000;
                troopsData.count = troop.Value["count"].AsInt;
                troopsData.atkSpeed = new Fix64(troop.Value["atkSpeed"].AsInt) / 1000;
                troopsData.atkRange = new Fix64(troop.Value["atkRange"].AsInt) / 1000;
                troopsData.inAtkRange = new Fix64(troop.Value["inAtkRange"].AsInt) / 1000;
                troopsData.soldierType = troop.Value["soldierType"].AsInt;
                troopsData.atkSkillShowPos = GameUtils.JsonPosToV3(troop.Value["atkSkillShowPos"]);
                troopsData.atkSkillCfgId = troop.Value["atkSkillCfgId"].AsInt;
                troopsData.center = GameUtils.JsonPosToV3(troop.Value["center"]);
                troopsData.atkReadyTime = new Fix64(troop.Value["atkReadyTime"].AsInt) / 1000;
                troopsData.atkBackswing = new Fix64(troop.Value["atkBackswing"].AsInt) / 1000;
                troopsData.deadSkillCfgId = troop.Value["deadSkillCfgId"].AsInt;
                troopsData.bornSkillCfgId = troop.Value["bornSkillCfgId"].AsInt;
                troopsData.pos = GameUtils.JsonPosToV3(troop.Value["pos"]);
                troopsData.animCfgId = troop.Value["animCfgId"].AsInt;
                troopsData.freeSeekDis = new Fix64(7);//new Fix64(troop.Value["freeSeekDis"].AsInt) / 1000;
                //troopsData.SoldierFlagId = GameUtils.GetId();
                int entityIndex = int.Parse(troop.Key);
                battleData.summons.Add(entityIndex, troopsData);

                if (!SummonDict.ContainsKey(entityIndex))
                {
                    SummonDict.Add(entityIndex, troopsData);
                }
            }

            return battleData;
        }

        public void DeserializeObjectOpFrames(JSONNode opFramesJsonNode)
        {
            foreach (var item in opFramesJsonNode)
            {
                var jsonnode = item.Value;
                int frame = jsonnode["frame"];
                string x = jsonnode["x"];
                string y = jsonnode["y"];
                int cardSkillId = jsonnode["cardSkillId"];

                DebugUtils.Log($"{frame}, {x}, {y}");

                OpFrame op = ClassPool.instance.Pop<OpFrame>();
                op.Frame = frame;
                op.X = x;
                op.Y = y;
                op.CardSkillId = cardSkillId;

                OpFramesDict.Add(frame, op);
            }
        }


        public void ReleaseSkillData()
        {
            foreach (var item in SkillDict)
            {
                item.Value.skillEffectCfgIds.Clear();
                item.Value.fix64Args.Clear();
                item.Value.stringArgs.Clear();
                item.Value.animCfgIds.Clear();
                ClassPool.instance.Push(item.Value);
            }

            foreach (var item in SkillEffectDict)
            {
                item.Value.fix64Args.Clear();
                item.Value.stringArgs.Clear();
                ClassPool.instance.Push(item.Value);
            }

            foreach (var item in BuffDict)
            {
                item.Value.skillEffectCfgIds.Clear();
                ClassPool.instance.Push(item.Value);
            }

            foreach (var item in CardDict)
            {
                ClassPool.instance.Push(item.Value);
            }

            foreach (var item in SummonDict)
            {
                ClassPool.instance.Push(item.Value);
            }

            SkillDict.Clear();
            SkillEffectDict.Clear();
            BuffDict.Clear();
            CardDict.Clear();
            SummonDict.Clear();
            //StepMd5.Clear();
        }

        public void Release()
        {
            EndStep = 0;
            StepMd5.Clear();

            foreach (var item in OpFramesDict)
            {
                var op = item.Value;
                ClassPool.instance.Push(op);
            }

            OpFramesDict.Clear();

#if _CLIENTLOGIC_
            for (int i = TroopsMonos.Count - 1; i >= 0; i--)
            {
                var troop = TroopsMonos.ElementAt(i);
                troop.Value.RemoveEntitys();
            }

            TroopsMonos.Clear();
#endif
            //ReleaseSkillData();
            //PlayerData = null;
        }
    }
}

