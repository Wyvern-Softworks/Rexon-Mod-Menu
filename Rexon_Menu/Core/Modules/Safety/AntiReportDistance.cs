// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Safety.AntiReportDistance
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Safety;

[Mod("Anti Report Distance", "Safety", "Triggers (VR) / Click (PC) to adjust.", false, 2, ModType.Toggle, false)]
internal sealed class AntiReportDistance : MonoBehaviour
{
	private const string LabelFormat = "Anti Report Distance: {0:F1}";
	private const float MinimumDistance = 0.1f;
	private const float MaximumDistance = 3f;
	private const float VrStep = 0.05f;
	private const float DesktopStep = 0.1f;
	private const float InputRepeatSeconds = 0.01f;

	private float _lastAdjustmentTime;
	private float _displayedDistance;

	private void OnEnable()
	{
		if (!XRSettings.isDeviceActive)
		{
			AntiReport.ReportDistance += DesktopStep;
			if (AntiReport.ReportDistance > MaximumDistance)
			{
				AntiReport.ReportDistance = MinimumDistance;
			}

			AntiReport.ReportDistance =
				Mathf.Round(AntiReport.ReportDistance * 10f) / 10f;
			UpdateLabel();
			Object.Destroy(this);
			return;
		}

		_displayedDistance = AntiReport.ReportDistance;
		UpdateLabel();
	}

	private void Update()
	{
		if (GorillaTagger.Instance == null ||
			GorillaTagger.Instance.offlineVRRig == null ||
			Time.time <= _lastAdjustmentTime + InputRepeatSeconds)
		{
			return;
		}

		float rightTrigger = ControllerInputPoller.instance.rightControllerIndexFloat;
		float leftTrigger = ControllerInputPoller.instance.leftControllerIndexFloat;

		if (rightTrigger > 0.5f)
		{
			_lastAdjustmentTime = Time.time;
			AntiReport.ReportDistance = Mathf.Clamp(
				AntiReport.ReportDistance + VrStep,
				MinimumDistance,
				MaximumDistance);
		}
		else if (leftTrigger > 0.5f)
		{
			_lastAdjustmentTime = Time.time;
			AntiReport.ReportDistance = Mathf.Clamp(
				AntiReport.ReportDistance - VrStep,
				MinimumDistance,
				MaximumDistance);
		}

		if (Mathf.Abs(_displayedDistance - AntiReport.ReportDistance) > 0.01f)
		{
			_displayedDistance = AntiReport.ReportDistance;
			UpdateLabel();
		}
	}

	private static void UpdateLabel()
	{
		BundleManager.SetAntiReportDistanceStatusText(string.Format(LabelFormat, AntiReport.ReportDistance));
	}

	private void OnDisable()
	{
		UpdateLabel();
	}
}
