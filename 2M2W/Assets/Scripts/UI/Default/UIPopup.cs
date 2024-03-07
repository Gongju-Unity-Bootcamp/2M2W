public class UIPopup : UIBase
{
    public override void Init()
    {
        base.Init();

        Managers.UI.SetCanvas(gameObject);
    }

    public virtual void ClosePopupUI()
        => Managers.UI.ClosePopupUI(this);
}
