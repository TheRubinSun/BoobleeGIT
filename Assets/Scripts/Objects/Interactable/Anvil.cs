using UnityEngine;

public class Anvil : DrawOutline, IInteractable
{
    public void Interact()
    {
        UIControl.Instance.OpenCraftWindowSurv(CraftTable.Anvil);
    }
}
