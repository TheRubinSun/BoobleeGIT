using UnityEngine;

public class Traider : DrawOutline, IInteractable
{
    public string nameTrader;
    public void Interact()
    {
        UIControl.Instance.OpenShopSurv(nameTrader);
    }
}
