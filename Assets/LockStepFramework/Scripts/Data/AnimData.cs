#if _CLIENTLOGIC_
using UnityEngine;

public class AnimData
{
    public int Cfgid;
    public string Prefab;
    public Vector3 PrefabSize;
    //public float Time;
    public float IdleStartTime;
    public float IdleEndTime;
    public float IdleSpeed;
    public float MoveStartTime;
    public float MoveEndTime;
    public float MoveSpeed;
    public float AtkStartTime;
    public float AtkEndTime;
    public float AtkSpeed;
    public float ShadowOffset;

    public float IdleTime;
    public float MoveTime;
    public float AtkTime;
}
#endif
