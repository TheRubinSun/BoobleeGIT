using UnityEngine;

public class Alchemy_station : DrawOutline, IInteractable
{
    public void Interact()
    {
        UIControl.Instance.OpenCraftWindowSurv(CraftTable.Alchemy_Station);
    }
}
