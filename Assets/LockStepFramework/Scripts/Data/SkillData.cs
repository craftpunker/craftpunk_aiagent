
using Battle;
using System;
using System.Collections.Generic;

[Serializable]
public class SkillData
{
    public int cfgId;
    public SkillType type; //
    public PlayerGroup targetGroup; //
    public List<int> skillEffectCfgIds;
    public List<Fix64> fix64Args;
    public List<string> stringArgs;
    public List<int> animCfgIds;
}

[Serializable]
public class SkillEffectData
{
    public int cfgId;
    public SkillEffectType type;//skilleffect
    public BuffStack skillBuffStack;
    public List<Fix64> fix64Args;
    public List<string> stringArgs;
    public Fix64 frequency;
    public int skillEffectCfgId;
    public int skillBuffCfgId;
    public int entityIndex;
    public int skillCfgId;
    public int animCfgId;
}

[Serializable]
public class BuffData
{
    public int cfgId;
    public Fix64 lifeTime;
    public List<int> skillEffectCfgIds;
}

