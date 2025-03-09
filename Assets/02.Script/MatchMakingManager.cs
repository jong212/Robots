using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System;
using Fusion.Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runner;
    public Text playerCountText; // UI에 표시할 텍스트
    private const int MAX_PLAYERS = 3; // 최대 8명
    private const string SESSION_NAME = "MyGameRoom"; // 방 이름 고정
    public GameObject playerPrefab;
    public GameObject Starter;
    [Networked] TickTimer timer { get; set; } // ✅ 네트워크 동기화 타이머

    private async void Start()
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        await StartMatchmaking();
    }

    // 1️⃣ 빠른 매치메이킹 실행 (빈 방 찾거나 없으면 새 방 생성)
    public async Task StartMatchmaking()
    {
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared, // Shared Mode 사용 (서버 없이 동기화 가능)
            PlayerCount = MAX_PLAYERS, // 최대 8명 설정
            MatchmakingMode = MatchmakingMode.FillRoom, // 빈 방 먼저 찾고 없으면 생성
            SessionName = SESSION_NAME, // ✅ 모든 클라이언트가 같은 방에 들어가도록 설정
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>() // 이거 없이도 씬 바뀌긴 하는데 멀티씬 로딩,
        });

        if (result.Ok)
        {
            Debug.Log("✅ Matchmaking started: 참가 성공!");
        }
        else
        {
            Debug.LogError($"❌ Failed to Start: {result.ShutdownReason}");
        }
    }

    // 2️⃣ 플레이어가 입장할 때 UI 업데이트 (1/8 → 8/8)
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        int currentPlayers = runner.SessionInfo.PlayerCount; // ✅ 현재 세션의 플레이어 수 가져오기
        Debug.Log($"🎮 Player Joined! 현재 인원: {currentPlayers}/{MAX_PLAYERS}");
        playerCountText.text = $"{currentPlayers}/{MAX_PLAYERS}"; // UI 업데이트

        // 8명이 모이면 자동으로 게임 시작
        if (currentPlayers >= MAX_PLAYERS)
        {
            StartGame();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerCountText.text == null) return;
        int currentPlayers = runner.SessionInfo.PlayerCount; // ✅ 정확한 인원 수 반영
        Debug.Log($"🚪 Player Left! 현재 인원: {currentPlayers}/{MAX_PLAYERS}");
        playerCountText.text = $"{currentPlayers}/{MAX_PLAYERS}";
    }




    // 4️⃣ 8명 모이면 게임 시작
    private void StartGame()
    {
        if (runner.IsSceneAuthority) // ✅ 마스터 클라이언트만 씬 변경 실행
        {
            Debug.Log($"🚀 {MAX_PLAYERS}명 도달! 게임 시작!");
            runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single); // ✅ 모든 플레이어에게 게임 씬으로 이동하도록 설정
        }
    }

    // INetworkRunnerCallbacks 필수 구현 (빈 메서드)
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("🟢 씬 로드 완료! 캐릭터 스폰 시작!");

        // 모든 클라이언트가 자신의 캐릭터를 스폰하도록 설정
        SpawnPlayer(runner);
    }
    private void SpawnPlayer(NetworkRunner runner)
    {
        Debug.Log("🟢 캐릭터 스폰 중...");
        if (playerPrefab == null)
        {
            Debug.LogError("❌ 플레이어 프리팹이 없습니다!");
            return;
        }

        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-3f, 3f), 1f, UnityEngine.Random.Range(-3f, 3f)); // 랜덤 위치

        // ✅ 네트워크 오브젝트 스폰
        NetworkObject player = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, runner.LocalPlayer);
        if (runner.IsSharedModeMasterClient)
        {
/*            runner.Spawn(Starter, spawnPos, Quaternion.identity, runner.LocalPlayer);
*/        }


    }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }
}