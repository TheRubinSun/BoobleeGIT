using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    [SerializeField] Transform WeaponSlots;
    Vector2 mousePos;
    private void Update()
    {
        RotateWeaponSlots();
    }
    void RotateWeaponSlots()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        WeaponSlots.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
