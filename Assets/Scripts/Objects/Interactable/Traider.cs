using UnityEngine;

public class Traider : DrawOutline, IInteractable
{
    public string nameTrader;
    public void Interact()
    {
        GlobalData.UIControl.OpenShopSurv(nameTrader);
    }
}
