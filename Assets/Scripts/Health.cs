using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] float MaxHealth = 100;
    private float currentHealth;

    [Header("UI References")]
    [SerializeField] Image healthImg;
    [SerializeField] Image backImage;
    [SerializeField] float HpAnimSpeed = 1;

    private float targetFillAmount;
    private Coroutine currentCoroutine;


    void Awake()
    {
        if (MaxHealth <= 0)
            MaxHealth = 1;

        currentHealth = MaxHealth;

        if (healthImg)
            healthImg.fillAmount = 1;

        if (backImage)
            backImage.fillAmount = 1;
    }

    void Start()
    {

    }

    public void Damage(float damage)
    {
        if (damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(this.gameObject.transform.root.gameObject);
            return;
        }

        if (healthImg)
        {
            healthImg.fillAmount = currentHealth / MaxHealth;

            if (backImage)
            {
                targetFillAmount = currentHealth / MaxHealth;

                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(AnimateHealthBar());
            }
        }

    }

    private IEnumerator AnimateHealthBar()
    {
        while (Mathf.Abs(backImage.fillAmount - targetFillAmount) > 0.01f)
        {
            float fillAmount = Mathf.Lerp(backImage.fillAmount, targetFillAmount, Time.deltaTime * HpAnimSpeed);
            backImage.fillAmount = fillAmount;
            yield return null;
        }
        backImage.fillAmount = targetFillAmount;
    }

    public float GetHP() => currentHealth;
}
