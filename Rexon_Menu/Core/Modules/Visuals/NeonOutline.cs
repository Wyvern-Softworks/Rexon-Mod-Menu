// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.NeonOutline
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Neon Outline", "Visuals", "Glowing edge on your rig.", false, 31, ModType.Toggle, false)]
internal class NeonOutline : MonoBehaviour
{
	private const string OutlineShaderName = "GUI/Text Shader";
	private const string EmissionColorProperty = "_EmissionColor";

	private static readonly Color OutlineColor = new(0f, 1f, 1f, 1f);
	private static readonly Color EmissionColor = new(0f, 2f, 2f, 1f);

	private readonly struct RendererState
	{
		internal readonly Renderer Renderer;
		internal readonly Material[] OriginalMaterials;

		internal RendererState(Renderer renderer, Material[] originalMaterials)
		{
			Renderer = renderer;
			OriginalMaterials = originalMaterials;
		}
	}

	private readonly List<RendererState> _rendererStates = new();
	private bool _isApplied;

	private void OnEnable()
	{
		ApplyOutline();
	}

	private void Update()
	{
		if (!_isApplied)
		{
			ApplyOutline();
		}
	}

	private void ApplyOutline()
	{
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		if (localRig == null)
		{
			return;
		}

		Renderer[] renderers = localRig.GetComponentsInChildren<Renderer>(true);
		if (renderers == null || renderers.Length == 0)
		{
			return;
		}

		_rendererStates.Clear();
		Shader outlineShader = Shader.Find(OutlineShaderName);
		if (outlineShader == null)
		{
			return;
		}

		foreach (Renderer renderer in renderers)
		{
			if (renderer == null || renderer.materials == null)
			{
				continue;
			}

			Material[] materials = renderer.materials;
			Material[] originalMaterials = new Material[materials.Length];
			for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
			{
				originalMaterials[materialIndex] = new Material(materials[materialIndex]);
				materials[materialIndex].shader = outlineShader;
				materials[materialIndex].color = OutlineColor;
				materials[materialIndex].SetColor(EmissionColorProperty, EmissionColor);
			}

			_rendererStates.Add(new RendererState(renderer, originalMaterials));
		}

		_isApplied = true;
	}

	private void OnDisable()
	{
		RestoreOriginalMaterials();
	}

	private void OnDestroy()
	{
		RestoreOriginalMaterials();
	}

	private void RestoreOriginalMaterials()
	{
		foreach (RendererState state in _rendererStates)
		{
			if (state.Renderer == null)
			{
				continue;
			}

			Material[] materials = state.Renderer.materials;
			int materialCount = Mathf.Min(materials.Length, state.OriginalMaterials.Length);
			for (int materialIndex = 0; materialIndex < materialCount; materialIndex++)
			{
				Material original = state.OriginalMaterials[materialIndex];
				materials[materialIndex].shader = original.shader;
				materials[materialIndex].color = original.color;
				if (original.HasProperty(EmissionColorProperty))
				{
					materials[materialIndex].SetColor(EmissionColorProperty, original.GetColor(EmissionColorProperty));
				}
			}
		}

		_rendererStates.Clear();
		_isApplied = false;
	}
}
