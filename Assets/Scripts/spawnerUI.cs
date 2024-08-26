using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

// Handles all of the functionality for displaying the Unit's UI.
public class spawnerUI : MonoBehaviour
{
    public RectTransform container;
    public Image healthBar;
    public Vector3 offset = new Vector3(0, 2, 0);
    private IDamageable trackedDamageable;
    private Transform trackedTransform; 
    private MonsterSpawner MonsterSpawner;
    private Player player;

    // Cache initial references to the unit and damageable interface.
    private void Awake()
    {
        MonsterSpawner = GameObject.FindObjectOfType<MonsterSpawner>();
        trackedDamageable = MonsterSpawner.GetComponent<IDamageable>();
        player = GameObject.FindObjectOfType<Player>();
    }


    private void Update()
    {
        if (player.isDead)
        {
            gameObject.SetActive(false);
            return;
        }
        if (MonsterSpawner.isDead)
        {
            gameObject.SetActive(false);
            return;
        }
    }
    // Update the position and display of the UI every frame.
    private void LateUpdate()
    {
        // Move the health bar to the correct screen position.
        //Vector3 world = trackedTransform.position + offset;


        // Update the amount of the red health bar which is visible.
        healthBar.fillAmount = trackedDamageable.GetCurrentHealthPercent();
    }

}