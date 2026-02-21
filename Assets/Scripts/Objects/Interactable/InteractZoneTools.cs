using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface IInteractable
{
    public void Interact();
}
public class DrawOutline : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material mat;

    [SerializeField] private float outlineSize = 1f;

    [SerializeField] private GameObject e_icon;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mat = sr.material;
    }
    public void DrawOutlineObj()
    {
        mat.SetFloat("_OutlineSize", outlineSize);
        if (e_icon != null) e_icon.SetActive(true);
    }
    public void EarseOutlineObj()
    {
        mat.SetFloat("_OutlineSize", 0);
        if(e_icon!=null) e_icon.SetActive(false);
    }
}
public class InteractZoneTools : MonoBehaviour
{
    public static InteractZoneTools instance;
    private Collider2D playerCol;
    private LayerMask interactiveLayer =>
    (1 << LayerManager.interactableLayer) | (1 << LayerManager.interactableTriggerLayer);

    //private IInteractable old_interactable;
    private List<IInteractable> interactablesInRange = new();
    public IInteractable cur_interactable;

    private float interval = 1f / 6f;
    private float timer = 0f;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        playerCol = GetComponent<Collider2D>();
    }
    private void Update()
    {
        if (cur_interactable != null && Input.GetKeyDown(KeyCode.E))
        {
            cur_interactable.Interact();
        }


        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer -= interval;
        }
    }
    private void OpenTools()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & interactiveLayer) == 0) return;

        if(collision.TryGetComponent(out IInteractable interactable))
        {
            if(!interactablesInRange.Contains(interactable)) //Если новый в обзоре, то добовляем в лист
                interactablesInRange.Add(interactable);

            if (cur_interactable != null && cur_interactable is DrawOutline outlineOld) //Если не пустой то старый стираем выделение
                outlineOld.EarseOutlineObj();

            cur_interactable = interactable;
            if(interactable is DrawOutline outlineObject)
                outlineObject.DrawOutlineObj();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & interactiveLayer) == 0) return;

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if(interactablesInRange.Contains(interactable)) //Удаляем из списка активных объектов
                interactablesInRange.Remove(interactable);

            if(interactable == cur_interactable) //Если вышел текущий выделенный объект то
            {
                if(interactable is DrawOutline outline) //Удаляем выделение с объекта
                    outline.EarseOutlineObj();

                cur_interactable = null;

                if(interactablesInRange.Count > 0)//Если в списке есть еще другие объекты, то
                {
                    cur_interactable = interactablesInRange[^1]; 

                    if(cur_interactable is DrawOutline outlineNew) //Назначем последний из них текущим и выделяем
                        outlineNew.DrawOutlineObj();
                }
            }
        }
    }
}
