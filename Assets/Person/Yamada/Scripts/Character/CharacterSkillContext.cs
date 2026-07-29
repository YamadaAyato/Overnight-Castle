using System;
using System.Threading;
using UnityEngine;

/// <summary>
///     キャラクターのスキル効果を実行するためのコンテキスト情報を提供するクラス
/// </summary>
public class CharacterSkillContext
{
    public CharacterSkillContext(
        PieceSpawnModifiers modifiers,
        Timer timer,
        CancellationToken cancellationToken)
    {
        _modifiers = modifiers ?? throw new ArgumentNullException(nameof(modifiers));
        _timer = timer ?? throw new ArgumentNullException(nameof(timer));
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    ///     時間を追加する
    /// </summary>
    /// <param name="time">追加する時間（秒）</param>
    public void AddTime(float time)
    {
        _timer.AddTime(time);
    }

    /// <summary>
    ///     指定された回数分、指定された種類のピースの出現確率に補正を適用する
    /// </summary>
    /// <param name="type">ピースの種類</param>
    /// <param name="multiplier">重みの倍率</param>
    /// <param name="drawCount">補正を適用するピースの数</param>
    public void AddDrawCountWeightMultiplier(
        PieceType type,
        float multiplier,
        int drawCount)
    {
        _modifiers.AddDrawCountWeightMultiplier(type, multiplier, drawCount);
    }

    /// <summary>
    ///     指定された時間が経過するまで、指定された種類のピースの出現確率に補正を適用する
    /// </summary>
    /// <param name="type">ピースの種類</param>
    /// <param name="multiplier">重みの倍率</param>
    /// <param name="duration">補正を適用する時間（秒）</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void AddTimedWeightMultiplier(
        PieceType type,
        float multiplier,
        float duration)
    {
        if (duration <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be greater than zero.");
        }

        _modifiers.AddWeightMultiplier(type, multiplier);
        int modifierId = _modifiers.AddWeightMultiplier(type, multiplier);

        // 非同期で指定された時間が経過した後に重み補正を削除する
        _ = RemoveTimedWeightMultiplierAsync(modifierId, duration);
    }

    private readonly PieceSpawnModifiers _modifiers;
    private readonly Timer _timer;
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    ///     指定された時間が経過した後に、指定されたIDの重み補正を削除する非同期メソッド
    /// </summary>
    /// <param name="modifierId">削除する重み補正のID</param>
    /// <param name="duration">待機する時間（秒）</param>
    /// <returns></returns>
    private async Awaitable RemoveTimedWeightMultiplierAsync(
        int modifierId, float duration)
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(duration, _cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        _modifiers.RemoveWeightMultiplier(modifierId);
    }
}
