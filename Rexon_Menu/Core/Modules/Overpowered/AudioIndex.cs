// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.AudioIndex
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Audio Index: 213", "Overpowered", "Triggers (VR) / Click (PC) to adjust.", false, 30, ModType.Toggle, false)]
internal class AudioIndex : MonoBehaviour
{
	private const string LabelFormat = "Audio Index: {0}";
	private const int MinimumIndex = 0;
	private const int MaximumIndex = 300;
	private const float InputRepeatDelay = 0.02f;

	public static int CurrentIndex = 213;

	private float _lastInputTime;
	private int _lastDisplayedIndex;


	private void OnEnable()
	{
		if (!XRSettings.isDeviceActive)
		{
			CurrentIndex += 10;
			if (CurrentIndex > MaximumIndex)
			{
				CurrentIndex = MinimumIndex;
			}

			UpdateLabel();
			ConfigurationManager.SaveIfAutoLoadEnabled();
			Object.Destroy(this);
			return;
		}

		_lastDisplayedIndex = CurrentIndex;
		UpdateLabel();
	}

	private void Update()
	{
		if (Time.time < _lastInputTime + InputRepeatDelay)
		{
			return;
		}

		float rightTrigger = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat;
		float leftTrigger = ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat;

		if (rightTrigger > 0.5f)
		{
			CurrentIndex = Mathf.Clamp(CurrentIndex + 1, MinimumIndex, MaximumIndex);
			_lastInputTime = Time.time;
		}
		else if (leftTrigger > 0.5f)
		{
			CurrentIndex = Mathf.Clamp(CurrentIndex - 1, MinimumIndex, MaximumIndex);
			_lastInputTime = Time.time;
		}

		if (_lastDisplayedIndex != CurrentIndex)
		{
			_lastDisplayedIndex = CurrentIndex;
			UpdateLabel();
		}
	}

	private static void UpdateLabel()
	{
		BundleManager.SetAudioIndexStatusText(string.Format(LabelFormat, CurrentIndex));
	}

	private void OnDisable()
	{
		UpdateLabel();
		ConfigurationManager.SaveIfAutoLoadEnabled();
	}
}

