using UnityEngine;

public class Workbench : DrawOutline, IInteractable
{
    public void Interact()
    {
        UIControl.Instance.OpenCraftWindow();
    }
}
