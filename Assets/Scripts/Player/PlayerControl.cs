using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour 
{
    Player player;
    //������� ������
    [SerializeField] GameObject playerObj;
    [SerializeField] Transform cameraObj;
    [SerializeField] GameObject hand;
    [SerializeField] Transform centerLegs;
    [SerializeField] Transform[] legsLines;
    [SerializeField] Transform[] legsCenter;
    [SerializeField] Transform WeaponSlots;
    [SerializeField] Transform MinionsSlots;

    [SerializeField] private float radiusCenterLegs = 0.4f;                // ������ �������

    private LineRenderer lineRenderer;

    //����������� � �������
    [SerializeField] private Vector2 inputDirection;


    private Vector2 mousePos;
    private Vector2 movement;
    //Vector2 mousePos;
    public Vector2 direction;

    private Dictionary<int, WeaponControl> weaponsAndArms;
    private Dictionary<int, MinionControl> minionSlots;
    private Rigidbody2D rb;
    private float rangeMinion;

    //�����
    AudioSource audioSource_Move;

    //Таймеры
    private float updateRate = 0.2f;
    private float nextUpdateTime = 0f;

    [SerializeField] AudioClip audioClips;

    void Start()
    {
        player = GetComponentInParent<Player>();
        //lineRenderer = MinionsSlots.GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    // ������� ��� ��������, ��������� �� ��������� ���� ��� UI
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    private void Update()
    {
        //���� ��������
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(inputDirection.sqrMagnitude > 0 )
        {
            Move();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= nextUpdateTime)
            {
                MoveLegs();
                nextUpdateTime = Time.time + updateRate;
            }
        }

        RotateWeaponSlots();

        if(!IsPointerOverUI())
        {
            if(Input.GetMouseButton(0))
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseMinions();
        }
    }
    private void LateUpdate()
    {
        cameraObj.position = new Vector3(transform.position.x, transform.position.y, -10f);
        //cameraObj.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), -10f);
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
            if (child != null) //��������, ���� ����� ������ ���� ��������
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
        movement = inputDirection.normalized * player.GetPlayerStats().Mov_Speed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        MoveLegs();

        MoveCenterLegs(inputDirection);
    }
    private void MoveLegs()
    {
        foreach (Transform legsLine in legsLines)
        {
            legsLine.GetComponent<LineControle>().AnimMove();
        }
        foreach (Transform legCenter in legsCenter)
        {
            legCenter.GetComponent<LegControl>().CheckPos();
        }
    }
    void MoveCenterLegs(Vector2 inputDirection)
    {
        // ������������ �������� ��� ������ ���
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
