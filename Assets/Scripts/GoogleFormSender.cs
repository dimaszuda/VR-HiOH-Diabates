using System; 
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GoogleFormSender : MonoBehaviour {
    [System.Serializable]
    public class OptionGene {
        public string targetSheet = "pilihan_gen";
        public string class_name;
        public string team;
        public string full_name;
        public string number;
        public string anak_ke;
        public string gen_ayah_1;
        public string gen_ayah_2;
        public string gen_ayah_3;
        public string gen_ibu_1;
        public string gen_ibu_2;
        public string gen_ibu_3;
    }

    [System.Serializable]
    public class ChoicedGene {
        public string targetSheet = "gen_yang_dipilih";
        public string class_name;
        public string team;
        public string full_name;
        public string number;
        public string anak_ke;
        public string gen_ayah_1;
        public string gen_ayah_2;
        public string gen_ayah_3;
        public string gen_ibu_1;
        public string gen_ibu_2;
        public string gen_ibu_3;
        public string risk;
    }

    [System.Serializable]
    public class ActPattern {
        public string targetSheet = "pola_aktivitas";
        public string class_name;
        public string team;
        public string full_name;
        public string number;
        public string virtual_hour;
        public string activity_type;
        public string activity_name;
        public float carbohydrate;
        public float glycemic_index;
        public float glycemic_load;
        public float glucose_change;
    }

    [System.Serializable]
    public class KantinSehat {
        public string targetSheet = "kantin_sehat";
        public string class_name;
        public string team;
        public string full_name;
        public string number;
        public string food_name;
        public float carbohydrate;
        public float glycemic_index;
        public float glycemic_load;
    }

    [System.Serializable]
    public class KantinResult {
        public string targetSheet = "hasil_kantin_sehat";
        public string class_name;
        public string team;
        public string full_name;
        public string number;
        public float total_carbohydrate;
        public float average_gi;
        public float total_gl;
        public string summary;
    }

    [System.Serializable]
    public class PatternResult {
        public string targetSheet = "hasil_pola";
        public string class_name;
        public string team;
        public string full_name;
        public string number;
        public string summary;
    }

    [System.Serializable]
    public class SheetUploadPayload {
        public string scene;
        public List<List<object>> values;
    }

    private string endpointUrl = "https://dimaszudafa.pythonanywhere.com/upload";

    public static GoogleFormSender Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    SheetUploadPayload ConvertToPayload(OptionGene data) {
        string date_collected = System.DateTime.Now.ToString("yyyy-MM-dd");
        string hour_collected = System.DateTime.Now.ToString("HH:mm");
        List<object> row = new List<object> {
            date_collected,
            hour_collected,
            data.class_name,
            data.team,
            data.full_name,
            data.number,
            data.anak_ke,
            data.gen_ayah_1,
            data.gen_ayah_2,
            data.gen_ayah_3,
            data.gen_ibu_1,
            data.gen_ibu_2,
            data.gen_ibu_3
        };

        return new SheetUploadPayload {
            scene = "pilihan_gen", // atau bisa di-hardcode kalau mau
            values = new List<List<object>> { row }
        };
    }

    SheetUploadPayload ConvertToPayload(ChoicedGene data) {
        string date_collected = System.DateTime.Now.ToString("yyyy-MM-dd");
        string hour_collected = System.DateTime.Now.ToString("HH:mm");
        List<object> row = new List<object> {
            date_collected,
            hour_collected,
            data.class_name,
            data.team,
            data.full_name,
            data.number,
            data.anak_ke,
            data.gen_ayah_1,
            data.gen_ayah_2,
            data.gen_ayah_3,
            data.gen_ibu_1,
            data.gen_ibu_2,
            data.gen_ibu_3,
            data.risk
        };

        return new SheetUploadPayload {
            scene = "gen_yang_dipilih", // atau bisa di-hardcode kalau mau
            values = new List<List<object>> { row }
        };
    }

    SheetUploadPayload ConvertToPayload(ActPattern data) {
        string date_collected = System.DateTime.Now.ToString("yyyy-MM-dd");
        string hour_collected = System.DateTime.Now.ToString("HH:mm");
        List<object> row = new List<object> {
            date_collected,
            hour_collected,
            data.class_name,
            data.team,
            data.full_name,
            data.number,
            data.virtual_hour,
            data.activity_type,
            data.activity_name,
            data.carbohydrate,
            data.glycemic_index,
            data.glycemic_load,
            data.glucose_change
        };

        return new SheetUploadPayload {
            scene = "pola_aktivitas", // atau bisa di-hardcode kalau mau
            values = new List<List<object>> { row }
        };
    }

    SheetUploadPayload ConvertToPayload(KantinSehat data) {
        string date_collected = System.DateTime.Now.ToString("yyyy-MM-dd");
        string hour_collected = System.DateTime.Now.ToString("HH:mm");
        List<object> row = new List<object> {
            date_collected,
            hour_collected,
            data.class_name,
            data.team,
            data.full_name,
            data.number,
            data.food_name,
            data.carbohydrate,
            data.glycemic_index,
            data.glycemic_load,
        };

        return new SheetUploadPayload {
            scene = "kantin_sehat", // atau bisa di-hardcode kalau mau
            values = new List<List<object>> { row }
        };
    }

    SheetUploadPayload ConvertToPayload(KantinResult data) {
        string date_collected = System.DateTime.Now.ToString("yyyy-MM-dd");
        string hour_collected = System.DateTime.Now.ToString("HH:mm");
        List<object> row = new List<object> {
            date_collected,
            hour_collected,
            data.class_name,
            data.team,
            data.full_name,
            data.number,
            data.total_carbohydrate,
            data.average_gi,
            data.total_gl,
            data.summary,
        };
        
        return new SheetUploadPayload {
            scene = "hasil_kantin_sehat", // atau bisa di-hardcode kalau mau
            values = new List<List<object>> { row }
        };
    }

    SheetUploadPayload ConvertToPayload(PatternResult data) {
        string date_collected = System.DateTime.Now.ToString("yyyy-MM-dd");
        List<object> row = new List<object> {
            date_collected,
            data.class_name,
            data.team,
            data.full_name,
            data.number,
            data.summary,
        };
        
        return new SheetUploadPayload {
            scene = "hasil_pola", // atau bisa di-hardcode kalau mau
            values = new List<List<object>> { row }
        };
    }


    // Satuan (langsung kirim)
    public void SendOptionGene(OptionGene data) {
        SheetUploadPayload payload = ConvertToPayload(data);
        string json = JsonConvert.SerializeObject(payload);
        Debug.Log("Payload JSON: " + json);
        StartCoroutine(PostToSheet(json, endpointUrl));
    }

    public void SendChoicedGen(ChoicedGene data) {
        SheetUploadPayload payload = ConvertToPayload(data);
        string json = JsonConvert.SerializeObject(payload);
        Debug.Log("Payload JSON: " + json);
        StartCoroutine(PostToSheet(json, endpointUrl));
    }

    public void SendActPattern(ActPattern data) {
        SheetUploadPayload payload = ConvertToPayload(data);
        string json = JsonConvert.SerializeObject(payload);
        Debug.Log("Payload JSON: " + json);
        StartCoroutine(PostToSheet(json, endpointUrl));
    }

    public void SendKantinSummary(KantinResult data) {
        SheetUploadPayload payload = ConvertToPayload(data);
        string json = JsonConvert.SerializeObject(payload);
        Debug.Log("Payload JSON: " + json);
        StartCoroutine(PostToSheet(json, endpointUrl));
    }

    public void SendPolaSummary(PatternResult data) {
        SheetUploadPayload payload = ConvertToPayload(data);
        string json = JsonConvert.SerializeObject(payload);
        Debug.Log("Payload JSON: " + json);
        StartCoroutine(PostToSheet(json, endpointUrl));
    }

    public void SendKantinSehatSequentially(List<KantinSehat> dataList, Action onComplete = null) {
        StartCoroutine(SendKantinSehatList(dataList, onComplete));
    }

    IEnumerator SendKantinSehatList(List<KantinSehat> list, Action onComplete = null) {
        foreach (var data in list) {
            SheetUploadPayload payload = ConvertToPayload(data);
            string json = JsonConvert.SerializeObject(payload);
            Debug.Log("Payload JSON: " + json);
            yield return StartCoroutine(PostToSheet(json, endpointUrl));
            yield return new WaitForSeconds(1f); // jeda biar aman
        }
        onComplete?.Invoke(); // panggil callback setelah selesai
    }

    public void SendOptionGenesSequentially(List<OptionGene> dataList, Action onComplete = null) {
        StartCoroutine(SendOptionGeneList(dataList, onComplete));
    }

    IEnumerator SendOptionGeneList(List<OptionGene> list, Action onComplete = null) {
        foreach (var data in list) {
           SheetUploadPayload payload = ConvertToPayload(data);
            string json = JsonConvert.SerializeObject(payload);
            Debug.Log("Payload JSON: " + json);
            yield return StartCoroutine(PostToSheet(json, endpointUrl));
            yield return new WaitForSeconds(1f); // jeda biar aman
        }
        onComplete?.Invoke(); // panggil callback setelah selesai
    }

    public void SendActPatternsSequentially(List<ActPattern> dataList, Action onComplete = null) {
        StartCoroutine(SendActPatternList(dataList, onComplete));
    }

    IEnumerator SendActPatternList(List<ActPattern> list, Action onComplete = null) {
        foreach (var data in list) {
            SheetUploadPayload payload = ConvertToPayload(data);
            string json = JsonConvert.SerializeObject(payload);
            Debug.Log("Payload JSON: " + json);
            yield return StartCoroutine(PostToSheet(json, endpointUrl));
            yield return new WaitForSeconds(1f);
        }
        onComplete?.Invoke();
    }


    // Coroutine utama kirim ke Google Apps Script
    IEnumerator PostToSheet(string json, string url) {
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log("✅ Data berhasil dikirim");
        else
            Debug.LogError("❌ Gagal kirim data: " + www.error);
            Debug.LogError("Status Code: " + www.responseCode);
            Debug.LogError("Response Text: " + www.downloadHandler.text);
    }
}
