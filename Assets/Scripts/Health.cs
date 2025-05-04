using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] float MaxHealth =100;
    private float currentHealt;

    [SerializeField] Image healthImg;

    [SerializeField] Image backImage;
    [SerializeField] float HpAnimSpeed=1;
    private float targetFillAmount;
    private Coroutine currentCoroutine;
    // Start is called before the first frame update
    void Awake()
    {
        if(MaxHealth<=0)
        MaxHealth=1;

        currentHealt = MaxHealth;

        if(healthImg)
        healthImg.fillAmount=1;

        if(backImage)
        backImage.fillAmount=1;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damage)
    {
        if(damage<=0)
        return;

        currentHealt-=damage;

        if(currentHealt<=0)
        {
            currentHealt=0;
            Destroy(this.gameObject.transform.root.gameObject);
            return;
        }

        if(healthImg)
        {
            healthImg.fillAmount=currentHealt/MaxHealth;

            if(backImage)
            {
                targetFillAmount=currentHealt/MaxHealth;

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
            float fillAmount= Mathf.Lerp(backImage.fillAmount, targetFillAmount, Time.deltaTime * HpAnimSpeed);
            Debug.Log("Back fill am" + fillAmount);
            backImage.fillAmount = fillAmount;
            yield return null;
        }
        backImage.fillAmount = targetFillAmount;
    }

    
}
