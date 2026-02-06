using UnityEngine;

public class RotateCir : MonoBehaviour
{
    private Transform cirTrans;
    public float rotateSpeed;
    private void Start()
    {
        cirTrans = GetComponent<Transform>();
    }
    private void LateUpdate()
    {
        cirTrans.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
