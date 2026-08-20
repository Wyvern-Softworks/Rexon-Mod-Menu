// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.MultiBlockGrab
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

using HandState = BuilderPieceInteractor.HandState;

namespace Rexon_Menu.Core.Modules.Block;

[Mod("Multi Building Block Grab [Block Map]", "Block Mods", "Grab multiple blocks at once.", false, 3, ModType.Toggle, false)]
internal class MultiBlockGrab : MonoBehaviour
{
	private void Update()
	{
		if (BuilderPieceInteractor.instance != null)
		{
			BuilderPieceInteractor.instance.handState[1] = (HandState)0;
			BuilderPieceInteractor.instance.heldPiece[1] = null;
		}
	}
}
