
public class SkillView : UiBase
{
    U3dObj BtnClose;

    protected override void OnInit()
    {
        base.OnInit();

        BtnClose = Find("ImgBg/BtnClose");
        SetOnClick(BtnClose, OnClose);
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    private void OnClose()
    {
        UiMgr.Close<SkillView>();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }
}
