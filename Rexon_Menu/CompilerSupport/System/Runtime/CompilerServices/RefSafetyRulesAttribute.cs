// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: System.Runtime.CompilerServices.RefSafetyRulesAttribute
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices;

[Embedded]
[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
internal sealed class RefSafetyRulesAttribute : Attribute
{
	public readonly int Version;

	public RefSafetyRulesAttribute(int version)
	{
		Version = version;
	}
}
