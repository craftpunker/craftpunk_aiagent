
using System.Collections.Generic;
using System.Linq;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class BuffValue
    {
        public SkillEffectType SkillEffectType;
        public SkillEffectBase SkillEffect;
        public Buff Buff;
        public Fix64 Value;
        public BuffStack BuffStack;

        public void Init(SkillEffectBase skillEffect, Buff buff, Fix64 value, BuffStack buffStack)
        {
            SkillEffectType = skillEffect.SkillEffectData.type;
            SkillEffect = skillEffect;
            Buff = buff;
            Value = value;
            BuffStack = buffStack;
        }

        //BUFF
        public void ChangeBuffValue(Fix64 value, SkillEffectData data, Buff buff)
        {
            SkillEffect.Init();
            SkillEffect.SkillEffectData = data;
            SkillEffect.Start();
            Buff = buff;
            Value = value;
        }

        public void Release()
        {
            Buff = null;
            SkillEffect = null;
            ClassPool.instance.Push(this);
        }
    }

#if _CLIENTLOGIC_
    public class BuffValue2SpecialEffect
    {
        public BuffValue BuffValue;
        public SpecialEffect SpecialEffect;

        public void Init(BuffValue value, SpecialEffect effect)
        {
            BuffValue = value;
            SpecialEffect = effect;
        }

        public void Release()
        {
            BuffValue = null;
            SpecialEffect.BKilled = true;
            SpecialEffect = null;
        }
    }
