using UnityEngine;

public class DirtBed : DrawOutline, IInteractable
{
    public int ID {  get; set; }
    public bool Busy { get; set; }
    public void Interact()
    {
        return;
    }
}
