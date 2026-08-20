// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.Gravity
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Gravity: 0.0", "World", "VR: triggers to adjust. PC: click to cycle.", false, 31, ModType.Toggle, false)]
internal class Gravity : MonoBehaviour
{
	private const string LabelFormat = "Gravity: {0:F1}";
	private const float TriggerThreshold = 0.4f;
	private const float AdjustmentStep = 0.01f;

	private static readonly float[] DesktopPresets = { 0f, 0.5f, 1f, 1.5f, 2f, 3f, 5f, -0.5f, -1f };

	public static float Multiplier;

	private float _lastDisplayedMultiplier;

	private void OnEnable()
	{
		if (!XRSettings.isDeviceActive)
		{
			Multiplier = GetNextDesktopPreset();
			UpdateLabel();
			ConfigurationManager.SaveIfAutoLoadEnabled();
			Object.Destroy(this);
			return;
		}

		_lastDisplayedMultiplier = Multiplier;
		UpdateLabel();
	}

	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}

		float rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		float leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		if (rightTrigger > TriggerThreshold)
		{
			Multiplier += AdjustmentStep;
		}
		else if (leftTrigger > TriggerThreshold)
		{
			Multiplier -= AdjustmentStep;
		}

		float roundedMultiplier = Mathf.Round(Multiplier * 10f) / 10f;
		if (!Mathf.Approximately(_lastDisplayedMultiplier, roundedMultiplier))
		{
			_lastDisplayedMultiplier = roundedMultiplier;
			UpdateLabel();
		}

		Rigidbody playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
		playerBody.useGravity = false;
		playerBody.AddForce(Physics.gravity * Multiplier * playerBody.mass, (ForceMode)0);
	}

	private static float GetNextDesktopPreset()
	{
		for (int index = 0; index < DesktopPresets.Length; index++)
		{
			if (Mathf.Approximately(DesktopPresets[index], Multiplier))
			{
				return DesktopPresets[(index + 1) % DesktopPresets.Length];
			}
		}

		return DesktopPresets[0];
	}

	private static void UpdateLabel()
	{
		BundleManager.SetGravityStatusText(string.Format(LabelFormat, Multiplier));
	}

	private static void RestoreBuiltInGravity()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}

		Rigidbody playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
		if (playerBody != null)
		{
			playerBody.useGravity = true;
		}
	}

	private void OnDisable()
	{
		RestoreBuiltInGravity();
		UpdateLabel();
		ConfigurationManager.SaveIfAutoLoadEnabled();
	}

	private void OnDestroy()
	{
		RestoreBuiltInGravity();
	}
}
