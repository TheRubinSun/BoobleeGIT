using UnityEngine;

public class Anvil : DrawOutline, IInteractable
{
    public void Interact()
    {
        GlobalData.UIControl.OpenCraftWindowSurv(CraftTable.Anvil);
    }
}
