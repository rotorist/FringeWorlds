using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelationImageEffect : MonoBehaviour {

	[Range(0, 1.0f)]
	public float pixelSize = 0.0f;

	private string m_pixelSizePropertyName = "_PixelSize";

	private int m_pixelSizeID;

	private Material m_material;

	void Awake ()
	{
		InitPropertyIDs();
	}


	private void InitPropertyIDs()
	{
		if(m_material == null)
			m_material = new Material( Shader.Find("Unlit/Pixelization Shader") );

		m_pixelSizeID = Shader.PropertyToID(m_pixelSizePropertyName);
	}


	private void OnValidate()
	{
		if(m_material == null)
			m_material = new Material( Shader.Find("Unlit/Pixelization Shader") );

		m_material.SetFloat(m_pixelSizeID, pixelSize);
	}


	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit (source, destination, m_material);
	}
}
