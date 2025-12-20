using UnityEngine;

public class Workbench : DrawOutline, IInteractable
{
    public void Interact()
    {
        GlobalData.UIControl.OpenCraftWindowSurv(CraftTable.Workbench);
    }
}
