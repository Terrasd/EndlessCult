using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] private CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    private float health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return health; }

        // If we try and set the current health, the UI interface
        // on the pause screen will also be updated.
        set
        {
            // Check if the value has changed
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }
    #endregion

    [Header("Visuals")]
    public ParticleSystem damageEffect; // If damage is dealt
    public ParticleSystem blockedEffect; // If armor completely blocks damage

    // XP and level of the player
    [Header("XP / Level")]
    public int xp = 0;
    public int level = 1;
    public int xpCap;

    // Class for defining a level range and the corresponding XP cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int xpCapIncrease;
    }
    public List<LevelRange> levelRanges;

    private PlayerCollector collector;

    [Header("I-Frames")]
    public float invicibilityDuration;
    private float invicibilityTimer;
    private bool isInvicible;

    private PlayerInventory inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image xpBar;
    public TMP_Text levelText;

    private void Awake()
    {
        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance)
        {
            CharacterSelector.instance.DestroySingleton();
        }

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        // Assign the variables
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    private void Start()
    {
        // Spawn the starting weapon
        inventory.Add(characterData.StartingWeapon);

        // Initialize the XP cap as the first XP cap increase 
        xpCap = levelRanges[0].xpCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateXPBar();
        UpdateLevelText();
    }

    private void Update()
    {
        if (invicibilityTimer > 0)
        {
            invicibilityTimer -= Time.deltaTime;
        }
        else if (isInvicible)
        {
            isInvicible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        collector.SetRadius(actualStats.magnet);
    }

    public void IncreaseXP(int amount)
    {
        xp += amount;

        LevelUpChecker();

        UpdateXPBar();
    }

    private void LevelUpChecker()
    {
        if (xp >= xpCap)
        {
            level++;
            xp -= xpCap;

            int xpCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    xpCapIncrease = range.xpCapIncrease;
                    break;
                }
            }
            xpCap += xpCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();
        }
    }

    private void UpdateXPBar()
    {
        xpBar.fillAmount = (float)xp / xpCap;
    }

    private void UpdateLevelText()
    {
        levelText.text = "Level " + level.ToString();
    }

    public void TakeDamage(float damage)
    {
        // If the player is not currently invicible, reduce health and start invicibility
        if (!isInvicible)
        {
            // Take armor into account before dealing the damage
            damage -= actualStats.armor;

            if (damage > 0)
            {
                // Deal the damage
                CurrentHealth -= damage;

                // If there is a damage effect assigned, play it
                if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                // If there is a blocked effect assigned, play it
                if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            invicibilityTimer = invicibilityDuration;
            isInvicible = true;
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }

    private void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;

            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }
}
