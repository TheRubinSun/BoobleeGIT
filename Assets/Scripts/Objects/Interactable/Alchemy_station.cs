using UnityEngine;

public class Alchemy_station : DrawOutline, IInteractable
{
    public void Interact()
    {
        GlobalData.UIControl.OpenCraftWindowSurv(CraftTable.Alchemy_Station);
    }
}
