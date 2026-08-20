// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CritterSize
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Critter Size [MASTER]", "Critter", "RT increases, LT decreases critter size.", false, 3, ModType.Toggle, false)]
internal class CritterSize : MonoBehaviour
{
	private const string LabelPrefix = "Critter Size: ";
	private const string ValueFormat = "F2";
	private const float AdjustmentInterval = 0.05f;
	private const float TriggerThreshold = 0.4f;
	private const float AdjustmentStep = 0.01f;
	private const float MinimumScale = 0.25f;
	private const float MaximumScale = 1.5f;

	private float _lastAdjustmentTime;

	private void Update()
	{
		if (Time.time < _lastAdjustmentTime + AdjustmentInterval)
		{
			return;
		}

		float rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		float leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		if (rightTrigger > TriggerThreshold)
		{
			_lastAdjustmentTime = Time.time;
			CritterSizeSetting.CurrentScale = Mathf.Min(
				CritterSizeSetting.CurrentScale + AdjustmentStep,
				MaximumScale);
			UpdateLabel();
		}
		else if (leftTrigger > TriggerThreshold)
		{
			_lastAdjustmentTime = Time.time;
			CritterSizeSetting.CurrentScale = Mathf.Max(
				CritterSizeSetting.CurrentScale - AdjustmentStep,
				MinimumScale);
			UpdateLabel();
		}
	}

	private static void UpdateLabel()
	{
		BundleManager.SetCritterSizeStatusText(
			LabelPrefix + CritterSizeSetting.CurrentScale.ToString(ValueFormat));
	}
}
