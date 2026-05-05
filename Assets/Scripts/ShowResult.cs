using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowResult : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject labelPrefab;
    public Sprite circleSprite;
    public int yAxisDivisions = 5;

    void Start()
    {
        List<float> data = GlucoseDataStore.collectedData;

        Debug.Log("Jumlah data: " + (data != null ? data.Count : 0));

        ShowGraph(Smooth(data));
    }

    void ShowGraph(List<float> values)
    {
        float graphWidth = graphContainer.rect.width;
        float graphHeight = graphContainer.rect.height;
        float yMax = 350f;
        float xStep = values.Count > 1 ? graphWidth / (values.Count - 1) : 0;

        GameObject lastCircle = null;

        for (int i = 0; i < values.Count; i++)
        {
            float xPos = i * xStep;
            float yPos = (values[i] / yMax) * graphHeight;
            GameObject newCircle = CreateCircle(new Vector2(xPos, yPos));

            if (lastCircle != null)
            {
                CreateDotConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition, newCircle.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircle = newCircle;

            CreateLabelX(xPos, i.ToString() + "s");
        }

        for (int i = 0; i <= yAxisDivisions; i++)
        {
            float normalizedValue = i / (float)yAxisDivisions;
            float yPos = normalizedValue * graphHeight;
            CreateLabelY(yPos, (normalizedValue * yMax).ToString("0"));
        }
    }

    GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(18, 18);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        return gameObject;
    }

    void CreateDotConnection(Vector2 pointA, Vector2 pointB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(0f, 0.6f, 1f, 1f); // biru muda terang
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (pointB - pointA).normalized;
        float distance = Vector2.Distance(pointA, pointB);
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(distance, 3f);
        rt.anchoredPosition = pointA + dir * distance * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    void CreateLabelX(float xPos, string text)
    {
        GameObject label = Instantiate(labelPrefab, graphContainer);
        RectTransform rt = label.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(xPos, -20f);
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        label.GetComponent<TMP_Text>().text = text;
    }

    void CreateLabelY(float yPos, string text)
    {
        GameObject label = Instantiate(labelPrefab, graphContainer);
        RectTransform rt = label.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-30f, yPos);
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        label.GetComponent<TMP_Text>().text = text;
    }

    List<float> Smooth(List<float> raw, int windowSize = 3) {
        List<float> smooth = new List<float>();
        for (int i = 0; i < raw.Count; i++)
        {
            float sum = 0f;
            int count = 0;
            for (int j = i - windowSize; j <= i + windowSize; j++)
            {
                if (j >= 0 && j < raw.Count)
                {
                    sum += raw[j];
                    count++;
                }
            }
            smooth.Add(sum / count);
        }
        return smooth;
    }
}

