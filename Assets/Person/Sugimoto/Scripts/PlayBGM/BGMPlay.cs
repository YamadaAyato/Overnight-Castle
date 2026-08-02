using UnityEngine;

/// <summary>
/// Inspectorで指定した名前の音を再生する
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayBGM("TitleBGM");
    }

}