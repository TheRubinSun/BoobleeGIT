using System.Collections;
using UnityEditor;
using UnityEngine;

public class HookObj : MonoBehaviour
{
    public float speed = 1f;
    private Transform hook;
    public bool IsRotate { private get; set; } = true;
    private void Start()
    {
        hook = GetComponent<Transform>();
        StartCoroutine(RotateHook());
    }
    private IEnumerator RotateHook()
    {
        while (IsRotate)
        {
            hook.Rotate(0, 0, speed * Time.deltaTime);
            yield return null;
        }
    }
}
