#if _CLIENTLOGIC_
using System;
using UnityEngine;


public class PhysicsRayMono : MonoBehaviour
{
    public Action<Ray> MouseDownOnUI; //UI
    public Action<Ray> MouseDown; //UI

    public Action<Ray> MouseOnUI; //UI
    public Action<Ray> Mouse; //UI

    public Action MouseUpOnUI; //UI
    public Action MouseUp; //UI


    //public Action<Vector3> OnMouseButtonDown;
    //public Action<Vector3, bool> OnMouseButton; // true:PhysicsRayMono false:UI
    //public Action<bool> OnMouseButtonUp; // true:PhysicsRayMono false:UI

    //private void OnDisable()
    //{
    //    OnMouseButtonDown = null;
    //    OnMouseButton = null;
    //    OnMouseButtonUp = null;
    //}
}
#endif