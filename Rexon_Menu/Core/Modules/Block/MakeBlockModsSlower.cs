// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.MakeBlockModsSlower
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Block;

[Mod("Make Block Mods Slower", "Block Mods", "Reduces block RPC kicks.", false, 2, ModType.Toggle, false)]
internal class MakeBlockModsSlower : MonoBehaviour
{
	internal static bool Enabled { get; private set; }

	private void OnEnable()
	{
		Enabled = true;
	}

	private void OnDisable()
	{
		Enabled = false;
	}
}
