#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public enum BuffStack
    {
        None = 0,
        MaxValue = 1, //cfgidͬʱֻ����һ��,ȡ���ֵ
        Sole = 2, //����
    }

    public class BuffFactory : Singleton<BuffFactory>
    {
        public Buff CreateBuff(int cfgid, params object[] args)
        {
            if (GameData.instance.BuffDict.TryGetValue(cfgid, out BuffData buffData))
            {
                //BuffData buffData = GameData.instance.BuffDict[cfgid];
                Buff buff = ClassPool.instance.Pop<Buff>();

                buff.Init();
                buff.BuffData = buffData;
                buff.Start();
                return buff;
            }
            return null;
        }
    }
}
