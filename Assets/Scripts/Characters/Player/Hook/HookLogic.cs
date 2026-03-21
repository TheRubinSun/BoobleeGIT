using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.Tilemaps;
using UnityEngine;

public class HookLogic : MonoBehaviour 
{
    private float RangeHook;
    private float SpeedHook;
    private float SpeedMove;
    private LayerMask LayerTarger;

    private bool isWorking;
    private bool playerIsMoving;

    private GameObject hookPrefab;
    private GameObject hookObj;
    private int idHook;
    private LineRenderer lineRenderer;
    public void LoadData(float range, float speedHook, float speedMove, LayerMask layerTarger, out Vector2 direction, int _idHook = 0)
    {
        direction = GetDirectionToMouse();
        if (isWorking) return;

        RangeHook = range;
        SpeedHook = speedHook;
        SpeedMove = speedMove;
        LayerTarger = layerTarger;
        idHook = _idHook;

        if (hookPrefab == null) hookPrefab = ResourcesData.GetHookPrefab(idHook);

        StartCoroutine(ExecuteHookCycle(direction));
    }
    public bool GetMoveToHook() => playerIsMoving;
    private Vector2 GetDirectionToMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3 playerPos = transform.position;
        playerPos.z = 0;

        return (mouseWorldPos - playerPos).normalized;
    }
    private IEnumerator ExecuteHookCycle(Vector2 direction)
    {
        isWorking = true;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, RangeHook, LayerTarger);
        Vector2 targetPos = hit.collider != null ? hit.point : (Vector2)transform.position + direction * RangeHook;

        hookObj = Instantiate(hookPrefab, transform.position, Quaternion.identity);
        lineRenderer = hookObj.GetComponent<LineRenderer>();

        yield return MoveObject(hookObj.transform, targetPos, SpeedHook);

        if(hit.collider != null)
        {
            hookObj.GetComponent<HookObj>().IsRotate = false;
            playerIsMoving = true;
            yield return MoveObject(transform, targetPos, SpeedMove);
            playerIsMoving = false;
        }
        else
        {
            yield return MoveObject(hookObj.transform, transform, SpeedHook);
        }
        Destroy(hookObj);
        isWorking = false;

    }
    private IEnumerator MoveObject(Transform objToMove, Vector2 destination, float speed)
    {
        while(objToMove != null && Vector2.Distance(objToMove.position, destination) > 0.1f)
        {
            objToMove.position = Vector2.MoveTowards(objToMove.position, destination, speed * Time.deltaTime);
            UpdateLine();
            yield return null;
        }
    }
    private IEnumerator MoveObject(Transform objToMove, Transform target, float speed)
    {
        while (objToMove != null && target != null && Vector2.Distance(objToMove.position, target.position) > 0.1f)
        {
            objToMove.position = Vector2.MoveTowards(objToMove.position, target.position, speed * Time.deltaTime);
            UpdateLine();
            yield return null;
        }
    }
    private void UpdateLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hookObj.transform.position);
    }
    //private void Hooking(Vector2 moveTo)
    //{
    //    if (hookIsMoving) return;

    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, moveTo, RangeHook, LayerTarger);

    //    if (playerIsMoving) return;

    //    hookObj = Instantiate(hookPrefab, transform.position, transform.rotation);
    //    HookObjData = hookObj.GetComponent<HookObj>();
    //    hokTransfrom = hookObj.transform;

    //    Vector2 targetPos;
    //    if (hit.collider == null)
    //        targetPos = (Vector2)transform.position + moveTo * RangeHook;
    //    else targetPos = hit.point;

    //    Moving = StartCoroutine(LaunchHook(targetPos, hit.collider != null));
    //}

    //private IEnumerator LaunchHook(Vector2 endPos, bool findWall)
    //{
    //    yield return HookToPos(endPos);

    //    if (findWall)
    //        yield return MoveToHook();
    //    else
    //        yield return HookRetrun();
    //    Destroy(hookObj);
    //}
    //private IEnumerator HookToPos(Vector2 endPos)
    //{
    //    hookIsMoving = true;

    //    while (Vector2.Distance(hokTransfrom.position, endPos) > 0.01f)
    //    {
    //        hokTransfrom.position = Vector2.MoveTowards(hokTransfrom.position, endPos, SpeedHook * Time.deltaTime);
    //        yield return null;
    //    }
    //    hookIsMoving = false;
    //    HookObjData.IsRotate = false;
    //}
    //private IEnumerator HookRetrun()
    //{
    //    hookIsMoving = true;

    //    while (Vector2.Distance(hokTransfrom.position, transform.position) > 0.01f)
    //    {
    //        hokTransfrom.position = Vector2.MoveTowards(hokTransfrom.position, transform.position, SpeedHook * Time.deltaTime);
    //        yield return null;
    //    }
    //    hookIsMoving = false;
    //    HookObjData.IsRotate = false;
    //}
    //private IEnumerator MoveToHook()
    //{
    //    playerIsMoving = true;
    //    while (Vector2.Distance(transform.position, hokTransfrom.position) > 0.01f)
    //    {
    //        transform.position = Vector2.MoveTowards(transform.position, hokTransfrom.position, SpeedMove * Time.deltaTime);
    //        yield return null;
    //    }
    //    playerIsMoving = false;
    //}
}
