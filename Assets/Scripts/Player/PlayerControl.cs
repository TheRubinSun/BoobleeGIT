using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour 
{
    Player player;
    //Объекты игрока
    [SerializeField] GameObject playerObj;
    [SerializeField] Transform cameraObj;
    [SerializeField] GameObject hand;
    [SerializeField] Transform centerLegs;
    [SerializeField] Transform[] legsLines;
    [SerializeField] Transform[] legsCenter;
    [SerializeField] Transform WeaponSlots;
    [SerializeField] Transform MinionsSlots;

    [SerializeField] private float radiusCenterLegs = 0.4f;                // Радиус области

    private LineRenderer lineRenderer;

    //Напрвеления и радиусы
    [SerializeField] private Vector2 inputDirection;


    private Vector2 mousePos;
    private Vector2 movement;
    //Vector2 mousePos;
    private Vector2 direction;

    private Dictionary<int, WeaponControl> weaponsAndArms;
    private Dictionary<int, MinionControl> minionSlots;
    private float rangeMinion;

    //Звуки
    AudioSource audioSource_Move;

    [SerializeField] AudioClip audioClips;

    void Start()
    {
        player = GetComponentInParent<Player>();
        //lineRenderer = MinionsSlots.GetComponent<LineRenderer>();
        //rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        
        cameraObj.position = new Vector3(transform.position.x, transform.position.y, -10f);
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");
        if (inputDirection.x != 0 || inputDirection.y != 0) Move();
        RotateWeaponSlots();

        if(Input.GetMouseButton(0))
        {
            Attack();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            UseMinions();
        }


    }
    public void UpdateSlots(Dictionary<int, WeaponControl> weaponObj, Dictionary<int, MinionControl> minionObj)
    {
        weaponsAndArms = weaponObj;
        minionSlots = minionObj;
    }
    void Attack()
    {
        if (weaponsAndArms == null) return;
        foreach (WeaponControl child in weaponsAndArms.Values)
        {
            if (child != null) //Проверка, если вдруг оружие было удаленно
            {
                child.Attack();
            }
            
        }
    }
    void UseMinions()
    {
        if (minionSlots == null) return;
        foreach(MinionControl minion in minionSlots.Values)
        {
            if (minion != null)
            {
                minion.UseMinion();
            }
        }
    }
    void RotateWeaponSlots()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        WeaponSlots.rotation = Quaternion.Euler(0, 0, -angle);
    }
    public void Move()
    {
        //movement = inputDirection.normalized * Speed * Time.fixedDeltaTime;
        transform.position += (Vector3)inputDirection.normalized * player.Mov_Speed * Time.deltaTime;
        //rb.MovePosition(rb.position + movement);

        foreach (Transform legsLine in legsLines)
        {
            legsLine.GetComponent<LineControle>().AnimMove();
        }
        foreach (Transform legCenter in legsCenter)
        {
            legCenter.GetComponent<LegControl>().CheckPos();
        }
        MoveCenterLegs(inputDirection);
    }
    void MoveCenterLegs(Vector2 inputDirection)
    {
        // Рассчитываем смещение для центра ног
        Vector2 currentPos = centerLegs.localPosition;
        Vector2 newPos = currentPos + inputDirection.normalized * 10f * Time.deltaTime;


        if (newPos.magnitude > radiusCenterLegs)
        {
            centerLegs.localPosition = newPos.normalized * radiusCenterLegs;
        }
        else
        {
            centerLegs.localPosition = newPos;
        }
    }
    //public void DrawRange(bool action, float range)
    //{
    //    if (action)
    //    {
    //        DrawCircle(range);
    //        lineRenderer.enabled = true;
    //    }
    //    else
    //    {
    //        lineRenderer.enabled = false;
    //    }
    //}
    //void DrawCircle(float range)
    //{
    //    int segments = 50; // Количество точек круга
    //    Vector2 center = MinionsSlots.position;
    //    float angleStep = 360f / segments;
    //    for (int i = 0; i <= segments; i++)
    //    {
    //        float angle = Mathf.Deg2Rad * i * angleStep;
    //        float x = center.x + Mathf.Cos(angle) * range;
    //        float y = center.y + Mathf.Sin(angle) * range;
    //        lineRenderer.SetPosition(i, new Vector3(x, y, 0));
    //    }
    //}
    //void GetFirstMinion()
    //{

    //}
}
