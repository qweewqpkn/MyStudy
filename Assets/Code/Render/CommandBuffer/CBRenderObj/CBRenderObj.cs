using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CBRenderObj : MonoBehaviour {

    public Renderer mTargetRenderer;
    public Material mTargetMaterial;
    CommandBuffer myCB;

    // Use this for initialization
    void OnEnable () {
        myCB = new CommandBuffer();
        myCB.DrawRenderer(mTargetRenderer, mTargetMaterial, 0, 0);
        Camera.main.AddCommandBuffer(CameraEvent.BeforeImageEffects, myCB);
        mTargetRenderer.enabled = false;

    }

    private void OnDisable()
    {
        Camera.main.RemoveCommandBuffer(CameraEvent.AfterImageEffects, myCB);
        myCB.Clear();
    }
}
