// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.EarapeAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using Photon.Pun;
using Photon.Voice.Unity;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Utilities;
using UnityEngine;
using UnityEngine.Networking;

using InputSourceType = Photon.Voice.Unity.Recorder.InputSourceType;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Earape All", "Overpowered", "Replaces mic with earape audio file.", false, 29, ModType.Toggle, false)]
internal class EarapeAll : MonoBehaviour
{
	private bool _audioReplacementStarted;


	private void Update()
	{
		if (!PhotonNetwork.InRoom || _audioReplacementStarted)
		{
			return;
		}
		Recorder myRecorder = GorillaTagger.Instance.myRecorder;
		if (myRecorder != null)
		{
			if (!LocalOnlyPolicy.TryPrepareEmbeddedEarapeAudio(out string audioPath))
			{
				return;
			}
			_audioReplacementStarted = true;
			GorillaTagger.Instance.StartCoroutine(LoadAndPlay(audioPath, myRecorder));
		}
	}

	private IEnumerator LoadAndPlay(string audioPath, Recorder recorder)
	{
		using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + audioPath, AudioType.UNKNOWN))
		{
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				yield break;
			}

			AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
			if (clip == null)
			{
				yield break;
			}

			recorder.SourceType = InputSourceType.AudioClip;
			recorder.AudioClip = clip;
			recorder.LoopAudioClip = true;
			recorder.DebugEchoMode = false;
			yield return new WaitForSeconds(0.5f);
			recorder.RestartRecording(true);
			recorder.TransmitEnabled = true;
		}
	}

	private void OnDisable()
	{
		RestoreAudio();
	}

	private void OnDestroy()
	{
		RestoreAudio();
	}

	private void RestoreAudio()
	{
		_audioReplacementStarted = false;
		Recorder myRecorder = GorillaTagger.Instance.myRecorder;
		if (myRecorder != null && myRecorder.SourceType == InputSourceType.AudioClip)
		{
			myRecorder.SourceType = InputSourceType.Microphone;
			myRecorder.DebugEchoMode = false;
			myRecorder.RestartRecording(true);
			myRecorder.TransmitEnabled = true;
		}
	}
}
