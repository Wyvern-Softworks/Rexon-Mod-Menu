// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FoodSize
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Food Size [MASTER]", "Critter", "RT increases, LT decreases food size.", false, 11, ModType.Toggle, false)]
internal class FoodSize : MonoBehaviour
{
	private const string LabelPrefix = "Food Size: ";
	private const string ValueFormat = "F2";
	private const float AdjustmentInterval = 0.05f;
	private const float TriggerThreshold = 0.4f;

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
			FoodSizeSetting.CurrentScale = Increase(FoodSizeSetting.CurrentScale);
			UpdateLabel();
		}
		else if (leftTrigger > TriggerThreshold)
		{
			_lastAdjustmentTime = Time.time;
			FoodSizeSetting.CurrentScale = Decrease(FoodSizeSetting.CurrentScale);
			UpdateLabel();
		}
	}

	private static float Increase(float scale)
	{
		if (scale <= 1f)
		{
			return scale + 0.01f;
		}
		if (scale <= 10f)
		{
			return scale + 0.1f;
		}
		if (scale <= 50f)
		{
			return scale + 1f;
		}

		return scale + 10f;
	}

	private static float Decrease(float scale)
	{
		if (scale <= 1f)
		{
			return Mathf.Max(0.01f, scale - 0.01f);
		}
		if (scale <= 10f)
		{
			return scale - 0.1f;
		}
		if (scale <= 50f)
		{
			return scale - 1f;
		}

		return scale - 10f;
	}

	private static void UpdateLabel()
	{
		BundleManager.SetFoodSizeStatusText(LabelPrefix + FoodSizeSetting.CurrentScale.ToString(ValueFormat));
	}
}
