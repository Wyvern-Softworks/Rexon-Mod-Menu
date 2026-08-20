// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.TrackingAbuseFlick
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Tracking Abuse Flick", "Rig", "Tracking abuse flick.", false, 36, ModType.Toggle, false)]
internal sealed class TrackingAbuseFlick : MonoBehaviour
{
	private void Update()
	{
		if (!ControllerInputPoller.instance.rightControllerSecondaryButton)
		{
			return;
		}

		Transform head = GorillaTagger.Instance.headCollider.transform;
		Vector3 forward = head.forward.normalized;
		Vector3 right = head.right.normalized;
		float time = Time.time;
		Vector3 trackingJitter = new Vector3(
			Mathf.PerlinNoise(time * 10f, 0f) - 0.5f,
			Mathf.PerlinNoise(0f, time * 10f) - 0.5f,
			Mathf.PerlinNoise(time * 10f, time * 10f) - 0.5f) * 0.05f;
		Vector3 handOffset = new Vector3(0f, 1.5f, 0f) + right * Mathf.Sin(time * 0.1f) * 0.3f;

		GorillaTagger.Instance.rightHandTransform.position = head.position + handOffset + trackingJitter + right * 0.2f;
		GorillaTagger.Instance.leftHandTransform.position = head.position + handOffset + trackingJitter - right * 0.2f;
		GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity = (-forward + right * 0.1f).normalized * 16f;
	}
}
