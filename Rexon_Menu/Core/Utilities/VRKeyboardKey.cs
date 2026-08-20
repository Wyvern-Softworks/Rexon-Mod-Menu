// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Utilities.VRKeyboardKey
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using UnityEngine;

namespace Rexon_Menu.Core.Utilities;

public class VRKeyboardKey : MonoBehaviour
{
	public string KeyValue;

	private float NextAllowedPressTime;

	private static SphereCollider LeftHandPointer;

	private static SphereCollider RightHandPointer;

	public static void SetHandPointers(SphereCollider leftPointer, SphereCollider rightPointer)
	{
		LeftHandPointer = leftPointer;
		RightHandPointer = rightPointer;
	}

	private void OnTriggerEnter(Collider other)
	{
		bool isLeftHand = other == LeftHandPointer;
		bool isRightHand = other == RightHandPointer;
		if ((isLeftHand || isRightHand) && Time.time >= NextAllowedPressTime)
		{
			NextAllowedPressTime = Time.time + 0.15f;
			GorillaTagger.Instance.StartVibration(isLeftHand, 0.05f, 0.02f);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, isLeftHand, 0.3f);
			VRKeyboard.HandleKeyPress(KeyValue);
		}
	}
}
