
using UnityEngine;

public class LegControl : LineControle
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform leg;
    public void MoveLinesLegs()
    {
        if (anim_need) AnimMove();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, body.position);
        lineRenderer.SetPosition(1, leg.position);
    }
}
