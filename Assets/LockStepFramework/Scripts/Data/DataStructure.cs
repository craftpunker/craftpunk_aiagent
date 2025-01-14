#if _CLIENTLOGIC_
using UnityEngine;

public struct Ind_Vector3
{
    public int I;
    public Vector3 V3;

    public Ind_Vector3(int i, Vector3 v3)
    {
        I = i; 
        V3 = v3;
    }
}
#endif