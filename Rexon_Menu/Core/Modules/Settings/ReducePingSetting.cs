// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ReducePingSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Reduce Ping", "Settings", "Increases network send rate.", false, 17, ModType.Toggle, false)]
internal class ReducePingSetting : MonoBehaviour
{
	private int _originalSendRate;

	private int _originalSerializationRate;

	private void OnEnable()
	{
		_originalSendRate = PhotonNetwork.SendRate;
		_originalSerializationRate = PhotonNetwork.SerializationRate;
		PhotonNetwork.SendRate = 1000;
		PhotonNetwork.SerializationRate = 1000;
	}

	private void Update()
	{
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private void OnDisable()
	{
		if (_originalSendRate != -1)
		{
			PhotonNetwork.SendRate = _originalSendRate;
		}
		if (_originalSerializationRate != -1)
		{
			PhotonNetwork.SerializationRate = _originalSerializationRate;
		}
	}
}

