using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneSlider : MonoBehaviour
{
    [System.Serializable]
    public class SlidePanel
    {
        public RectTransform rect;
        public CanvasGroup canvasGroup;
    }

    public List<SlidePanel> childPanels;
    public float duration = 0.4f;

    private int currentIndex = 0;
    private bool isSliding = false;

    public void ShowNext()
    {
        if (isSliding || currentIndex >= childPanels.Count - 1) return;
        StartCoroutine(SlideFade(currentIndex, currentIndex + 1, -1));
    }

    public void ShowPrevious()
    {
        if (isSliding || currentIndex <= 0) return;
        StartCoroutine(SlideFade(currentIndex, currentIndex - 1, 1));
    }

    IEnumerator SlideFade(int fromIdx, int toIdx, int direction)
    {
        isSliding = true;

        var current = childPanels[fromIdx];
        var next = childPanels[toIdx];

        Vector2 fromPos = Vector2.zero;
        Vector2 toStartPos = new Vector2(direction * Screen.width, 0);

        next.rect.anchoredPosition = toStartPos;
        next.canvasGroup.alpha = 0;
        next.rect.gameObject.SetActive(true);

        float t = 0;
        while (t < duration)
        {
            float normalized = t / duration;

            // Geser posisi
            current.rect.anchoredPosition = Vector2.Lerp(Vector2.zero, -toStartPos, normalized);
            next.rect.anchoredPosition = Vector2.Lerp(toStartPos, Vector2.zero, normalized);

            // Fade
            current.canvasGroup.alpha = Mathf.Lerp(1, 0, normalized);
            next.canvasGroup.alpha = Mathf.Lerp(0, 1, normalized);

            t += Time.deltaTime;
            yield return null;
        }

        // Finalize
        current.rect.anchoredPosition = -toStartPos;
        current.canvasGroup.alpha = 0;
        current.rect.gameObject.SetActive(false);

        next.rect.anchoredPosition = Vector2.zero;
        next.canvasGroup.alpha = 1;

        currentIndex = toIdx;
        isSliding = false;
    }
}
