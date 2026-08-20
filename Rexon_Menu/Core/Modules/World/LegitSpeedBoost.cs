// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.LegitSpeedBoost
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Legit Speed Boost [BOTH GRIPS]", "World", "Moves your hands to boost speed naturally.", false, 38, ModType.Toggle, false)]
internal class LegitSpeedBoost : MonoBehaviour
{
	private const float GripThreshold = 0.5f;
	private const float MinimumGroundDistance = 0.1f;
	private const float MaximumGroundDistance = 1.5f;

	private bool _rightHandBoosted;
	private bool _leftHandBoosted;

	private void Update()
	{
		float rightGrip = ControllerInputPoller.GripFloat(XRNode.RightHand);
		float leftGrip = ControllerInputPoller.GripFloat(XRNode.LeftHand);
		if (rightGrip <= GripThreshold || leftGrip <= GripThreshold)
		{
			return;
		}

		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		UpdateHand(localRig.rightHandTransform, GorillaTagger.Instance.rightHandTransform, ref _rightHandBoosted);
		UpdateHand(localRig.leftHandTransform, GorillaTagger.Instance.leftHandTransform, ref _leftHandBoosted);
	}

	private static void UpdateHand(Transform rigHand, Transform trackedHand, ref bool isBoosted)
	{
		if (!Physics.Raycast(rigHand.position, Vector3.down, out RaycastHit groundHit))
		{
			return;
		}

		bool isWithinBoostRange = groundHit.distance >= MinimumGroundDistance && groundHit.distance <= MaximumGroundDistance;
		if (isWithinBoostRange && !isBoosted)
		{
			MoveHandBelowRig(trackedHand, 1f);
			isBoosted = true;
		}
		else if (!isWithinBoostRange)
		{
			MoveHandBelowRig(trackedHand, 1.5f);
			isBoosted = false;
		}
	}

	private static void MoveHandBelowRig(Transform hand, float verticalOffset)
	{
		Transform rigTransform = GorillaTagger.Instance.offlineVRRig.transform;
		hand.position = rigTransform.position - new Vector3(0f, verticalOffset, 0f) - rigTransform.forward;
	}

	private void OnDisable()
	{
		_rightHandBoosted = false;
		_leftHandBoosted = false;
	}
}
