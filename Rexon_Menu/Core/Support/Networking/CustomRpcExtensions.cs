// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CustomRpcExtensions
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu_Mat;

namespace Recovered.Obfuscated;

public static class CustomRpcExtensions
{
	private const byte RpcEventCode = 200;
	private const int DefaultTimestamp = -2147483646;

	public static bool SendRpc(this PhotonView view, string rpcName, int[] targetActorNumbers, params object[] parameters)
	{
		return SendRpcCore(view, rpcName, RpcTarget.AllBuffered, null, targetActorNumbers, parameters);
	}

	public static bool SendRpc(this PhotonView view, string rpcName, Player targetPlayer, params object[] parameters)
	{
		return SendRpcCore(view, rpcName, RpcTarget.AllBuffered, targetPlayer, null, parameters);
	}

	public static bool SendRpc(this PhotonView view, string rpcName, NetPlayer targetPlayer, params object[] parameters)
	{
		return SendRpcCore(view, rpcName, RpcTarget.AllBuffered, targetPlayer.GetPlayerRef(), null, parameters);
	}

	public static bool SendRpc(this PhotonView view, string rpcName, RpcTarget target, params object[] parameters)
	{
		return SendRpcCore(view, rpcName, target, null, null, parameters);
	}

	private static bool SendRpcCore(
		PhotonView view,
		string rpcName,
		RpcTarget target,
		Player targetPlayer,
		int[] targetActorNumbers,
		object[] parameters)
	{
		if (!MatBridge.ShouldSendRPC(rpcName) || view == null || parameters == null || string.IsNullOrEmpty(rpcName))
		{
			return false;
		}

		Hashtable rpcData = BuildRpcPayload(view, rpcName, parameters);

		if (targetActorNumbers != null && targetActorNumbers.Length > 0)
		{
			SendToActors(view, rpcData, targetActorNumbers);
		}
		else if (targetPlayer != null)
		{
			SendToPlayer(rpcData, targetPlayer);
		}
		else
		{
			SendToRpcTarget(view, rpcData, target);
		}

		return false;
	}

	private static Hashtable BuildRpcPayload(PhotonView view, string rpcName, object[] parameters)
	{
		byte viewIdKey = GetPhotonRpcKey("keyByteZero");
		byte prefixKey = GetPhotonRpcKey("keyByteOne");
		byte timestampKey = GetPhotonRpcKey("keyByteTwo");
		byte parametersKey = GetPhotonRpcKey("keyByteFour");
		byte methodNameKey = GetPhotonRpcKey("keyByteFive");

		Hashtable rpcData = new Hashtable
		{
			[viewIdKey] = view.ViewID,
			[methodNameKey] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(rpcName),
			[timestampKey] = GetRpcTimestamp(rpcName)
		};

		if (view.Prefix > 0)
		{
			rpcData[prefixKey] = (byte)view.Prefix;
		}

		if (parameters.Length > 0)
		{
			rpcData[parametersKey] = parameters;
		}

		return rpcData;
	}

	private static byte GetPhotonRpcKey(string fieldName)
	{
		return Traverse.Create(typeof(PhotonNetwork)).Field<byte>(fieldName).Value;
	}

	private static int GetRpcTimestamp(string rpcName)
	{
		if (rpcName == "RPC_PlaySplashEffect" || rpcName == "AddPartyMembers")
		{
			return PhotonNetwork.ServerTimestamp;
		}

		if (rpcName == "JoinWithItemsRPC" || rpcName == "OwnershipRequested")
		{
			return 0;
		}

		return DefaultTimestamp;
	}

	private static void SendToRpcTarget(PhotonView view, Hashtable rpcData, RpcTarget target)
	{
		RaiseEventOptions options;
		bool executeLocally = false;

		switch (target)
		{
			case RpcTarget.All:
				options = CreateRaiseEventOptions(view, ReceiverGroup.Others);
				break;
			case RpcTarget.Others:
				options = CreateRaiseEventOptions(view, ReceiverGroup.MasterClient, EventCaching.AddToRoomCache);
				break;
			case RpcTarget.MasterClient:
				options = CreateRaiseEventOptions(view, ReceiverGroup.All, EventCaching.AddToRoomCache);
				executeLocally = !MatBridge.GetCRPC();
				break;
			case RpcTarget.AllBuffered:
				options = CreateRaiseEventOptions(view, ReceiverGroup.Others, EventCaching.AddToRoomCache);
				break;
			case RpcTarget.OthersBuffered:
				options = CreateRaiseEventOptions(view, ReceiverGroup.All, EventCaching.AddToRoomCache);
				executeLocally = PhotonNetwork.OfflineMode && !MatBridge.GetCRPC();
				break;
			case RpcTarget.AllViaServer:
				options = CreateRaiseEventOptions(view, ReceiverGroup.All);
				executeLocally = PhotonNetwork.OfflineMode && !MatBridge.GetCRPC();
				break;
			case RpcTarget.AllBufferedViaServer:
				return;
			default:
				options = CreateRaiseEventOptions(view, ReceiverGroup.All);
				break;
		}

		RaiseRpcEvent(rpcData, options);
		if (executeLocally)
		{
			PhotonNetwork.ExecuteRpc(rpcData, PhotonNetwork.LocalPlayer);
		}
	}

	private static RaiseEventOptions CreateRaiseEventOptions(
		PhotonView view,
		ReceiverGroup receivers,
		EventCaching caching = EventCaching.DoNotCache)
	{
		return new RaiseEventOptions
		{
			Receivers = receivers,
			InterestGroup = view.Group,
			CachingOption = caching
		};
	}

	private static void SendToActors(PhotonView view, Hashtable rpcData, int[] targetActorNumbers)
	{
		RaiseRpcEvent(rpcData, new RaiseEventOptions
		{
			TargetActors = targetActorNumbers,
			InterestGroup = view.Group
		});

		if (MatBridge.GetCRPC())
		{
			return;
		}

		foreach (int actorNumber in targetActorNumbers)
		{
			if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				PhotonNetwork.ExecuteRpc(rpcData, PhotonNetwork.LocalPlayer);
				break;
			}
		}
	}

	private static void SendToPlayer(Hashtable rpcData, Player targetPlayer)
	{
		if (PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber == targetPlayer.ActorNumber)
		{
			if (!MatBridge.GetCRPC())
			{
				PhotonNetwork.ExecuteRpc(rpcData, PhotonNetwork.LocalPlayer);
			}
			return;
		}

		RaiseRpcEvent(rpcData, new RaiseEventOptions
		{
			TargetActors = new[] { targetPlayer.ActorNumber }
		});
	}

	private static void RaiseRpcEvent(Hashtable rpcData, RaiseEventOptions options)
	{
		SendOptions sendOptions = new SendOptions
		{
			Reliability = true,
			DeliveryMode = DeliveryMode.ReliableUnsequenced,
			Encrypt = false
		};

		PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(RpcEventCode, rpcData, options, sendOptions);
	}
}
