using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DadGenPicker : MonoBehaviour
{
    public Image[] knobImages;

    public Color color1 = Color.black;
    public Color color2 = Color.blue;
    public Color color3 = Color.green;
    public Color color4 = Color.red;

    public float spinDuration = 2.0f;
    public float changeInterval = 0.1f;

    public void StartSpin()
    {
        StopAllCoroutines();
        StartCoroutine(SpinAndPick());
    }

    IEnumerator SpinAndPick()
    {
        Debug.Log("Start Spinning Colors Dad");
        float timer = 0f;
        Color[] allColors = new Color[] { color1, color2, color3, color4 };
        Color[] pickableColors = new Color[] { color2, color3, color4 };

        // efek berkedip semua warna
        while (timer < spinDuration)
        {
            foreach (Image knob in knobImages)
            {
                knob.color = allColors[Random.Range(0, allColors.Length)];
            }

            yield return new WaitForSeconds(changeInterval);
            timer += changeInterval;
        }

        Color[] result = new Color[3];
        int childIndex = -1;
        Color[] groupChosen = new Color[3];

        // Setelah spin selesai, tetapkan hasil akhir
        for (int i = 0; i < knobImages.Length; i++)
        {
            // Reset setiap 3 knob (1 anak)
            if (i == 0 || i == 3 || i == 6 || i == 9 || i == 12)
            {
                result = new Color[3];
                childIndex++;

                // Pilih salah satu dari 8 kombinasi (mask 0..7)
                int mask = Random.Range(0, 8);

                // Ambil warna acak unik
                List<Color> shuffledColors = new List<Color>(pickableColors);
                shuffledColors = Shuffle(shuffledColors);

                int colorIdx = 0;
                for (int local = 0; local < 3; local++)
                {
                    bool isWarna = (mask & (1 << local)) != 0;
                    if (isWarna)
                    {
                        groupChosen[local] = shuffledColors[colorIdx];
                        colorIdx++;
                    }
                    else
                    {
                        groupChosen[local] = color1; // hitam
                    }
                }
            }

            int localIndex = i % 3; // posisi dalam grup (0..2)

            knobImages[i].color = groupChosen[localIndex];
            result[localIndex] = groupChosen[localIndex];

            if (localIndex == 2)
            {
                GeneResultData.Instance.dadChild[childIndex] = result;
            }
        }
    }

    List<T> Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
        return list;
    }
}
