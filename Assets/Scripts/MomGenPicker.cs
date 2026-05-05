using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MomGenPicker : MonoBehaviour
{
    public Image[] knobImages;               // 15 gambar knob (5 anak * 3 gen)
    public Color inactiveColor = Color.black; // Warna default
    public Color activeColor = Color.blue;    // Warna biru (gen pembawa risiko)

    public float spinDuration = 2.0f;         // Lama spin
    public float changeInterval = 0.1f;       // Jeda antar perubahan

    public void StartSpin()
    {
        StopAllCoroutines(); // Reset jika dipanggil ulang
        StartCoroutine(SpinAndPick());
    }

    IEnumerator SpinAndPick()
    {
        float timer = 0f;
        while (timer < spinDuration)
        {
            foreach (var knob in knobImages)
            {
                knob.color = (knob.color == inactiveColor) ? activeColor : inactiveColor;
            }
            yield return new WaitForSeconds(changeInterval);
            timer += changeInterval;
        }

        Debug.Log("Start Spinning Colors");

        int childIndex = -1;
        for (int i = 0; i < knobImages.Length; i++)
        {
            if (i % 3 == 0)
            {
                childIndex++;

                // Pilih salah satu dari 4 kombinasi untuk ibu:
                // 000, 001, 010, 100
                int[] validMasks = { 0b000, 0b001, 0b010, 0b100 };
                int mask = validMasks[Random.Range(0, validMasks.Length)];

                Color[] result = new Color[3];
                for (int local = 0; local < 3; local++)
                {
                    bool isActive = (mask & (1 << local)) != 0;
                    result[local] = isActive ? activeColor : inactiveColor;
                }

                // Terapkan ke knobImages
                for (int local = 0; local < 3; local++)
                {
                    int idx = i + local;
                    knobImages[idx].color = result[local];
                }

                // Simpan hasil ke GeneResultData
                if (GeneResultData.Instance == null)
                {
                    Debug.LogError("GeneResultData.Instance is NULL");
                }
                else
                {
                    GeneResultData.Instance.momChild[childIndex] = result;
                }
            }
        }

        Debug.Log("Spin Ibu Selesai...");
    }
}
