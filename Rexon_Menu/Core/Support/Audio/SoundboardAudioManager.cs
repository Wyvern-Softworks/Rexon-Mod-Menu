// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SoundboardAudioManager
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace Recovered.Obfuscated;

internal static class SoundboardAudioManager
{
	private static readonly string[] SupportedExtensions = { ".mp3", ".wav", ".ogg", ".m4a" };
	private static readonly List<string> CachedFileNames = new List<string>();
	private static readonly List<AudioClip> CachedClips = new List<AudioClip>();

	private static string audioDirectory;
	private static string currentlyPlaying;
	private static bool hearSelf = true;

	public static string CurrentlyPlaying => currentlyPlaying;

	public static bool HearSelf
	{
		get => hearSelf;
		set
		{
			hearSelf = value;
			if (currentlyPlaying != null && GorillaTagger.Instance != null)
			{
				Recorder recorder = GorillaTagger.Instance.myRecorder;
				if (recorder != null && recorder.SourceType == Recorder.InputSourceType.AudioClip)
				{
					recorder.DebugEchoMode = value;
				}
			}
		}
	}

	public static void InitializeAudioDirectory()
	{
		audioDirectory = Path.Combine(Paths.PluginPath, "RexonAudios");
		if (!Directory.Exists(audioDirectory))
		{
			Directory.CreateDirectory(audioDirectory);
		}
	}

	public static string[] GetAvailableAudioNames()
	{
		EnsureAudioDirectory();
		if (!Directory.Exists(audioDirectory))
		{
			return Array.Empty<string>();
		}

		List<string> names = new List<string>();
		foreach (string filePath in Directory.GetFiles(audioDirectory))
		{
			string extension = Path.GetExtension(filePath);
			if (string.IsNullOrEmpty(extension) || Array.IndexOf(SupportedExtensions, extension.ToLowerInvariant()) < 0)
			{
				continue;
			}

			string name = Path.GetFileNameWithoutExtension(filePath);
			if (!string.IsNullOrEmpty(name))
			{
				names.Add(name);
			}
		}

		return names.ToArray();
	}

	public static bool IsPlaying(string audioName)
	{
		return currentlyPlaying == audioName;
	}

	public static void Play(string audioName)
	{
		if (currentlyPlaying != null && currentlyPlaying != audioName)
		{
			Stop();
		}

		EnsureAudioDirectory();
		string filePath = FindAudioFile(audioName);
		if (filePath == null || !File.Exists(filePath))
		{
			return;
		}

		currentlyPlaying = audioName;
		GorillaTagger.Instance.StartCoroutine(LoadAndPlayAudio(filePath, audioName));
	}

	private static void EnsureAudioDirectory()
	{
		if (string.IsNullOrEmpty(audioDirectory))
		{
			InitializeAudioDirectory();
		}
	}

	private static string FindAudioFile(string audioName)
	{
		if (audioDirectory == null)
		{
			return null;
		}

		foreach (string extension in SupportedExtensions)
		{
			string candidate = Path.Combine(audioDirectory, audioName + extension);
			if (File.Exists(candidate))
			{
				return candidate;
			}
		}

		return null;
	}

	private static IEnumerator LoadAndPlayAudio(string filePath, string requestedAudioName)
	{
		string fileName = Path.GetFileName(filePath);
		int cachedIndex = CachedFileNames.IndexOf(fileName);

		if (cachedIndex >= 0)
		{
			if (currentlyPlaying == requestedAudioName && cachedIndex < CachedClips.Count)
			{
				ApplyClipToRecorder(CachedClips[cachedIndex]);
			}
			yield break;
		}

		UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, (AudioType)0);
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
			CachedFileNames.Add(fileName);
			CachedClips.Add(clip);
			if (currentlyPlaying == requestedAudioName)
			{
				ApplyClipToRecorder(clip);
			}
		}
	}

	public static void Stop()
	{
		currentlyPlaying = null;
		if (GorillaTagger.Instance == null)
		{
			return;
		}

		Recorder recorder = GorillaTagger.Instance.myRecorder;
		if (recorder != null && recorder.SourceType == Recorder.InputSourceType.AudioClip)
		{
			recorder.SourceType = Recorder.InputSourceType.Microphone;
			recorder.DebugEchoMode = false;
			recorder.RestartRecording(true);
			recorder.TransmitEnabled = true;
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}

	internal static void ApplyClipToRecorder(AudioClip clip)
	{
		if (clip == null || GorillaTagger.Instance == null)
		{
			return;
		}

		Recorder recorder = GorillaTagger.Instance.myRecorder;
		if (recorder == null)
		{
			return;
		}

		recorder.SourceType = Recorder.InputSourceType.AudioClip;
		recorder.AudioClip = clip;
		recorder.LoopAudioClip = true;
		recorder.DebugEchoMode = hearSelf;
		recorder.RestartRecording(true);
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
