// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BackTrackDelay
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Back Track Delay: 0.50", "Rig", "Triggers (VR) / Click (PC) to adjust.", false, 31, ModType.Toggle, false)]
internal class BackTrackDelay : MonoBehaviour
{
	private const string LabelPrefix = "Back Track Delay: ";
	private const string ValueFormat = "F2";
	private const float InputRepeatDelay = 0.02f;
	private const float AdjustmentStep = 0.005f;
	private const float MinimumDelay = 0.05f;
	private const float MaximumDelay = 3f;

	private static readonly float[] DesktopDelayPresets =
	{
		0.1f,
		0.2f,
		0.3f,
		0.5f,
		0.75f,
		1f,
		1.5f,
		2f
	};

	private static int _desktopPresetIndex = 3;

	private float _lastInputTime;
	private float _lastDisplayedDelay;

	private void OnEnable()
	{
		if (!XRSettings.isDeviceActive)
		{
			_desktopPresetIndex = (_desktopPresetIndex + 1) % DesktopDelayPresets.Length;
			BackTrack.DelaySeconds = DesktopDelayPresets[_desktopPresetIndex];
			UpdateLabel();
			Object.Destroy(this);
			return;
		}

		_lastDisplayedDelay = BackTrack.DelaySeconds;
		UpdateLabel();
	}

	private void Update()
	{
		if (Time.time - _lastInputTime < InputRepeatDelay)
		{
			return;
		}

		_lastInputTime = Time.time;
		float rightTrigger = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat;
		float leftTrigger = ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat;

		if (rightTrigger > 0.5f)
		{
			BackTrack.DelaySeconds = Mathf.Min(BackTrack.DelaySeconds + AdjustmentStep, MaximumDelay);
		}
		if (leftTrigger > 0.5f)
		{
			BackTrack.DelaySeconds = Mathf.Max(BackTrack.DelaySeconds - AdjustmentStep, MinimumDelay);
		}

		if (Mathf.Abs(BackTrack.DelaySeconds - _lastDisplayedDelay) > 0.01f)
		{
			_lastDisplayedDelay = BackTrack.DelaySeconds;
			UpdateLabel();
		}
	}

	private static void UpdateLabel()
	{
		BundleManager.SetBacktrackDelayStatusText(LabelPrefix + BackTrack.DelaySeconds.ToString(ValueFormat));
	}

	private void OnDisable()
	{
		UpdateLabel();
	}
}
