using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     爆弾ピースの挙動を制御するクラス
/// </summary>
[RequireComponent(typeof(FallingPiece))]
public class BombPiece : MonoBehaviour
{
    [Header("爆弾の設定")]
    [SerializeField, Range(0f, 5f), Tooltip("接触してから爆発するまでの時間")]
    private float _fadeDuration = 1.5f;

    [SerializeField, Range(0f, 10f), Tooltip("爆発の半径")]
    private float _explosionRadius = 3f;

    [SerializeField, Min(0f), Tooltip("爆発の力")]
    private float _explosionForce = 5f;

    [SerializeField, Tooltip("爆発の影響を受けるレイヤー")]
    private LayerMask _explosionLayer;

    [Header("演出設定")]
    [SerializeField, Tooltip("爆発エフェクト")]
    private ParticleSystem _explosionEffect;

    [SerializeField, Tooltip("SEの名前")]
    private string _explosionSEName;

    private readonly HashSet<Rigidbody2D> _affectedRigidbodies = new HashSet<Rigidbody2D>();

    private FallingPiece _fallingPiece;
    private Rigidbody2D _rigidbody2D;

    private bool _hasStartedFuse;
    private bool _hasExploded;

    private void Awake()
    {
        _fallingPiece = GetComponent<FallingPiece>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_fallingPiece.HasDropped ||
            _hasStartedFuse ||
            _hasExploded)
        {
            return;
        }

        _hasStartedFuse = true;
        _ = ExplodeAfterDelay();
    }

    /// <summary>
    ///     爆発までの遅延処理を行う
    /// </summary>
    /// <returns></returns>
    private async Awaitable ExplodeAfterDelay()
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(_fadeDuration, destroyCancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (_rigidbody2D.bodyType == RigidbodyType2D.Static)
        {
            return;
        }

        Explode();
    }

    /// <summary>
    ///     爆発処理を実行する
    /// </summary>
    private void Explode()
    {
        if (_hasExploded)
        {
            return;
        }

        _hasExploded = true;
        _affectedRigidbodies.Clear();

        Vector2 explosionPosition = transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            explosionPosition,
            _explosionRadius,
            _explosionLayer);

        Debug.Log($"爆発位置: {explosionPosition}, 爆発半径: {_explosionRadius}, 影響を受けるコライダー数: {colliders.Length}");

        foreach (var collider in colliders)
        {
            if (collider == null)
            {
                continue;
            }

            Rigidbody2D targetRigidbody = collider.attachedRigidbody;

            if (!CanApplyExplosionForce(targetRigidbody))
            {
                continue;
            }

            if (!_affectedRigidbodies.Add(targetRigidbody))
            {
                continue;
            }

            ApplyExplosionForce(targetRigidbody, explosionPosition);
        }

        PlayExplosionEffect(explosionPosition);

        if (AudioManager.Instance != null &&
            !string.IsNullOrEmpty(_explosionSEName))
        {
            AudioManager.Instance.PlaySE(
                _explosionSEName);
        }

        Destroy(gameObject);
    }

    /// <summary>
    ///     爆発力を適用できるかどうかを判定する
    /// </summary>
    /// <param name="targetRigidbody">判定対象のRigidbody2D</param>
    /// <returns>爆発力を適用できる場合はtrue、それ以外はfalse</returns>
    private bool CanApplyExplosionForce(Rigidbody2D targetRigidbody)
    {
        if (targetRigidbody == null ||
            targetRigidbody == _rigidbody2D)
        {
            return false;
        }

        return targetRigidbody.bodyType == RigidbodyType2D.Dynamic;
    }

    /// <summary>
    ///     爆発力を適用する
    /// </summary>
    /// <param name="targetRigidbody">爆発力を適用するRigidbody2D</param>
    /// <param name="explosionPosition">爆発の中心位置</param>
    private void ApplyExplosionForce(Rigidbody2D targetRigidbody, Vector2 explosionPosition)
    {
        Vector2 direction = targetRigidbody.worldCenterOfMass - explosionPosition;

        float distance = direction.magnitude;

        if (distance <= Mathf.Epsilon)
        {
            direction = Vector2.up;
            distance = 0f;
        }

        float distanceRate = 1f - Mathf.Clamp01(distance / _explosionRadius);
        float force = _explosionForce * distanceRate;

        targetRigidbody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    /// <summary>
    ///     爆発エフェクトを再生する
    /// </summary>
    /// <param name="explosionPos">爆発の位置</param>
    private void PlayExplosionEffect(Vector2 explosionPos)
    {
        if (_explosionEffect == null)
        {
            return;
        }

        ParticleSystem effect = Instantiate(_explosionEffect, explosionPos, Quaternion.identity);
        ParticleSystem.MainModule mainModule = effect.main;

        float destroyDelay = mainModule.duration + mainModule.startLifetime.constantMax;
        Destroy(effect.gameObject, destroyDelay);
    }
}