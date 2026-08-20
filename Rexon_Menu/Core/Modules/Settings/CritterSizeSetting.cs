// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CritterSizeSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Critter Size: 1.00", "Settings", "Click to adjust critter size.", false, 18, ModType.Toggle, false)]
internal class CritterSizeSetting : MonoBehaviour
{
	private const string LabelPrefix = "Critter Size: ";
	private const string ValueFormat = "F2";

	public static float CurrentScale = 1f;

	private static int _currentPresetIndex = 2;

	private static readonly float[] ScalePresets = { 0.25f, 0.5f, 1f, 1.5f, 2f, 3f, 5f };


	private void OnEnable()
	{
		_currentPresetIndex = (_currentPresetIndex + 1) % ScalePresets.Length;
		CurrentScale = ScalePresets[_currentPresetIndex];
		BundleManager.SetCritterSizeStatusText(LabelPrefix + CurrentScale.ToString(ValueFormat));
		Object.Destroy(this);
	}
}
