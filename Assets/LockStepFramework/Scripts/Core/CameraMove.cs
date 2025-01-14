#if _CLIENTLOGIC_
using Battle;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove : MonoSingleton<CameraMove>
{
    private Ray ray;
    private RaycastHit hit;//射线碰到的碰撞信息
    private float defaultZ = -10;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameData.instance.BattleScene == BattleScene.Ready)
        {
            ReadySceneMoveCamera();
        }
        else if (GameData.instance.BattleScene == BattleScene.Battle)
        {
            BattleSceneMoveCamera();
        }
    }

    private void BattleSceneMoveCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
            {
  
            }
        }
        else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonUp(0))
        {

        }
    }

    private void ReadySceneMoveCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
            {

            }
        }
        else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonUp(0))
        {

        }
    }

    private void DrawLine()
    {

    }

    private void DoRay()
    {

    }
}
#endif