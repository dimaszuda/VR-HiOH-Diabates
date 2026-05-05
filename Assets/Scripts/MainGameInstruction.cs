using UnityEngine;
using UnityEngine.UI;

public class MainGameInstruction : MonoBehaviour
{
    public GameObject activity;
    public GameObject foodCanvas;
    public GameObject menuMakan;
    public GameObject canvasMenuMakan;
    public GameObject[] guide;
    public GameObject[] listActivity;

    private void SetGuideActive(int index, bool isActive)
    {
        if (guide != null && index >= 0 && index < guide.Length && guide[index] != null)
        {
            guide[index].SetActive(isActive);
        }
    }

    public void ShowFirstGuide()
    {
        SetGuideActive(0, false);
        SetGuideActive(1, true);
    }

    public void ShowSecondGuide()
    {
        SetGuideActive(1, false);
        foodCanvas.SetActive(false);
        activity.SetActive(true);
    }

    public void ShowThirdGuide()
    {
        SetGuideActive(1, false);
        foodCanvas.SetActive(true);
        activity.SetActive(false);
        SetGuideActive(2, true);
    }

    public void ShowFourthGuide()
    {
        SetGuideActive(2, false);
        SetGuideActive(3, true);
    }

    public void EndGuide()
    {
        foodCanvas?.SetActive(false);
        foreach (var g in guide)
        {
            g?.SetActive(false);
        }   
        foreach (var a in listActivity) {
            a?.SetActive(true);
        }
        menuMakan.SetActive(true);
        canvasMenuMakan.SetActive(true);
    }
}
