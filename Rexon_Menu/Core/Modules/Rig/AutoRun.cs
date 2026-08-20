// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.AutoRun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Auto Run", "Rig", "Hold right grab to auto run.", false, 9, ModType.Toggle, false)]
internal sealed class AutoRun : MonoBehaviour
{
	private void Update()
	{
		if (!ControllerInputPoller.instance.rightGrab)
		{
			return;
		}

		Transform head = GorillaTagger.Instance.headCollider.transform;
		Vector3 forward = head.forward.normalized;
		Vector3 right = head.right.normalized;
		float phase = Time.time * 40f;

		Vector3 rightSwing = -forward * Mathf.Cos(phase) * 1.2f
			+ Vector3.up * (Mathf.Sin(phase) * 0.8f);
		Vector3 leftSwing = forward * Mathf.Cos(phase) * 1.2f
			- Vector3.up * (Mathf.Sin(phase) * 0.8f);
		Vector3 rightBase = forward * 0.4f + right * 0.15f + Vector3.down * 0.5f;
		Vector3 leftBase = forward * 0.4f - right * 0.15f + Vector3.down * 0.5f;

		GorillaTagger.Instance.rightHandTransform.position = head.position + rightBase + rightSwing;
		GorillaTagger.Instance.leftHandTransform.position = head.position + leftBase + leftSwing;
		GorillaTagger.Instance.rightHandTransform.rotation = Quaternion.LookRotation(forward);
		GorillaTagger.Instance.leftHandTransform.rotation = Quaternion.LookRotation(forward);

		Vector3 horizontalForward = new Vector3(forward.x, 0f, forward.z).normalized;
		Transform body = GorillaTagger.Instance.bodyCollider.transform;
		Vector3 currentPosition = body.position;
		Vector3 targetPosition = currentPosition + horizontalForward * 0.12f;
		int groundLayer = LayerMask.GetMask("Default");
		if (Physics.Raycast(currentPosition, Vector3.down, out RaycastHit groundHit, 2f, groundLayer)
			&& groundHit.normal.y > 0.7f)
		{
			targetPosition.y = groundHit.point.y + 1f;
		}

		body.position = Vector3.Lerp(currentPosition, targetPosition, 0.3f);
	}
}
