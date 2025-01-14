
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class Buff : IRelease
    {
        public BuffData BuffData;
        public EntityBase Target; //BUFF
        public Fix64 BKillTime; //
        public bool BKill = false;

        public void Init()
        {
            BKillTime = Fix64.Zero;
            BKill = false;
        }

        public void Start()
        {
            BKillTime = GameData.instance._CumTime + BuffData.lifeTime;
        }

        public void Release()
        {
            BuffData = null;
            Target = null;
            ClassPool.instance.Push(this);
        }
    }
}
