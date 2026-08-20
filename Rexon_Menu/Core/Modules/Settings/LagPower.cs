// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Settings.LagPower
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Settings;

[Mod("Lag Power", "Settings", "Triggers (VR) / Click (PC) to adjust.", false, 14, ModType.Toggle, false)]
internal sealed class LagPower : MonoBehaviour
{
	private const string LabelFormat = "Lag Power: {0}";

	internal static int Power = 400;

	private float _lastAdjustmentAt;
	private int _displayedPower;

	private void OnEnable()
	{
		if (!XRSettings.isDeviceActive)
		{
			Power += 100;
			if (Power > 1500)
			{
				Power = 100;
			}

			UpdateLabel();
			ConfigurationManager.SaveIfAutoLoadEnabled();
			Destroy(this);
			return;
		}

		_displayedPower = Power;
		UpdateLabel();
	}

	private void Update()
	{
		if (Time.time <= _lastAdjustmentAt + 0.02f)
		{
			return;
		}

		float rightTrigger = ControllerInputPoller.instance.rightControllerIndexFloat;
		float leftTrigger = ControllerInputPoller.instance.leftControllerIndexFloat;
		if (rightTrigger > 0.5f)
		{
			_lastAdjustmentAt = Time.time;
			Power = Mathf.Clamp(Power + 10, 100, 1500);
		}
		else if (leftTrigger > 0.5f)
		{
			_lastAdjustmentAt = Time.time;
			Power = Mathf.Clamp(Power - 10, 100, 1500);
		}

		if (_displayedPower != Power)
		{
			_displayedPower = Power;
			UpdateLabel();
		}
	}

	private static void UpdateLabel()
	{
		BundleManager.SetLagPowerStatusText(string.Format(LabelFormat, Power));
	}

	private void OnDisable()
	{
		UpdateLabel();
		ConfigurationManager.SaveIfAutoLoadEnabled();
	}
}
