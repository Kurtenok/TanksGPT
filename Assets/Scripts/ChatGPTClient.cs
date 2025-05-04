using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ChatGPTClient : MonoBehaviour
{
    private string apiKey = "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";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    public async Task<string> SendMessageToChatGPT(string message)
    {
        Debug.Log($"[ChatGPT] Отправка запроса: {message}");

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

        Debug.Log("[ChatGPT] Отправка запроса...");

        // 🔁 Асинхронно ждем завершения запроса через обертку
        await AwaitUnityWebRequest(request);

        Debug.Log($"[ChatGPT] Код ответа: {request.responseCode}");

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            ChatGPTResponse response = JsonConvert.DeserializeObject<ChatGPTResponse>(responseJson);
            string reply = response.choices[0].message.content;
            Debug.Log($"[ChatGPT] Ответ от ChatGPT: {reply}");
            return reply;
        }
        else
        {
            Debug.LogError($"[ChatGPT] Ошибка: {request.error} | Код: {request.responseCode}");
            return null;
        }
    }

    private Task AwaitUnityWebRequest(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        operation.completed += _ => tcs.SetResult(true);

        return tcs.Task;
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
