using System;
using UnityEngine;

public class TracerMaterial
{
	private Material mat;

	private Color32 tracerColor;

	public Color32 color
	{
		get
		{
			return this.tracerColor;
		}
	}

	public Material material
	{
		get
		{
			return this.mat;
		}
	}

	public TracerMaterial(string hexColor, Material material)
	{
		this.tracerColor = ApplicationDataManager.ConvertHexColorToRGB(hexColor);
		this.mat = UnityEngine.Object.Instantiate<Material>(material);
	}

	private void BuildDynamic()
	{
		Material material = Resources.Load("Tracers/Tracer_MAT_1", typeof(Material)) as Material;
		if (material != null)
		{
			this.mat = UnityEngine.Object.Instantiate<Material>(material);
		}
		else
		{
			Debug.LogError("Material not loaded.");
		}
		Texture2D texture2D = null;
		if (this.mat != null)
		{
			Texture2D texture2D1 = Resources.Load("Tracers/TRACER_Lines_TEX_1", typeof(Texture2D)) as Texture2D;
			texture2D = UnityEngine.Object.Instantiate<Texture2D>(texture2D1);
		}
		if (texture2D == null)
		{
			Debug.LogError("Texture not loaded.");
			Debug.LogError("TRACER_MATERIAL::Error creating material clone.  Material not created.");
			return;
		}
		Color32[] pixels32 = texture2D.GetPixels32();
		for (int i = 0; i < (int)pixels32.Length; i++)
		{
			pixels32[i].r = this.tracerColor.r;
			pixels32[i].g = this.tracerColor.g;
			pixels32[i].b = this.tracerColor.b;
		}
		texture2D.SetPixels32(pixels32);
		texture2D.Apply();
		this.mat.SetTexture(0, texture2D);
	}
}