#endif
    public class BuffBag
    {
        public EntityBase Entity;

        //BuffStack1buff，,
        //skefType:BuffValue
        private Dictionary<SkillEffectType, BuffValue> buffValueDict = new Dictionary<SkillEffectType, BuffValue>();
        //buff
        private List<BuffValue> soleBuffValueList = new List<BuffValue>();

#if _CLIENTLOGIC_
        private BuffValue2SpecialEffect bf2spef;
#endif

        public void Init(EntityBase entity)
        {
            Entity = entity;
        }

        public void PushBuff(int buffCfgid)
        {
            var buff = BuffFactory.instance.CreateBuff(buffCfgid);
            var skillEffectIds = buff.BuffData.skillEffectCfgIds;

            bool killBuff = true;
            foreach (var id in skillEffectIds)
            {
                SkillEffectData skillEffectData = GameData.instance.SkillEffectDict[id];
                bool value = InitBuffValue(skillEffectData, buff);
                if (value)
                    killBuff = false;
            }

            if (killBuff)
                buff.Release();

        }

        public bool InitBuffValue(SkillEffectData skillEffectData, Buff buff)
        {
            //int skefCfgId = skillEffectData.skillEffectCfgId;
            SkillEffectType skefTpye = skillEffectData.type;
            BuffStack buffStack = skillEffectData.skillBuffStack;

            //buffStack = BuffStack.MaxValue;
            //skillEffectData.fix64Args.Add(Fix64.One);

            if (buffStack == BuffStack.MaxValue)
            {
                Fix64 value = skillEffectData.fix64Args[0];
                if (buffValueDict.TryGetValue(skefTpye, out BuffValue bv))
                {
                    if (bv.Value < value)
                    {
                        bv.ChangeBuffValue(value, skillEffectData, buff);
                        return true;
                    }
                }
                else
                {
                    var buffValue = CreateBuffValue(skillEffectData, buff);
                    buffValueDict.Add(skefTpye, buffValue);
                    return true;
                }

                return false;
            }
            else if (buffStack == BuffStack.Sole)
            {
                var buffValue = CreateBuffValue(skillEffectData, buff);
                soleBuffValueList.Add(buffValue);
                return true;
            }

            return false;
        }

        private BuffValue CreateBuffValue(SkillEffectData skillEffectData, Buff buff)
        {
            BuffValue buffValue = ClassPool.instance.Pop<BuffValue>();
            var skillEffect = SkillEffectFactory.instance.CreateSkillEffect(skillEffectData.cfgId, null, null, Entity);
            buffValue.Init(skillEffect, buff, Fix64.Zero, skillEffectData.skillBuffStack);

            return buffValue;
        }

        public void Update()
        {
            Fix64 cumTime = GameData.instance._CumTime;
            for (int i = buffValueDict.Count - 1; i >= 0; i--)
            {
                var kv = buffValueDict.ElementAt(i);
                var buffValue = kv.Value;
                Buff buff = buffValue.Buff;
                //buffskefbuffValue
                if (buff.BKillTime <= cumTime)
                {
                    buffValue.SkillEffect.BKilled = true;

                    if (!buff.BKill)
                    {
                        buff.BKill = true;
                        buff.Release();
                    }

#if _CLIENTLOGIC_
                    if (bf2spef != null && buffValue == bf2spef.BuffValue)
                    {
                        bf2spef.Release();
                        bf2spef = null;
                    }
#endif

                    buffValue.Release();
                    buffValueDict.Remove(kv.Key);
                }
            }

            for (int i = soleBuffValueList.Count - 1; i >= 0; i--)
            {
                BuffValue buffValue = soleBuffValueList[i];
                Buff buff = buffValue.Buff;
                if (buff.BKillTime <= cumTime)
                {
                    buffValue.SkillEffect.BKilled = true;
                    buff.BKill = true;
                    buff.Release();

#if _CLIENTLOGIC_
                    if (bf2spef != null && buffValue == bf2spef.BuffValue)
                    {
                        bf2spef.Release();
                        bf2spef = null;
                    }
#endif
                    buffValue.Release();
                    soleBuffValueList.Remove(buffValue);
                }
            }

#if _CLIENTLOGIC_
            if (bf2spef == null)
            {
                for (int i = buffValueDict.Count - 1; i >= 0; i--)
                {
                    var kv = buffValueDict.ElementAt(i);
                    int animCfgid = kv.Value.SkillEffect.SkillEffectData.animCfgId;
                    if (animCfgid != 0)
                    {
                        bf2spef = CreateB2Sef(kv.Value);
                        return;
                    }
                }

                for (int i = soleBuffValueList.Count - 1; i >= 0; i--)
                {
                    var bv = soleBuffValueList[i];
                    int animCfgid = bv.SkillEffect.SkillEffectData.animCfgId;
                    if (animCfgid != 0)
                    {
                        bf2spef = CreateB2Sef(bv);
                        return;
                    }
                }
            }
#endif
        }

#if _CLIENTLOGIC_
        private BuffValue2SpecialEffect CreateB2Sef(BuffValue bf)
        {
            var bv2spef = ClassPool.instance.Pop<BuffValue2SpecialEffect>();
            var specialEffect = SpecialEffectFactory.instance.CreateSpecialEffect(bf.SkillEffect.SkillEffectData.animCfgId, FixVector3.None, SetSpefParent);
            bv2spef.Init(bf, specialEffect);
            return bv2spef;
        }


        private void SetSpefParent(SpecialEffect effect)
        {
            effect.Trans.parent = Entity.BuffBagTrans;
            effect.Trans.localPosition = Vector3.zero;
        }
#endif

        public void Release()
        {
            foreach (var item in buffValueDict)
            {
                var buffValue = item.Value;
                Buff buff = buffValue.Buff;
                buffValue.SkillEffect.BKilled = true;
                buff.Release();
                buffValue.Release();
            }

            foreach (var buffValue in soleBuffValueList)
            {
                Buff buff = buffValue.Buff;
                buffValue.SkillEffect.BKilled = true;
                buff.Release();
                buffValue.Release();
            }

            buffValueDict.Clear();
            soleBuffValueList.Clear();

            Entity = null;

#if _CLIENTLOGIC_
            bf2spef = null;
#endif
    }
}
}
