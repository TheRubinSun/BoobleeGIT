using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour 
{
    public static PlayerControl Instance;
    //public event System.Action Switch_volume;

    private Player player;

    [SerializeField] private GameObject playerObj;
    private Transform cameraObj;
    [SerializeField] private Transform centerLegs;
    [SerializeField] private LineControle[] legsLines;
    [SerializeField] private LegsControl legsControl;
    [SerializeField] private Transform WeaponSlots;

    [SerializeField] private float radiusCenterLegs = 0.23f;        

    [SerializeField] private Vector2 inputDirection;


    private GameManager g_m;
    private Vector2 mousePos;
    private Vector2 movement;
    public Vector2 currentPosCenterLegs;
    //Vector2 mousePos;
    public Vector2 direction;

    private Dictionary<int, WeaponControl> weaponsAndArms;
    private Dictionary<int, MinionControl> minionSlots;
    private Rigidbody2D rb;
    private float rangeMinion;

    //�����
    AudioSource audioSource_Move;

    //Таймеры
    [SerializeField] private float updateRateLegsMove = 0.13f;
    [SerializeField] private float updateRateLegsStop = 0.3f;
    private float nextUpdateTime = 0f;

    [SerializeField] AudioClip audioClips;

    void Start()
    {
        Instance = this;
        g_m = GameManager.Instance;
        if (cameraObj == null) cameraObj =  GameObject.Find("Main Camera").transform;
        player = GetComponentInParent<Player>();
        rb = GetComponent<Rigidbody2D>();
        SetVolume();
    }
    private void SetVolume()
    {
        //legsControl.GetComponent<AudioSource>().volume = GlobalData.VOLUME_SOUNDS;
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (player.HaveMana(2)) player.SpendMana(2);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (player.TakeHealMana(2)) { }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Jump(1f);
        }
    }
    private void FixedUpdate()
    {
        if (inputDirection.sqrMagnitude > 0 && !player.PlayerStay)
        {
            Move();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= nextUpdateTime)
            {
                speed = player.GetPlayerStats().Mov_Speed;
                MoveLegs(speed);
                nextUpdateTime = Time.time + updateRateLegsStop;
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
        //foreach(MinionControl minion in minionSlots.Values)
        foreach (KeyValuePair<int, MinionControl> minion in minionSlots)
        {
            if (minion.Value != null)
            {
                minion.Value.UseMinion(minion.Key);
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
    public void Jump(float distance)
    {
        transform.position += (Vector3)inputDirection.normalized * distance;

        foreach (LineControle legsLine in legsLines)
        {
            legsLine.transform.position += (Vector3)inputDirection.normalized * distance;
        }
    }
    private float speed;
    public void Move()
    {
        speed = player.GetPlayerStats().Mov_Speed;
        movement = inputDirection.normalized * speed;
        g_m.PlayerPosY = transform.position.y;

        rb.linearVelocity = movement;

        if (Time.time >= nextUpdateTime)
        {
            MoveLegs(speed);
            nextUpdateTime = Time.time + updateRateLegsMove;
        }

        MoveCenterLegs(inputDirection, speed);
    }
    //int i = 0;
    private void MoveLegs(float speed)
    {
        //i++;
        //Debug.Log(i);
        legsControl.MoveLegs(speed);
    }

    void MoveCenterLegs(Vector2 inputDirection, float speed)
    {
        // ������������ �������� ��� ������ ���
        currentPosCenterLegs = centerLegs.localPosition;
        Vector2 newPos = currentPosCenterLegs + inputDirection * (speed / 2) * Time.deltaTime;


        if (newPos.sqrMagnitude > radiusCenterLegs * radiusCenterLegs)
        {
            centerLegs.localPosition = newPos.normalized * radiusCenterLegs;
        }
        else
        {
            centerLegs.localPosition = newPos;
        }
    }
}
