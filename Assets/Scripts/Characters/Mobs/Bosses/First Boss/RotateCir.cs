using UnityEngine;

public class RotateCir : MonoBehaviour
{
    private Transform cirTrans;
    public float rotateSpeed = 30f;
    private void Start()
    {
        cirTrans = GetComponent<Transform>();
    }
    private void Update()
    {
        cirTrans.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
