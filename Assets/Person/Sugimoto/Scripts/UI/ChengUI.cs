using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ChengUI : MonoBehaviour
{
    [Header("変えたときに移動するUI")]
    [SerializeField] private GameObject[] _upImage;
    [SerializeField] private int _upImageIndex = 5;
    [SerializeField] private GameObject[] _downImage;
    [SerializeField] private int _downImageIndex = 5;

    [Header("横からスライドさせるイメージ")]
    [SerializeField] private GameObject PreviewImage;
    [SerializeField] private int ImageIndex = 1500;

    [SerializeField] private GameObject ButtonImage;
    [SerializeField] private int ButtonIndex = -450;

    [SerializeField] private int _takesTime = 1;

    public void UICheng()
    {
        foreach (var imageObject in _upImage)
        {
            imageObject.transform.DOMoveY(_upImageIndex, _takesTime);
        }

        foreach (var imageObject in _downImage)
        {
            imageObject.transform.DOMoveY(_downImageIndex, _takesTime);
        }

        PreviewImage.transform.DOMoveX(ImageIndex, _takesTime);
        ButtonImage.transform.DOMoveX(ButtonIndex, _takesTime);
    }
}
