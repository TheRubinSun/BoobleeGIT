using System;
using UnityEngine;
using UnityEngine.U2D;

public class LineControle: MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] private Texture[] textures;
    private int animStep;

    [SerializeField] private float fps = 30f;
    [SerializeField] Transform body;
    [SerializeField] Transform leg;
    [SerializeField] bool anim_need;

    private float fpsCounter;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if(anim_need) AnimMove();

    }
    public void AnimMove()
    {
        
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animStep++;
            if (animStep == textures.Length)
                animStep = 0;

            lineRenderer.material.SetTexture("_MainTex", textures[animStep]);
            fpsCounter = 0f;
        }
    }
    public void MoveLinesLegs()
    {
        if (anim_need) AnimMove();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, body.position);
        lineRenderer.SetPosition(1, leg.position);
    }
}
