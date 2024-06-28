using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CobaHealthSlider : MonoBehaviour
{
    public Slider healthBarSlider;
    public Slider easeHealthBarSlider;
    public float maxHealth = 100f;
    public float currentHealth;
    public float lerpSpeed = 0.05f;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBarSlider.value != currentHealth)
        {
            healthBarSlider.value = currentHealth;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            takingDamage(10);
        }
        if(healthBarSlider.value != easeHealthBarSlider.value)
        {
            easeHealthBarSlider.value = Mathf.Lerp(easeHealthBarSlider.value, currentHealth, lerpSpeed);
        }
    }

    void takingDamage(float damage)
    {
        currentHealth -= damage;
    }

}
