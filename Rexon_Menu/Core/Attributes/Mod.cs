// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Attributes.Mod
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;

namespace Rexon_Menu.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class Mod : Attribute
{
	public string Name { get; set; }

	public string Category { get; set; }

	public string Description { get; set; }

	public bool DefaultEnabled { get; set; }

	public int Order { get; set; }

	public ModType Type { get; set; }

	public bool PCOnly { get; set; }

	public Mod(string name, string category, string description, bool defaultEnabled, int order, ModType type = ModType.Toggle, bool pcOnly = false)
	{
		Name = name;
		Category = category;
		Description = description;
		DefaultEnabled = defaultEnabled;
		Order = order;
		Type = type;
		PCOnly = pcOnly;
	}
}
