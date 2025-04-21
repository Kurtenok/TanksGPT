using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;

public class ChatGPTClient : MonoBehaviour
{
    private string apiKey = "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"; // Вставь API-ключ OpenAI
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    public void SendMessageToChatGPT(string message)
    {
        Debug.Log($"[ChatGPT] Запрос отправлен: {message}");
        StartCoroutine(SendRequest(message));
    }

    private IEnumerator SendRequest(string message)
    {
        yield return new WaitForSeconds(2f); 

        var requestData = new
        {
            model = "gpt-3.5-turbo", 
            messages = new object[] { new { role = "user", content = message } }
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        Debug.Log($"[ChatGPT] JSON-запрос: {jsonData}");

        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        Debug.Log("[ChatGPT] Запрос отправляется...");

        yield return request.SendWebRequest();

        Debug.Log($"[ChatGPT] Код ответа: {request.responseCode}");

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            Debug.Log($"[ChatGPT] Получен ответ JSON: {responseJson}");

            ChatGPTResponse response = JsonConvert.DeserializeObject<ChatGPTResponse>(responseJson);
            string reply = response.choices[0].message.content;
            Debug.Log($"[ChatGPT] Ответ от ChatGPT: {reply}");
        }
        else
        {
            Debug.LogError($"[ChatGPT] Ошибка запроса: {request.error} | Код: {request.responseCode}");
        }
    }

    [System.Serializable]
    private class ChatGPTResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }
}
