using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour 
{
    public static PlayerControl Instance;
    //public event System.Action Switch_volume;

    [SerializeField] private GameObject playerObj;
    private Transform cameraObj;
    [SerializeField] private Transform centerLegs;
    [SerializeField] private LegControl[] legsLines;
    [SerializeField] private LegsControl legsControl;
    [SerializeField] private Transform WeaponSlots;

    [SerializeField] private float radiusCenterLegs = 0.23f;        

    private Vector2 inputDirection;

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

    public void StartControl()
    {
        Instance = this;
        if (cameraObj == null) cameraObj = GameObject.Find("Main Camera").transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!GlobalData.LoadedGame) return;

       inputDirection = PlayerInputHandler.CurInputDirection;
        RotateWeaponSlots();
    }
    private void FixedUpdate()
    {
        if (inputDirection.sqrMagnitude > 0 && !GlobalData.Player.PlayerStay)
        {
            Move();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= nextUpdateTime)
            {
                speed = GlobalData.Player.GetPlayerStats().Mov_Speed;
                MoveLegs(speed);
                nextUpdateTime = Time.time + updateRateLegsStop;
            }
        }
    }
    private void LateUpdate()
    {
        Vector3 targerPos = new Vector3(transform.position.x, transform.position.y, -10f);
        cameraObj.position = Vector3.Lerp(cameraObj.position, targerPos, Time.deltaTime * 7f);

        foreach (LegControl legsLine in legsLines)
        {
            legsLine.MoveLinesLegs();
        }
    }
    public void UpdateSlots(Dictionary<int, WeaponControl> weaponObj, Dictionary<int, MinionControl> minionObj)
    {
        weaponsAndArms = weaponObj;
        minionSlots = minionObj;
    }

    public void Attack()
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
    public void UseMinions()
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
        float roundedAngle = (int)(angle * 10f) / 10f;

        WeaponSlots.rotation = Quaternion.Euler(0, 0, roundedAngle);
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
        speed = GlobalData.Player.GetPlayerStats().Mov_Speed;
        movement = inputDirection.normalized * speed;
        GlobalData.GameManager.PlayerPosY = transform.position.y;

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
