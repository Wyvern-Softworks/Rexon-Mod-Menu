// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.PlatformGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;

namespace Recovered.Obfuscated;

[Mod("Platform Gun", "Super Infection/Casual [MASTERCLIENT]", "Spawns platforms.", false, 6, ModType.Toggle, false)]
internal class PlatformGun : GadgetGunBase
{

	protected override string ModId => "PlatformGun";

	protected override string GadgetName => "PlatformDeployerGadget";
}
