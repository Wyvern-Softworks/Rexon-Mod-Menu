// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SilentHandTaps
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Silent Hand Taps", "Rig", "Others can't hear your hand taps.", false, 39, ModType.Toggle, false)]
internal class SilentHandTaps : MonoBehaviour
{
	private bool _tapTimesRestored;


	private void Update()
	{
		GorillaTagger.Instance.lastLeftTap = 1E+10f;
		GorillaTagger.Instance.lastRightTap = 1E+10f;
		_tapTimesRestored = false;
	}

	private void OnDisable()
	{
		if (!_tapTimesRestored)
		{
			_tapTimesRestored = true;
			GorillaTagger.Instance.lastLeftTap = 0f;
			GorillaTagger.Instance.lastRightTap = 0f;
		}
	}
}

