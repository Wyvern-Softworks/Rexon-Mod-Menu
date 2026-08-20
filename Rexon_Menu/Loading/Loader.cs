// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Loading.Loader
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Rexon_Menu.Core;
using Rexon_Menu_Mat;

namespace Loading;

public static class Loader
{
	private static int _startupEntered;

	public static void Load()
	{
		if (!EmbeddedCompanionAssemblyLoader.TryLoadRequiredAssemblies())
		{
			return;
		}

		ContinueStartup();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ContinueStartup()
	{
		if (Interlocked.CompareExchange(ref _startupEntered, 1, 0) != 0)
		{
			return;
		}

		MatBridge.Initialize();
		Bootstrapper.Run();
	}
}

internal static class EmbeddedCompanionAssemblyLoader
{
	private const string MatResourceName = "Rexon_Menu.Resources.dependencies.Rexon-Menu-Mat.dll";
	private const string ShaderResourceName = "Rexon_Menu.Resources.dependencies.Rexon-Shader.dll";
	private const string MatAssemblyName = "Rexon-Menu-Mat";
	private const string ShaderAssemblyName = "Rexon-Shader";

	private static readonly object LoadGate = new object();
	private static Assembly _matAssembly;
	private static Assembly _shaderAssembly;

	internal static bool TryLoadRequiredAssemblies()
	{
		lock (LoadGate)
		{
			if (_matAssembly == null)
			{
				_matAssembly = LoadEmbeddedAssembly(MatResourceName, MatAssemblyName);
			}

			if (_matAssembly == null)
			{
				return false;
			}

			if (_shaderAssembly == null)
			{
				_shaderAssembly = LoadEmbeddedAssembly(ShaderResourceName, ShaderAssemblyName);
			}

			return _shaderAssembly != null;
		}
	}

	private static Assembly LoadEmbeddedAssembly(string resourceName, string expectedAssemblyName)
	{
		try
		{
			byte[] assemblyBytes;
			Assembly hostAssembly = typeof(Loader).Assembly;
			using (Stream resourceStream = hostAssembly.GetManifestResourceStream(resourceName))
			{
				if (resourceStream == null)
				{
					return null;
				}

				using (MemoryStream buffer = new MemoryStream())
				{
					resourceStream.CopyTo(buffer);
					assemblyBytes = buffer.ToArray();
				}
			}

			Assembly loadedAssembly = Assembly.Load(assemblyBytes);
			if (!string.Equals(loadedAssembly.GetName().Name, expectedAssemblyName, StringComparison.Ordinal)
				|| !string.IsNullOrEmpty(loadedAssembly.Location))
			{
				return null;
			}

			return loadedAssembly;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
