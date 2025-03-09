using Fusion;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [Networked] private TickTimer gameTimer { get; set; } // 타이머 기능
    private void Update()
    {
     /*   float? remainingTime = gameTimer.RemainingTime(Runner);
        if (remainingTime.HasValue)
        {
            if (Runner.Tick % Runner.TickRate == 0) // ✅ 1초마다 실행
            {
                Debug.Log($"⏳ [Client] 남은 시간: {remainingTime.Value:F2}초 (클라이언트 ID: {Runner.LocalPlayer.PlayerId})");
            }
        }
        else
        {
            Debug.Log($"⚠ [Client] 타이머가 아직 설정되지 않음! (클라이언트 ID: {Runner.LocalPlayer.PlayerId})");
        }*/
    }

    public override void Render()
    {
        
        float? remainingTime = gameTimer.RemainingTime(Runner);
        int secondsRemaining = Mathf.CeilToInt(remainingTime.Value);
        Debug.Log($"남은 시간: {secondsRemaining}초");
    }
    public override void FixedUpdateNetwork()
    {
        if (Runner.IsSharedModeMasterClient) 
        {
            if (gameTimer.ExpiredOrNotRunning(Runner))
            {
                gameTimer = TickTimer.CreateFromSeconds(Runner, 5); // ✅ 5초 타이머 설정
                Debug.Log("🕒 [Master] 새로운 5초 타이머 시작!");
            }
        }
    }
}
