using UnityEngine;

public class HealthBar2D
{
    private Transform HpBarFillTrans;
    public SpriteRenderer healthBarBG_sp_ren;
    public SpriteRenderer healthBarFill_sp_ren;
    private float maxWidth;
    private float height;
    private float localPosY;
    private float xPosForHp;
    float healthPercent;

    public HealthBar2D(GameObject HpBarBG, GameObject HpBarFill)
    {
        this.HpBarFillTrans = HpBarFill.transform;

        healthBarBG_sp_ren = HpBarBG.GetComponent<SpriteRenderer>();
        healthBarFill_sp_ren = HpBarFill.GetComponent<SpriteRenderer>();

        height = HpBarFillTrans.localScale.y;

        maxWidth = HpBarFillTrans.localScale.x;
        xPosForHp = -maxWidth / 2;
        localPosY = HpBarFillTrans.localPosition.y;

        SetActive(false);
    }
    public void UpdateHealthBar(float currentHp, float maxHp)
    {
        if(healthBarBG_sp_ren.enabled == false) SetActive(true);

        healthPercent = Mathf.Clamp01(currentHp / maxHp);
        
        if (healthPercent >= 0.65f) healthBarFill_sp_ren.color = GlobalColors.fullHp;
        else if(healthPercent < 0.65f && healthPercent >= 0.30f) healthBarFill_sp_ren.color = GlobalColors.halfHp;
        else healthBarFill_sp_ren.color = GlobalColors.lowHp;

        HpBarFillTrans.localScale = new Vector2(maxWidth * healthPercent, height);
        HpBarFillTrans.localPosition = new Vector2(xPosForHp * (1 - healthPercent), localPosY);
    }
    public void SetActiveHP(bool active)
    {
        if (healthPercent == 0) return;
        else
        {
            SetActive(active);
        }
    }
    public void SetActive(bool active)
    {
        healthBarBG_sp_ren.enabled = active;
        healthBarFill_sp_ren.enabled = active;
    }
    public void FlipX(bool flip)
    {
        if (healthBarBG_sp_ren.enabled == false) return;
        healthBarBG_sp_ren.flipX = flip;
        healthBarFill_sp_ren.flipX = flip;
    }
}
