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
    [SerializeField] private GameObject PreviewObj;
    [SerializeField] private int ImageIndex = 1500;

    [SerializeField] private GameObject ButtonObj;
    [SerializeField] private int ButtonIndex = -450;

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

        PreviewObj.transform.DOMoveX(ImageIndex, _takesTime);
        ButtonObj.transform.DOMoveX(ButtonIndex, _takesTime);
    }
}
