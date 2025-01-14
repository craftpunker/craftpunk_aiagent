
#if _CLIENTLOGIC_
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class SpecialEffectFactory : Singleton<SpecialEffectFactory>
    {
        public SpecialEffect CreateSpecialEffect(List<int> animCfgids, int index, FixVector3 pos, Action<SpecialEffect> callBack = null, params object[] args)
        {
            if (animCfgids.Count - 1 < index)
                return null;

            int animCfgid = animCfgids[index];

            return GetSpecialEffect(animCfgid, pos, callBack);
        }

        public SpecialEffect CreateSpecialEffect(int animCfgid, FixVector3 pos, Action<SpecialEffect> callBack = null, params object[] args)
        {
            return GetSpecialEffect(animCfgid, pos, callBack);
        }

        private SpecialEffect GetSpecialEffect(int animCfgid, FixVector3 pos, Action<SpecialEffect> callBack)
        {
            SpecialEffect effect = ClassPool.instance.Pop<SpecialEffect>();
            var animData = AnimMgr.instance.GetAnimData(animCfgid);
            effect.Init();
            effect.AnimData = animData;

            effect.CreatePrefabCallBack = callBack;
            if (pos != FixVector3.None)
            {
                effect.Fixv3LastPosition = pos;
                effect.Fixv3LogicPosition = pos;
            }
            //effect.IsUI = isUi;
            effect.Start();

            BattleMgr.instance.SpecialEffectList.Add(effect);

            return effect;
        }

        //public SpecialEffect CreateUISpecialEffect(int animCfgid, FixVector3 pos, Action<Transform> callBack)
        //{
        //    SpecialEffect effect = ClassPool.instance.Pop<SpecialEffect>();
        //    var animData = AnimMgr.instance.GetAnimData(animCfgid);

        //    effect.Init();
        //    effect.AnimData = animData;
        //    effect.CreatePrefabCallBack = callBack;

        //    if (pos != FixVector3.None)
        //    {
        //        effect.Fixv3LastPosition = pos;
        //        effect.Fixv3LogicPosition = pos;
        //    }
        //    effect.Start();

        //    BattleMgr.instance.SpecialEffectList.Add(effect);

        //    return effect;
        //}
    }
}
#endif