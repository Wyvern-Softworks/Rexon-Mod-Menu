// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Movement.SpeedBoost
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Movement;

[Mod("Speed Boost: 3", "Movement", "VR: triggers to adjust. PC: click to cycle.", false, 7, ModType.Toggle, false)]
internal class SpeedBoost : MonoBehaviour
{
	private const float MinimumMultiplier = 1f;
	private const float MaximumMultiplier = 20f;
	private const float VrAdjustmentStep = 0.01f;
	private static readonly float[] DesktopPresets = { 1f, 2f, 3f, 5f, 8f, 10f, 15f, 20f };

	public static float Multiplier = 3f;

	private float _originalMaxJumpSpeed;
	private float _originalJumpMultiplier;
	private float _lastDisplayedMultiplier;

	private void OnEnable()
	{
		if (GTPlayer.Instance != null)
		{
			_originalMaxJumpSpeed = GTPlayer.Instance.maxJumpSpeed;
			_originalJumpMultiplier = GTPlayer.Instance.jumpMultiplier;
		}

		if (!XRSettings.isDeviceActive)
		{
			CycleDesktopPreset();
			BundleManager.SetSpeedBoostStatusText($"Speed Boost: {Multiplier:G}");
			Recovered.Obfuscated.ConfigurationManager.SaveIfAutoLoadEnabled();
			Object.Destroy(this);
			return;
		}

		_lastDisplayedMultiplier = Multiplier;
		BundleManager.SetSpeedBoostStatusText($"Speed Boost: {Multiplier:F2}");
	}

	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}

		if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f)
		{
			Multiplier = Mathf.Clamp(Multiplier + VrAdjustmentStep, MinimumMultiplier, MaximumMultiplier);
		}
		else if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f)
		{
			Multiplier = Mathf.Clamp(Multiplier - VrAdjustmentStep, MinimumMultiplier, MaximumMultiplier);
		}

		float roundedMultiplier = Mathf.Round(Multiplier * 100f) / 100f;
		if (!Mathf.Approximately(_lastDisplayedMultiplier, roundedMultiplier))
		{
			_lastDisplayedMultiplier = roundedMultiplier;
			BundleManager.SetSpeedBoostStatusText($"Speed Boost: {Multiplier:F2}");
		}
	}

	private void LateUpdate()
	{
		if (GTPlayer.Instance != null)
		{
			float boostedSpeed = 5f * Multiplier;
			GTPlayer.Instance.maxJumpSpeed = boostedSpeed;
			GTPlayer.Instance.jumpMultiplier = boostedSpeed;
		}
	}

	private void OnDisable()
	{
		if (GTPlayer.Instance != null)
		{
			GTPlayer.Instance.maxJumpSpeed = _originalMaxJumpSpeed;
			GTPlayer.Instance.jumpMultiplier = _originalJumpMultiplier;
		}

		BundleManager.SetSpeedBoostStatusText($"Speed Boost: {Multiplier:F2}");
		Recovered.Obfuscated.ConfigurationManager.SaveIfAutoLoadEnabled();
	}

	private static void CycleDesktopPreset()
	{
		int nextIndex = 0;
		for (int index = 0; index < DesktopPresets.Length; index++)
		{
			if (Mathf.Approximately(DesktopPresets[index], Multiplier))
			{
				nextIndex = (index + 1) % DesktopPresets.Length;
				break;
			}
		}

		Multiplier = DesktopPresets[nextIndex];
	}
}
