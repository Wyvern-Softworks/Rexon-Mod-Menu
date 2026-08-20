// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class IgnoresAccessChecksToAttribute : Attribute
{
	public IgnoresAccessChecksToAttribute(string assemblyName)
	{
		AssemblyName = assemblyName;
	}

	public string AssemblyName { get; }
}
