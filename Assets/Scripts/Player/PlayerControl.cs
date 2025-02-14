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

    [SerializeField] private float radiusCenterLegs = 0.4f;                // Радиус области
    
    
    //Напрвеления и радиусы
    [SerializeField] private Vector2 inputDirection;
    
    Vector2 mousePos;
    Vector2 movement;
    //Vector2 mousePos;
    Vector2 direction;
    void Start()
    {
        player = GetComponentInParent<Player>();

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
        

        //MoveHand();
    }

    void Attack()
    {
        Dictionary<int, WeaponControl> weaponsAndArms = Player.Instance.GetDictWeaponAndArms();
        foreach (WeaponControl child in weaponsAndArms.Values)
        {
            if (child != null) //Проверка, если вдруг оружие было удаленно
            {
                child.Attack();
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
}
