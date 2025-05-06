using UnityEngine;

public class Smelter : DrawOutline, IInteractable
{
    public void Interact()
    {
        UIControl.Instance.OpenCraftWindowSurv(CraftTable.Smelter);
    }
}
