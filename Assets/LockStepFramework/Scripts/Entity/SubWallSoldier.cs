//90

#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class SubWallSoldier : EntityBase
    {
        public WallSoldier MainWallSoldier;

        public override void Release()
        {
            MainWallSoldier = null;
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}


