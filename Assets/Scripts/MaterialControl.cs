using UnityEngine;

public class MaterialControl : MonoBehaviour
{

    [Header("Кадры анимации")]
    [SerializeField] private Texture[] textures;

    [Header("Скорость анимации (кадров в секунду)")]
    [SerializeField] private float fps = 30f;


    [Header("Нужна ли анимация")]
    [SerializeField] protected bool anim_need;

    [Header("Материал для лазера")]
    [SerializeField] private Material laserMaterial;

    private int animStep;
    private float animTimer;

    private void Update()
    {
        if (anim_need) AnimMove();
    }
    public void AnimMove()
    {
        if (textures == null || textures.Length == 0 || laserMaterial == null)
            return;

        animTimer += Time.deltaTime;
        if (animTimer >= 1f / fps)
        {
            animStep = (animStep + 1) % textures.Length;
            laserMaterial.SetTexture("_MainTex", textures[animStep]);
            animTimer = 0f;
        }
    }
    /// <summary>
    /// Установить материал через код (если нужно динамически менять).
    /// </summary>
    public void SetMaterial(Material newMat)
    {
        laserMaterial = newMat;
    }
}
