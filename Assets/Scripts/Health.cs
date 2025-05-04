using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP = 200;
    public float currentHP;
    public float damagePerShot = 40f;

    private ChatGPTClient chatGPTClient;

    void Start()
    {
        currentHP = maxHP;
        chatGPTClient = FindObjectOfType<ChatGPTClient>();

        /*if (CompareTag("Enemy"))
        {
            // Надсилаємо запит одразу при старті
            chatGPTClient?.SendSituationToChatGPT();
        }*/
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        /*if (CompareTag("Enemy") && chatGPTClient != null)
        {
            chatGPTClient.SendSituationToChatGPT();
        }*/

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public float GetHP() => currentHP;
    public float GetDamage() => damagePerShot;
}
