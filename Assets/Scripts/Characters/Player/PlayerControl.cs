using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour 
{
    private Player player;

    [SerializeField] private GameObject playerObj;
    [SerializeField] private Transform cameraObj;
    [SerializeField] private GameObject hand;
    [SerializeField] private Transform centerLegs;
    [SerializeField] private LineControle[] legsLines;
    [SerializeField] private LegsControl legsControl;
    [SerializeField] private Transform WeaponSlots;

    [SerializeField] private float radiusCenterLegs = 0.4f;        

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
        rb = GetComponent<Rigidbody2D>();
    }
    // ������� ��� ��������, ��������� �� ��������� ���� ��� UI
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    private void Update()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        RotateWeaponSlots();

        if (!IsPointerOverUI())
        {
            if(Input.GetMouseButton(0))
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseMinions();
        }
    }
    private void FixedUpdate()
    {
        if (inputDirection.sqrMagnitude > 0 && !player.playerStay)
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
    }
    private void LateUpdate()
    {
        Vector3 targerPos = new Vector3(transform.position.x, transform.position.y, -10f);
        cameraObj.position = Vector3.Lerp(cameraObj.position, targerPos, Time.deltaTime * 7f);

        foreach (LineControle legsLine in legsLines)
        {
            legsLine.MoveLinesLegs();
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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        WeaponSlots.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void Move()
    {
        movement = inputDirection.normalized * player.GetPlayerStats().Mov_Speed;
        GameManager.Instance.PlayerPosY = transform.position.y;

        rb.linearVelocity = movement;

        MoveLegs();
        MoveCenterLegs(inputDirection);
    }
    private void MoveLegs()
    {
        legsControl.MoveLegs(player.GetPlayerStats().Mov_Speed);
    }
    void MoveCenterLegs(Vector2 inputDirection)
    {
        // ������������ �������� ��� ������ ���
        Vector2 currentPos = centerLegs.localPosition;
        Vector2 newPos = currentPos + inputDirection.normalized * (player.GetPlayerStats().Mov_Speed/2) * Time.deltaTime;


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
