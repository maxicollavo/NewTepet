using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintableSurface : MonoBehaviour
{
    private void Setup()
    {
        this._rt = RenderTexture((int)this.textureSize, (int)this.textureSize, 0, RenderTextureFormat.Default)
        {
            enableRandomWrite = true;
        };
        this._rt.Create();

        RenderTexture temp = RenderTexture.active;
        RenderTexture.active = this._rt;

        GL.Clear(true, true, Color.white);
        RenderTexture.active = temp;    
    

        Material remappedMaterial = new Material(material);
        remappedMaterial.SetTexture(SHADER_PARAM_DIRT_MASK, this._rt);

        this._cmd = CommandBufferPool.Get("Paint Surface");
        this._cmd.GetTemporaryRT(SHADER_PARAM_PREVIOUS_FRAME_TEX, this._rt.descriptor);
        this._cmd.Blit(this._rt, Shader, ParticleSystemAnimationMode, PREVIOUS_FRAME_TEX);
        this._cmd.SetRenderTarget(this._rt);
        this._cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        for (int i = 0; i < mainRendererMaterialCount; i++)
            this._cmd.DrawRenderer(this._mainRenderer, this.PaintMaterial, i, 0);
        this._cmd.RealeaseTemporaryRT(SHADER_PARAM_PREVIOUS_FRAME_TEX);
    }
}
