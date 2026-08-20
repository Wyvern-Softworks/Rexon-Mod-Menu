// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Patches.ShaderPatch
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Reflection;
using Rexon_Shader;
using UnityEngine;

namespace Rexon_Menu.Core.Patches;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
internal class ShaderPatch
{
	public static bool Skip;

	internal static Shader Cached => ShaderBridge.Cached;

	internal static void EnsureCached()
	{
		if (ShaderBridge.Cached == null)
		{
			ShaderBridge.Initialize();
		}
	}

	internal static Material CreateTransparentMaterial(Color color)
	{
		return ShaderBridge.CreateTransparentMaterial(color);
	}
}
