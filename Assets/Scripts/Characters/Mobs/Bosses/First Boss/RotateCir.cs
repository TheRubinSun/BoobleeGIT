using UnityEngine;

public class RotateCir : MonoBehaviour
{
    private Transform cirTrans;
    private void Start()
    {
        cirTrans = GetComponent<Transform>();
    }
    private void Update()
    {

        cirTrans.Rotate(0, 0, 0.1f);
    }
}
