using UnityEngine;

public class TrapLogic : MonoBehaviour 
{

    public virtual void SetParameters()
    {

    }
    public virtual void CreateTrap()
    {

    }
    public virtual void DestroyTrap()
    {

    }
    public virtual void Activate()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision!= null)
        {
            if(collision.CompareTag("Enemy"))
            {
                Debug.Log("Enemy");
                Activate();
            }
        }
    }
}
