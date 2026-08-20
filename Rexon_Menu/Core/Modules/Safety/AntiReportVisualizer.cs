// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntiReportVisualizer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Anti Report Visualizer", "Safety", "Shows report button range.", false, 4, ModType.Toggle, false)]
internal class AntiReportVisualizer : MonoBehaviour
{

	private void OnEnable()
	{
		AntiReport.ShowReportZones = true;
	}

	private void OnDisable()
	{
		AntiReport.ShowReportZones = false;
	}
}
