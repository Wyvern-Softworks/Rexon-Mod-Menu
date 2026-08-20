// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntiReportType
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Anti Report Type", "Safety", "", true, 3, ModType.Toggle, false)]
internal class AntiReportType : MonoBehaviour
{
	private void OnEnable()
	{
		AntiReport.ResponseModeIndex = (AntiReport.ResponseModeIndex + 1) % AntiReport.ResponseModeNames.Length;
		BundleManager.SetAntiReportTypeStatusText("Anti Report Type: " + AntiReport.ResponseModeNames[AntiReport.ResponseModeIndex]);
		Object.Destroy(this);
	}
}
