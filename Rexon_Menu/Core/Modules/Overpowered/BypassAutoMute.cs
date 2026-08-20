// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.BypassAutoMute
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaNetworking;
using Photon.Pun;
using Photon.Voice.Unity;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Bypass Auto Mute", "Overpowered", "Bypasses auto mute system.", false, 21, ModType.Toggle, false)]
internal sealed class BypassAutoMute : MonoBehaviour
{
	private const string AutoMuteDisabled = "OFF";
	private const string AutoMutePreference = "autoMute";

	private float _lastMaintenanceAt;
	private float _previousLoudness;
	private float _silenceStartedAt = -1f;
	private bool _recordingRestarted;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastMaintenanceAt + 0.1f)
		{
			return;
		}

		_lastMaintenanceAt = Time.time;
		GorillaTagger.moderationMutedTime = -1f;

		GorillaComputer computer = GorillaComputer.instance;
		if (computer.autoMuteType != AutoMuteDisabled)
		{
			computer.autoMuteType = AutoMuteDisabled;
			PlayerPrefs.SetInt(AutoMutePreference, 0);
			PlayerPrefs.Save();
		}

		Recorder recorder = GorillaTagger.Instance.myRecorder;
		if (recorder == null || (int)recorder.SourceType != 0)
		{
			return;
		}

		GorillaSpeakerLoudness speaker = VRRig.LocalRig.GetComponent<GorillaSpeakerLoudness>();
		if (speaker == null)
		{
			return;
		}

		float loudness = speaker.Loudness;
		if (loudness != 0f)
		{
			_silenceStartedAt = -1f;
			_recordingRestarted = false;
			_previousLoudness = loudness;
			return;
		}

		if (_previousLoudness != 0f)
		{
			_silenceStartedAt = Time.time;
			_recordingRestarted = false;
		}

		if (_silenceStartedAt > 0f
			&& !_recordingRestarted
			&& Time.time - _silenceStartedAt >= 0.25f)
		{
			recorder.RestartRecording(true);
			_recordingRestarted = true;
		}

		_previousLoudness = loudness;
	}
}
