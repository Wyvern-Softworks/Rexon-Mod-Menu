// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.LowHeartz
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Threading;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("30 FPS Lock", "World", "Locks frame rate to ~30fps.", false, 40, ModType.Toggle, false)]
internal class LowHeartz : MonoBehaviour
{

	private void Update()
	{
		Thread.Sleep(16);
	}
}
