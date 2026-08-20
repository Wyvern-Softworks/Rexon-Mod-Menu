// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HandTrails
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Hand Trails", "Rig", "Trail effects on both hands.", false, 38, ModType.Toggle, false)]
internal class HandTrails : MonoBehaviour
{
	private TrailRenderer _leftTrail;
	private TrailRenderer _rightTrail;

	private void OnEnable()
	{
		Transform leftHand = GorillaTagger.Instance.leftHandTransform;
		Transform rightHand = GorillaTagger.Instance.rightHandTransform;
		if (leftHand != null && _leftTrail == null)
		{
			_leftTrail = CreateHandTrail(leftHand);
		}
		if (rightHand != null && _rightTrail == null)
		{
			_rightTrail = CreateHandTrail(rightHand);
		}
	}

	private static TrailRenderer CreateHandTrail(Transform parent)
	{
		GameObject trailObject = new GameObject("HandTrail");
		trailObject.transform.SetParent(parent, false);
		trailObject.transform.localPosition = Vector3.zero;
		TrailRenderer trail = trailObject.AddComponent<TrailRenderer>();
		trail.time = 0.5f;
		trail.startWidth = 0.02f;
		trail.endWidth = 0f;
		trail.minVertexDistance = 0.01f;
		trail.startColor = Color.cyan;
		trail.endColor = new Color(0f, 1f, 1f, 0f);
		((Renderer)trail).material = ShaderPatch.CreateTransparentMaterial(Color.white);
		return trail;
	}

	private void OnDisable() => DestroyTrails();

	private void DestroyTrails()
	{
		DestroyTrail(ref _leftTrail);
		DestroyTrail(ref _rightTrail);
	}

	private static void DestroyTrail(ref TrailRenderer trail)
	{
		if (trail == null)
		{
			return;
		}
		Object.Destroy(trail.gameObject);
		trail = null;
	}
}
