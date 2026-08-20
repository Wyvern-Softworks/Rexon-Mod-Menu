// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.FortyFiveHZ
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Threading;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("45 FPS Lock", "World", "Locks frame rate to ~45fps.", false, 39, ModType.Toggle, false)]
internal class FortyFiveHZ : MonoBehaviour
{
	private void Update()
	{
		Thread.Sleep(12);
	}
}
