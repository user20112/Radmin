using UnityEngine;
using TMPro;
using DG.Tweening;

public class InterfaceManager : MonoBehaviour
{

    public RadminManager radminManager;
    public TextMeshProUGUI radminCountText;

    void Start()
    {
        radminManager.radminFollow.AddListener((x) => UpdateRadminNumber(x));
    }

    void UpdateRadminNumber(int num)
    {
        radminCountText.transform.DOComplete();
        radminCountText.transform.DOPunchScale(Vector3.one/3, .3f, 10, 1);
        radminCountText.text = num.ToString();
    }
}
