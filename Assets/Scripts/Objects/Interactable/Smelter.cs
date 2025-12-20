using UnityEngine;

public class Smelter : DrawOutline, IInteractable
{
    public void Interact()
    {
        GlobalData.UIControl.OpenCraftWindowSurv(CraftTable.Smelter);
    }
}
