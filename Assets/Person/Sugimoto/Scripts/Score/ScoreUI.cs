using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _height;
    [SerializeField] TextMeshProUGUI _heightScore;
    [SerializeField] TextMeshProUGUI _completionScore;
    [SerializeField] TextMeshProUGUI _totalScore;

    public void SetUI(CastleScoreResult castleScoreResult) 
    {
        _height.text = castleScoreResult.Height.ToString();
        _heightScore.text = castleScoreResult.HeightScore.ToString();
        _completionScore.text = castleScoreResult.CompletionScore.ToString();
        _totalScore.text = castleScoreResult.TotalScore.ToString();
    }
}
