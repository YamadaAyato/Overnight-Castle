using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ChengUI : MonoBehaviour
{
    [Header("変えたときに移動するUI")]
    [SerializeField] private GameObject[] _upObjs;
    [SerializeField] private int _upImageIndex = 5;
    [SerializeField] private GameObject[] _downObjs;
    [SerializeField] private int _downImageIndex = 5;

    [Header("横からスライドさせるイメージ")]
    [SerializeField] private GameObject _previewObj;
    [SerializeField] private int _imageIndex = 1500;

    [SerializeField] private GameObject _buttonObj;
    [SerializeField] private int _buttonIndex = -450;

    [SerializeField] private int _takesTime = 1;

    public void UIChange()
    {
        foreach (var imageObject in _upObjs)
        {
            imageObject.transform.DOMoveY(_upImageIndex, _takesTime);
        }

        foreach (var imageObject in _downObjs)
        {
            imageObject.transform.DOMoveY(_downImageIndex, _takesTime);
        }

        _previewObj.transform.DOMoveX(_imageIndex, _takesTime);
        _buttonObj.transform.DOMoveX(_buttonIndex, _takesTime);
    }
}
