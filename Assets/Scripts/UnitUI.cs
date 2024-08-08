using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Handles all of the functionality for displaying the Unit's UI.
public class UnitUI : MonoBehaviour
{
    public RectTransform container;
    public Image healthBar;
    public Vector3 offset = new Vector3(0, 2, 0);
    private IDamageable trackedDamageable;
    private Transform trackedTransform;

    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float AttackStaminaCost;
    public float RunStaminaCost;
    public float ChargeRate;
    private Coroutine recharge;

    // Cache initial references to the unit and damageable interface.
    private void Awake()
    {
        trackedDamageable = GetComponentInParent<IDamageable>();
        trackedTransform = transform.parent;
    }
 

    //drain stamina while attacking
    public void AttackStaminaDrain()
    {
        Stamina -= AttackStaminaCost;
        if (Stamina < 0)
        {
            Stamina = 0;
        }
        StaminaBar.fillAmount = Stamina / MaxStamina;

        //ensures that two coroutines dont occur at the same time
        if (recharge != null) StopCoroutine(recharge);
        recharge = StartCoroutine(RechargeStamina());
    }

    //drain stamina while running
    public void RunStaminaDrain()
    {
        Stamina -= RunStaminaCost * Time.deltaTime;
        if (Stamina < 0)
        {
            Stamina = 0;
        }
        StaminaBar.fillAmount = Stamina / MaxStamina;

        if (recharge != null) StopCoroutine(recharge);
        recharge = StartCoroutine(RechargeStamina());
    }


    // Update the position and display of the UI every frame.
    private void LateUpdate()
    {
        // Move the health bar to the correct screen position.
        Vector3 world = trackedTransform.position + offset;
        container.anchoredPosition = Camera.main.WorldToScreenPoint(world);

        // Update the amount of the red health bar which is visible.
        healthBar.fillAmount = trackedDamageable.GetCurrentHealthPercent();
    }

    //recharge stamina
    public IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(2.5f);
        
        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f; 
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(.1f);
        }
    }
}
