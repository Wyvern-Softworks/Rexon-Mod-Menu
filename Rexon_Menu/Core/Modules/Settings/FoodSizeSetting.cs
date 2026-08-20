// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FoodSizeSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Food Size: 1.00", "Settings", "Click to adjust food size.", false, 19, ModType.Toggle, false)]
internal class FoodSizeSetting : MonoBehaviour
{
	private const string LabelPrefix = "Food Size: ";
	private const string ValueFormat = "F2";

	private static readonly float[] ScalePresets = { 0.5f, 1f, 2f, 5f, 10f, 50f, 150f };
	private static int _presetIndex = 1;

	internal static float CurrentScale = 1f;

	private void OnEnable()
	{
		_presetIndex = (_presetIndex + 1) % ScalePresets.Length;
		CurrentScale = ScalePresets[_presetIndex];
		BundleManager.SetFoodSizeStatusText(LabelPrefix + CurrentScale.ToString(ValueFormat));
		Object.Destroy(this);
	}
}
