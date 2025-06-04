using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ChatGPTClient : MonoBehaviour
{
    [Header("OpenAI Settings")]
    //private string apiKey = "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";
    private string apiKey;
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    [Header("References")]
    [SerializeField] private GameObject enemyTank;
    [SerializeField] private GameObject playerTank;

    private AIController aiController;
    private Health enemyHealth;
    private Health playerHealth;
    private PlayerAiming playerAiming;
    private PlayerMovement playerMovement;
    private UnityEngine.AI.NavMeshAgent enemyAgent;

    bool firstSend=true;

    void Start()
    {
        apiKey = ApiKeyLoader.LoadApiKey();

        if (!enemyTank) enemyTank = GameObject.FindGameObjectWithTag("Enemy");
        if (!playerTank) playerTank = GameObject.FindGameObjectWithTag("Player");

        aiController = enemyTank.GetComponent<AIController>();
        enemyHealth = enemyTank.GetComponent<Health>();
        playerHealth = playerTank.GetComponent<Health>();
        playerAiming = playerTank.GetComponent<PlayerAiming>();
        playerMovement = playerTank.GetComponent<PlayerMovement>();
        enemyAgent = enemyTank.GetComponent<UnityEngine.AI.NavMeshAgent>();

        SendInitialPrompt();
        StartCoroutine(SendSituationEveryXSeconds(7f));
    }

    private void SendInitialPrompt()
    {
        string introMessage =
            "Hi, I want to create a game in Unity where the opponent is controlled by a neural network. " +
            "My game is about tanks, with both the player and the opponent being tanks. During the game, events occur, and the neural network receives information about what is happening, the statistics of the tank it controls, and so on.\n" +
            "For example, you are fighting in a field with only a few cover spots. The opponent (you) gets hit, and you receive a notification with the word \"Hit.\" " +
            "You also receive statistics showing that you have 150 health points left, while the enemy deals 200 damage. This means you won’t survive the next shot. You need to respond as quickly as possible to determine how the tank should act.\n" +
            "I will provide you with a description of the map, your tank, the enemy tank, and the event. You must choose one of the tactics from the list I give you. Here is the list:\n" +
            "Retreat:\n• \"Run away\"\n• \"Retreat backward without cover\"\n• \"Retreat using cover\"\n" +
            "Defense:\n• \"Hold position defensively\"\n• \"Fire back using cover\"\n• \"Stand still and fire back\"\n" +
            "Attack:\n• \"Advance slowly using cover\"\n• \"Attack while keeping distance\"\n• \"Attack at close range\"\n" +
            "Breakthrough:\n• \"Move in and attack from the side\"\n• \"Move in and attack from behind\"\n\n" +
            "Map:\nMiddle-sized map with covers in center.\n" +
            "Analyze this data. Next, I will give you a situation, and you must quickly choose the most logical course of action from the list.\n" +
            "You should write ONLY what is in quotation marks."+
            "Now, send me inital tactic, and it should be something offensive";

        StartCoroutine(SendRequest(introMessage));
    }

    IEnumerator SendSituationEveryXSeconds(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            string situationPrompt = GenerateSituationPrompt();
            StartCoroutine(SendRequest(situationPrompt));
        }
    }

    private string GenerateSituationPrompt()
    {
        float yourHP = enemyHealth.GetHP();
        float enemyHP = playerHealth.GetHP();

        float yourDamage = aiController.GetDamage();
        float enemyDamage = playerAiming.GetDamage();

        float yourReload = aiController.GetRemainingReload();
        float enemyReload = playerAiming.GetRemainingReload();

        float distToEnemy = Vector2.Distance(enemyTank.transform.position, playerTank.transform.position);
        float distToCover = FindNearestCoverDistance();

        float yourSpeed = enemyAgent != null ? enemyAgent.speed : 2f;
        float enemySpeed = playerMovement != null ? playerMovement.GetSpeed() : 5f;

        string prompt =
            $"Your HP: {yourHP}, " +
            $"Enemy HP: {enemyHP}, " +
            $"Your Damage: {yourDamage}, " +
            $"Enemy Damage: {enemyDamage}, " +
            $"You will be able to shoot in {yourReload:F1}s, " +
            $"Enemy will be able to shoot in {enemyReload:F1}s, " +
            $"Distance to enemy: {distToEnemy:F1}m, " +
            $"Distance to cover: {distToCover:F1}m, " +
            $"Your speed: {yourSpeed} m/s, " +
            $"Enemy speed: {enemySpeed} m/s."+
            "Write ONLY tactic from the list";

        return prompt;
    }

    private float FindNearestCoverDistance()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        float minDistance = float.MaxValue;

        foreach (GameObject cover in covers)
        {
            float distance = Vector2.Distance(enemyTank.transform.position, cover.transform.position);
            if (distance < minDistance)
                minDistance = distance;
        }

        return minDistance == float.MaxValue ? 999f : minDistance;
    }

    IEnumerator SendRequest(string prompt)
    {
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;
            string reply = ParseChatGPTResponse(result);
            Debug.Log("📥 ChatGPT reply: " + reply);
            aiController.ApplyTactic(reply);
        }
        else
        {
            Debug.LogError("❌ Request failed: " + request.error);
            aiController.ApplyTactic("hold position defensively");
        }
    }

    private string ParseChatGPTResponse(string json)
{
    JObject response = JObject.Parse(json);
    string reply = response["choices"]?[0]?["message"]?["content"]?.ToString();
    
    // Витягуємо тільки те, що в лапках
    int firstQuote = reply.IndexOf('\"');
    int lastQuote = reply.LastIndexOf('\"');

    if (firstQuote >= 0 && lastQuote > firstQuote)
    {
        return reply.Substring(firstQuote + 1, lastQuote - firstQuote - 1).Trim().ToLower();
    }

    return reply?.Trim().ToLower(); // fallback
}

public void ResendReminder()
{
    string reminderPrompt =
        "Your answer did not match any of the available tactics. " +
        "Please send ONLY one of the tactics from the list below, exactly as written:\n" +
        "\"Run away\", \"Retreat backward without cover\", \"Retreat using cover\", " +
        "\"Hold position defensively\", \"Fire back using cover\", \"Stand still and fire back\", " +
        "\"Advance slowly using cover\", \"Attack while keeping distance\", \"Attack at close range\", " +
        "\"Move in and attack from the side\", \"Move in and attack from behind\".\n" +
        "Respond ONLY with one of the tactics in quotation marks. Do not explain or comment.";

    StartCoroutine(SendRequest(reminderPrompt));
}

}
