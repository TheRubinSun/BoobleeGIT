using UnityEngine;

public class Traider : DrawOutline, IInteractable
{
    public void Interact()
    {
        UIControl.Instance.OpenShop();
    }
}
