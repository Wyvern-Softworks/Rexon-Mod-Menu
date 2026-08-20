// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.BlueAllMaster
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Blue All [MASTER]", "Overpowered", "Forces all players to blue team. Requires master.", false, 41, ModType.Toggle, false)]
internal class BlueAllMaster : MonoBehaviour
{
	private const string SetGameStateRpc = "RequestSetGameStateRPC";
	private const float UpdateInterval = 1f;
	private const int BlueTeamId = 0;

	private float _lastUpdateTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		SetAllPlayersToTeam(BlueTeamId);
	}

	private static void SetAllPlayersToTeam(int teamId)
	{
		MonkeBallGame game = MonkeBallGame.Instance;
		if (game == null)
		{
			return;
		}

		NetPlayer[] players = NetworkSystem.Instance.AllNetPlayers;
		int[] actorNumbers = new int[players.Length];
		int[] teamAssignments = new int[players.Length];
		for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
		{
			actorNumbers[playerIndex] = players[playerIndex].ActorNumber;
			teamAssignments[playerIndex] = teamId;
		}

		int[] teamScores = new int[game.team.Count];
		int ballCount = game.startingBalls.Count;
		long[] packedBallTransforms = new long[ballCount];
		long[] packedBallVelocities = new long[ballCount];
		for (int ballIndex = 0; ballIndex < ballCount; ballIndex++)
		{
			MonkeBall ball = game.startingBalls[ballIndex];
			packedBallTransforms[ballIndex] = BitPackUtils.PackHandPosRotForNetwork(
				ball.transform.position,
				ball.transform.rotation);
			packedBallVelocities[ballIndex] = BitPackUtils.PackWorldPosForNetwork(ball.gameBall.GetVelocity());
		}

		game.photonView.SendRpc(
			SetGameStateRpc,
			RpcTarget.All,
			2,
			PhotonNetwork.Time + game.gameDuration,
			actorNumbers,
			teamAssignments,
			teamScores,
			packedBallTransforms,
			packedBallVelocities);
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
