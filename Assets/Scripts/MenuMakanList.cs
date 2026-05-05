using TMPro;
using UnityEngine;


public class MenuMakanList : MonoBehaviour {
    public static MenuMakanList Instance;
    public GameObject[] foodItems;
    private TextMeshProUGUI foodNameText;
    private TextMeshProUGUI karboText;
    private TextMeshProUGUI GIText;
    private TextMeshProUGUI GLText;

    private string foodName = "";
    private string karbo = "";
    private string GI = "";
    private string GL = "";
    private int listActive = 0;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    /* fungsi untuk menambahkan 1 list makanan dengan penjelasan detail seperti nama makanan, karbo, GI dan GL
    */
    public void AddFoodItem(int index) {
        if (index < 0 || index >= foodItems.Length) {
            return;
        }

        RemoveFoodList removeBtn = foodItems[index].GetComponentInChildren<RemoveFoodList>();
        if (removeBtn != null) {
            removeBtn.SetIndex(index);
        }

        foodNameText = GetChildByName(foodItems[index], "nama makanan").GetComponent<TextMeshProUGUI>();
        karboText = GetChildByName(foodItems[index], "karbo-val").GetComponent<TextMeshProUGUI>();
        GIText = GetChildByName(foodItems[index], "val GI").GetComponent<TextMeshProUGUI>();
        GLText = GetChildByName(foodItems[index], "val GL").GetComponent<TextMeshProUGUI>();

        foodItems[index].SetActive(true);
        foodNameText.text = foodName;
        karboText.text = karbo;
        GIText.text = GI;
        GLText.text = GL;
        listActive++;
    }

    /* fungsi untuk menggeser daftar makanan
        misal yang dihapus index ke 1, maka
        index ke 1 akan diisi dengan index ke dua
        index ke 2 akan disii dengan index ketiga
        */
    public void ReplaceFoodItem(int index) {
        for (int i = index; i < listActive - 1; i++) {
            CopyFoodItem(foodItems[i + 1], foodItems[i]);

            RemoveFoodList removeBtn = foodItems[i].GetComponentInChildren<RemoveFoodList>();
            if (removeBtn != null) {
                removeBtn.SetIndex(i);
            }
        }

        // Matikan item terakhir
        foodItems[listActive - 1].SetActive(false);
        listActive--;
    }


    // fungsi untuk menghapus satu daftar makanan, lalu menggeser makanan dibawahnya ke atas dengan fungsi ReplaceFoodItem
    public void RemoveFoodItems(int index) {
        if (index < 0 || index >= listActive)
            return;

        ReplaceFoodItem(index);
    }


    // fungsi untuk mendapatkan data makanan
    public void setFoodInfo(string NameFood, string valKarbo, string valGI, string valGL) {
        foodName = NameFood;
        karbo = valKarbo;
        GI = valGI;
        GL = valGL;
    }

    // ini untuk mendapatkan child gameObject dari foodItems yang dipilih
    GameObject GetChildByName(GameObject parent, string childName) {
        foreach (Transform child in parent.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
        }
        return null; // Child tidak ditemukan
    }

    void CopyFoodItem(GameObject from, GameObject to) {
        SetTextFoodItem(
            to,
            GetChildByName(from, "nama makanan").GetComponent<TextMeshProUGUI>().text,
            int.Parse(GetChildByName(from, "karbo-val").GetComponent<TextMeshProUGUI>().text),
            int.Parse(GetChildByName(from, "val GI").GetComponent<TextMeshProUGUI>().text),
            int.Parse(GetChildByName(from, "val GL").GetComponent<TextMeshProUGUI>().text)
        );
        to.SetActive(true);
    }

    void SetTextFoodItem(GameObject go, string name, int karbo, int gi, int gl)
    {
        GetChildByName(go, "nama makanan").GetComponent<TextMeshProUGUI>().text = name;
        GetChildByName(go, "karbo-val").GetComponent<TextMeshProUGUI>().text = karbo.ToString();
        GetChildByName(go, "val GI").GetComponent<TextMeshProUGUI>().text = gi.ToString();
        GetChildByName(go, "val GL").GetComponent<TextMeshProUGUI>().text = gl.ToString();
    }

    public int GetCurrentListCount() {
        return listActive;
    }
}