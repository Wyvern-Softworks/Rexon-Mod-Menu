// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.TeleportToBlocks
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Block;

[Mod("Teleport To Building Blocks Map", "Block Mods", "Teleports to blocks area.", false, 1, ModType.Action, false)]
internal class TeleportToBlocks : MonoBehaviour
{
	private void OnEnable()
	{
		MonkeAgent.instance.StartCoroutine(BlockRoomTeleporter.TeleportSequence());
	}
}
