using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOLRUNNER : MonoBehaviour
{

    public int FPS = 0;

    public ComputeShader compute;
    public Texture input;
    [SerializeField] int size;
    public RenderTexture result;
    private RenderTexture buffer;
    private bool buffering = false;
    private int kernel;
    public Material material;
    
    void Awake()
    {
        if (FPS != 0)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = FPS;
        }
    }
    void Start()
    {
        kernel = compute.FindKernel("CSMain");

        result = new RenderTexture(size, size, 24);
        result.filterMode = FilterMode.Point;
        result.enableRandomWrite = true;
        result.Create();

        buffer = new RenderTexture(size, size, 24);
        buffer.filterMode = FilterMode.Point;
        buffer.enableRandomWrite = true;
        buffer.Create();


        Graphics.Blit(input, result);
        buffering = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (FPS != 0)
        {
            if (Application.targetFrameRate != FPS)
                Application.targetFrameRate = FPS;
        }

        if (buffering)
        {
            compute.SetTexture(kernel, "Input", result);
            compute.SetTexture(kernel, "Result", buffer);

            compute.Dispatch(kernel, size / 8, size / 8, 1);
            material.mainTexture = buffer;

            buffering = false;
        } else
        {
            compute.SetTexture(kernel, "Input", buffer);
            compute.SetTexture(kernel, "Result", result);

            compute.Dispatch(kernel, size / 8, size / 8, 1);
            material.mainTexture = result;

            buffering = true;
        }
        
    }
}
