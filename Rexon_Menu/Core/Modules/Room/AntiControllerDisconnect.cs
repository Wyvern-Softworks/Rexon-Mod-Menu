// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Room.AntiControllerDisconnect
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Reflection;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

using Controller = OVRInput.Controller;

namespace Rexon_Menu.Core.Modules.Room;

[Mod("Anti Controller Disconnect", "Room", "Prevents controller disconnect detection.", false, 10, ModType.Toggle, false)]
internal class AntiControllerDisconnect : MonoBehaviour
{
	private FieldInfo _fieldInfo1;

	private FieldInfo _fieldInfo2;


	private void OnEnable()
	{
		_fieldInfo1 = typeof(ConnectedControllerHandler).GetField("overrideLeftEnable", BindingFlags.Instance | BindingFlags.NonPublic);
		_fieldInfo2 = typeof(ConnectedControllerHandler).GetField("overrideRightEnable", BindingFlags.Instance | BindingFlags.NonPublic);
	}

	private void Update()
	{
		if (XRSettings.isDeviceActive && ConnectedControllerHandler.Instance != null)
		{
			if (_fieldInfo1 != null)
			{
				_fieldInfo1.SetValue(ConnectedControllerHandler.Instance, true);
			}
			if (_fieldInfo2 != null)
			{
				_fieldInfo2.SetValue(ConnectedControllerHandler.Instance, true);
			}
		}
	}

	private void OnDisable()
	{
		if (ConnectedControllerHandler.Instance != null)
		{
			if (_fieldInfo1 != null)
			{
				_fieldInfo1.SetValue(ConnectedControllerHandler.Instance, false);
			}
			if (_fieldInfo2 != null)
			{
				_fieldInfo2.SetValue(ConnectedControllerHandler.Instance, false);
			}
		}
	}
}
