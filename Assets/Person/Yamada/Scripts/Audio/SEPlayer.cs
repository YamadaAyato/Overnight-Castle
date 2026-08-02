using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public void PlaySE()
    {
        AudioManager.Instance.PlaySE(_seName);
    }

    [SerializeField] private string _seName;
}
