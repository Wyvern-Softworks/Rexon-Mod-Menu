// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Interface.MenuButtonHandler
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rexon_Menu.Interface;

internal class MenuButtonHandler : MonoBehaviour
{
	public bool IsCategoryButton;
	public string CategoryName;
	public int ModIndex;
	public ModType TargetModType;
	public bool IsNavigationButton;
	public string NavigationAction;
	public bool UseLegacyToggle;
	public bool IsSoundboardAudio;
	public string SoundboardAudioName;

	private bool touchingHandIsLeft;

	private void OnTriggerEnter(Collider other)
	{
		if (other == null)
		{
			return;
		}

		bool isMenuPointer = Main.MenuPointer != null && other.gameObject == Main.MenuPointer;
		bool isKeyboardHand = VRKeyboard.IsKeyboardPointer(other);
		if (!isMenuPointer && !isKeyboardHand)
		{
			return;
		}

		touchingHandIsLeft = isMenuPointer
			? !MenuHandSetting.IsLeftHand
			: VRKeyboard.IsLeftPointer(other);

		if (NavigationAction == "Leave")
		{
			VibrateTouchingHand();
			PlayClickSound();
			InvokeButtonClick();
			return;
		}

		if (!BundleManager.TryAcquireButtonClickCooldown())
		{
			return;
		}

		VibrateTouchingHand();
		PlayClickSound();
		HandleButtonAction();
	}

	private void VibrateTouchingHand()
	{
		GorillaTagger.Instance.StartVibration(
			touchingHandIsLeft,
			GorillaTagger.Instance.tagHapticStrength / 2f,
			GorillaTagger.Instance.tagHapticDuration / 2f);
	}

	private static void PlayClickSound()
	{
		bool rightHand = !MenuHandSetting.IsLeftHand;
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(106, rightHand, 0.4f);
	}

	private void HandleButtonAction()
	{
		if (IsNavigationButton)
		{
			HandleNavigationAction();
		}
		else if (IsCategoryButton && !string.IsNullOrEmpty(CategoryName))
		{
			BundleManager.OpenCategory(CategoryName);
		}
		else if (NavigationAction == "SoundboardHearSelf")
		{
			ToggleSoundboardEcho();
		}
		else if (IsSoundboardAudio && !string.IsNullOrEmpty(SoundboardAudioName))
		{
			ToggleSoundboardAudio();
		}
		else if (ModIndex >= 0)
		{
			Button button = GetComponent<Button>();
			if (button != null)
			{
				BundleManager.ToggleMod(ModIndex, button, UseLegacyToggle);
			}
		}
	}

	private void HandleNavigationAction()
	{
		switch (NavigationAction)
		{
			case "Forward":
				BundleManager.ChangePage(1);
				break;
			case "Previous":
				BundleManager.ChangePage(-1);
				break;
			case "Home":
				BundleManager.GoHome();
				break;
			case "KickMaster":
			case "Search":
				InvokeButtonClick();
				break;
		}
	}

	private void ToggleSoundboardEcho()
	{
		SoundboardAudioManager.HearSelf = !SoundboardAudioManager.HearSelf;
		BundleManager.SetSoundboardStatusText(SoundboardAudioManager.HearSelf ? "Hear Self: On" : "Hear Self: Off");

		Button button = GetComponent<Button>();
		if (button != null)
		{
			BundleManager.SetButtonEnabledVisual(button, SoundboardAudioManager.HearSelf);
		}

		ConfigurationManager.SaveIfAutoLoadEnabled();
	}

	private void ToggleSoundboardAudio()
	{
		if (SoundboardAudioManager.IsPlaying(SoundboardAudioName))
		{
			SoundboardAudioManager.Stop();
		}
		else
		{
			SoundboardAudioManager.Play(SoundboardAudioName);
		}

		BundleManager.RefreshMenu();
	}

	private void InvokeButtonClick()
	{
		Button button = GetComponent<Button>();
		if (button != null)
		{
			button.onClick.Invoke();
		}
	}
}
