using UnityEngine;
using unityroom.Api;

/// <summary>
/// スコアをunityRoomに送るクラス
/// </summary>
public class ScoreSender : MonoBehaviour
{
    [SerializeField] private int _boardNumber;

    public void SendHighScore(int score) 
    {
        UnityroomApiClient.Instance.SendScore(
            _boardNumber,
            score,
            ScoreboardWriteMode.HighScoreDesc
            );
    }
}
