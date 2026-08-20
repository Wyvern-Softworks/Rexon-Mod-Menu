// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BlastLobberGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;

namespace Recovered.Obfuscated;

[Mod("Blast Lobber Gun", "Super Infection/Casual [MASTERCLIENT]", "Spawns blast lobbers.", false, 19, ModType.Toggle, false)]
internal class BlastLobberGun : GadgetGunBase
{
	protected override string ModId => "BlastLobberGun";

	protected override string GadgetName => "BlastLobberGadget";
}
