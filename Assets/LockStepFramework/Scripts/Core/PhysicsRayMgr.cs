#if _CLIENTLOGIC_
using Battle;
using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

//UI
public class PhysicsRayMgr : MonoSingleton<PhysicsRayMgr>
{
    public Action<Ray> MouseDownOnUI; //UI
    public Action<Ray> MouseDown; //UI

    public Action<Ray> MouseOnUI; //UI
    public Action<Ray> Mouse; //UI

    public Action MouseUpOnUI; //UI
    public Action MouseUp; //UI

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                MouseDown?.Invoke(ray);
            }
            else
            {
                MouseDownOnUI?.Invoke(ray);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Mouse?.Invoke(ray);
            }
            else
            {
                MouseOnUI?.Invoke(ray);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                MouseUp?.Invoke();
            }
            else
            {
                MouseUpOnUI?.Invoke();
            }
        }
    }
}
#endif