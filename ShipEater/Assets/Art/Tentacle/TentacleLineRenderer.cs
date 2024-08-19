using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField]
    private Texture2D[] textures;
    private int animationStep;

    [SerializeField]
    private float fps = 30f;
    private float fpsCounter;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f / fps)
        {
            animationStep++;
            if(animationStep == textures.Length)
            {
                animationStep = 0;
            }

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0f;
        }
    }
}
