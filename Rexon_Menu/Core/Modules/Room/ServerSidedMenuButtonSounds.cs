// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Room.ServerSidedMenuButtonSounds
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Room;

[Mod("Server Sided Menu Button Sounds", "Room", "Other players hear your menu button clicks.", false, 9, ModType.Toggle, false)]
internal class ServerSidedMenuButtonSounds : MonoBehaviour
{
	internal static bool _enabled;


	public static bool Enabled
	{
		get
		{
			return _enabled;
		}
		private set
		{
			_enabled = value;
		}
	}

	private void OnEnable()
	{
		_enabled = true;
	}

	private void OnDisable()
	{
		_enabled = false;
	}
}
