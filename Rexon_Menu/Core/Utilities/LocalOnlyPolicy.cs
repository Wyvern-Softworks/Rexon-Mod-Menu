// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Utilities.LocalOnlyPolicy
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;

namespace Rexon_Menu.Core.Utilities;

internal static class LocalOnlyPolicy
{
	public const string LocalAuthorizationKey = "LOCAL";
	public const string LocalDeviceIdentity = "LOCAL";
	public const string EmbeddedEarapeAudioResourceName = "Rexon_Menu.Resources.earape.mp3";
	public const string EmbeddedEarapeAudioSha256 = "FB2434E668C5AD2AA819D557B9403270C1DC04608E4966D6AFFF37F0BD3027EC";

	public static string DataDirectory =>
		Path.Combine(Paths.ConfigPath, "Rexon_Menu", "LocalData");

	public static string EarapeAudioPath =>
		Path.Combine(DataDirectory, "EAR_RAPE_DO_NOT_PLAY.mp3");

	public static void EnsureDataDirectory()
	{
		try
		{
			Directory.CreateDirectory(DataDirectory);
		}
		catch (Exception)
		{
		}
	}

	public static bool TryPrepareEmbeddedEarapeAudio(out string audioPath)
	{
		audioPath = EarapeAudioPath;
		try
		{
			EnsureDataDirectory();
			byte[] embeddedAudio;
			Assembly assembly = typeof(LocalOnlyPolicy).Assembly;
			using (Stream resourceStream = assembly.GetManifestResourceStream(EmbeddedEarapeAudioResourceName))
			{
				if (resourceStream == null)
				{
					return false;
				}

				using (MemoryStream buffer = new MemoryStream())
				{
					resourceStream.CopyTo(buffer);
					embeddedAudio = buffer.ToArray();
				}
			}

			if (!HasExpectedEarapeAudioHash(embeddedAudio))
			{
				return false;
			}

			if (File.Exists(audioPath))
			{
				using (FileStream existingAudio = File.OpenRead(audioPath))
				{
					if (HasExpectedEarapeAudioHash(existingAudio))
					{
						return true;
					}
				}
			}

			File.WriteAllBytes(audioPath, embeddedAudio);
			using (FileStream extractedAudio = File.OpenRead(audioPath))
			{
				return HasExpectedEarapeAudioHash(extractedAudio);
			}
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static bool HasExpectedEarapeAudioHash(byte[] audioBytes)
	{
		using (SHA256 sha256 = SHA256.Create())
		{
			return IsExpectedEarapeAudioHash(sha256.ComputeHash(audioBytes));
		}
	}

	private static bool HasExpectedEarapeAudioHash(Stream audioStream)
	{
		using (SHA256 sha256 = SHA256.Create())
		{
			return IsExpectedEarapeAudioHash(sha256.ComputeHash(audioStream));
		}
	}

	private static bool IsExpectedEarapeAudioHash(byte[] hash)
	{
		string actualHash = BitConverter.ToString(hash).Replace("-", string.Empty);
		return string.Equals(actualHash, EmbeddedEarapeAudioSha256, StringComparison.Ordinal);
	}
}
