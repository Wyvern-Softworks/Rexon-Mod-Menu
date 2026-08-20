// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.DisableNetworkTriggers
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Disable Network Triggers [CS]", "Overpowered", "Disables network join triggers client-side.", false, 27, ModType.Toggle, false)]
internal class DisableNetworkTriggers : MonoBehaviour
{
	private const string JoinRoomTriggersPath = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab";

	private bool _triggersDisabled;


	private void OnEnable()
	{
		_triggersDisabled = false;
	}

	private void Update()
	{
		if (!_triggersDisabled)
		{
			_triggersDisabled = true;
			GameObject joinRoomTriggers = GameObject.Find(JoinRoomTriggersPath);
			if (joinRoomTriggers != null)
			{
				joinRoomTriggers.SetActive(false);
			}
		}
	}

	private void OnDisable()
	{
		_triggersDisabled = false;
		GameObject joinRoomTriggers = GameObject.Find(JoinRoomTriggersPath);
		if (joinRoomTriggers != null)
		{
			joinRoomTriggers.SetActive(true);
		}
	}
}
