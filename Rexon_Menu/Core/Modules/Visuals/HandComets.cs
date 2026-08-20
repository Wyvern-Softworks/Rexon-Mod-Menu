// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.HandComets
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Hand Comets", "Visuals", "Fiery trails when hands move fast.", false, 32, ModType.Toggle, false)]
internal class HandComets : MonoBehaviour
{
	private const float EmissionSpeed = 1f;

	private TrailRenderer _leftTrail;
	private TrailRenderer _rightTrail;
	private Vector3 _lastLeftPosition;
	private Vector3 _lastRightPosition;

	private void OnEnable()
	{
		Transform leftHand = GorillaTagger.Instance.leftHandTransform;
		Transform rightHand = GorillaTagger.Instance.rightHandTransform;
		if (leftHand != null && _leftTrail == null)
		{
			_leftTrail = CreateCometTrail(leftHand);
		}
		if (rightHand != null && _rightTrail == null)
		{
			_rightTrail = CreateCometTrail(rightHand);
		}
		_lastLeftPosition = leftHand != null ? leftHand.position : Vector3.zero;
		_lastRightPosition = rightHand != null ? rightHand.position : Vector3.zero;
	}

	private static TrailRenderer CreateCometTrail(Transform parent)
	{
		GameObject trailObject = new GameObject("HandComet");
		trailObject.transform.SetParent(parent, false);
		trailObject.transform.localPosition = Vector3.zero;
		TrailRenderer trail = trailObject.AddComponent<TrailRenderer>();
		trail.time = 0.3f;
		trail.startWidth = 0.05f;
		trail.endWidth = 0f;
		trail.minVertexDistance = 0.01f;
		Gradient gradient = new Gradient
		{
			colorKeys = new[]
			{
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(new Color(1f, 0.5f, 0f), 0.5f),
				new GradientColorKey(new Color(1f, 0.3f, 0f), 1f)
			},
			alphaKeys = new[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0.5f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			}
		};
		trail.colorGradient = gradient;
		((Renderer)trail).material = ShaderPatch.CreateTransparentMaterial(Color.white);
		trail.emitting = false;
		return trail;
	}

	private void Update()
	{
		Transform leftHand = GorillaTagger.Instance.leftHandTransform;
		Transform rightHand = GorillaTagger.Instance.rightHandTransform;
		float frameTime = Mathf.Max(Time.deltaTime, 0.0001f);
		if (leftHand != null && _leftTrail != null)
		{
			_leftTrail.emitting = (leftHand.position - _lastLeftPosition).magnitude / frameTime > EmissionSpeed;
			_lastLeftPosition = leftHand.position;
		}
		if (rightHand != null && _rightTrail != null)
		{
			_rightTrail.emitting = (rightHand.position - _lastRightPosition).magnitude / frameTime > EmissionSpeed;
			_lastRightPosition = rightHand.position;
		}
	}

	private void OnDisable() => DestroyTrails();
	private void OnDestroy() => DestroyTrails();

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
