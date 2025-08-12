using System;
using UnityEngine;
using UnityEngine.U2D;

public class LineControle: MonoBehaviour
{
    
    [SerializeField] private Texture[] textures;
    [SerializeField] private float fps = 30f;
    [SerializeField] protected bool anim_need;

    protected LineRenderer lineRenderer;

    private float fpsCounter;
    protected int animStep;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
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
}
