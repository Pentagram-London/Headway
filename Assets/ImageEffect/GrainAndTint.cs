using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GrainAndTint : MonoBehaviour {

	[Range(0,1)]
	public float grayScaleAmount = 1.0f;
	public Material material;
	float rand;

	void FixedUpdate() {
		rand = Random.Range (-1.0f, 1.0f);
	}

	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		material.SetFloat ("_LuminosityAmount", grayScaleAmount);
		material.SetFloat ("_RandomValue", rand);
		Graphics.Blit (sourceTexture, destTexture, material);
	}

}
