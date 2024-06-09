using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGameUIManager : MonoBehaviour
{
    #region Player UI var
    [Space(5)]
    [Header("Player UI")]
    [SerializeField] GameObject playerUI;
    [SerializeField] TextMeshProUGUI scopeTextUI;
    [SerializeField] Slider playerHPSlider;
    [SerializeField] Image PrimaryWeaponSprite;
    [SerializeField] TextMeshProUGUI PrimaryWeaponName;
    [SerializeField] TextMeshProUGUI healthPackNumber;
    [SerializeField] TextMeshProUGUI currentBulletsText;
    [SerializeField] TextMeshProUGUI playerCountText;

    [Space(10)]
    [Header("Notification UI")]
    [SerializeField] TextMeshProUGUI notificationText;
    [Tooltip("The color of the notification text appearing on screen")]
    [SerializeField] Color notificationColor;

    #endregion
    #region Variables
    private float maxHP;
    private float currentHP;
    private int currentBullets;
    private int currentHealthPacks;
    private A_Weapon currentWeapon;
    #endregion

    #region Mono
    private void OnEnable()
    {
        EventsManager.PlayerUIInitialize += InitPlayerUI;
        EventsManager.OnPlayerDead += OnPlayerDeath;
        EventsManager.OnPlayerDamage += OnDamageTaken;
        EventsManager.OnPlayerHeal += OnPlayerHeal;
        EventsManager.OnNewWeapon += OnGetWeapon;
        EventsManager.OnGrabHealthPack += OnGetHealthPack;
        EventsManager.OnUpdateBulltes += OnWeaponUpdateBullets;
        EventsManager.OnNotificatePlayer += OnNotificationText;
        EventsManager.OnPlayerUseHealthPack += OnUseHealthPack;
        EventsManager.OnUpdatePlayerCount += UpdatePlayerCount;
    }
    private void OnDisable()
    {
        EventsManager.PlayerUIInitialize -= InitPlayerUI;
        EventsManager.OnPlayerDead -= OnPlayerDeath;
        EventsManager.OnPlayerDamage -= OnDamageTaken;
        EventsManager.OnPlayerHeal -= OnPlayerHeal;
        EventsManager.OnNewWeapon -= OnGetWeapon;
        EventsManager.OnGrabHealthPack -= OnGetHealthPack;
        EventsManager.OnUpdateBulltes -= OnWeaponUpdateBullets;
        EventsManager.OnNotificatePlayer -= OnNotificationText;
        EventsManager.OnPlayerUseHealthPack -= OnUseHealthPack;
        EventsManager.OnUpdatePlayerCount -= UpdatePlayerCount;
    }

    #endregion

    #region Custom Methods
    /// <summary>
    /// This method is called by an event as soon as the game starts to compile the player UI.
    /// </summary>
    private void InitPlayerUI(float maxHP,int scope,A_Weapon weapon)
    {
        playerUI.SetActive(true);
        playerHPSlider.maxValue = maxHP;
        playerHPSlider.minValue= 0;
        currentHealthPacks = 0;
        currentWeapon = weapon;
    }
    /// <summary>
    /// This method is called by an event when the local player take damage
    /// </summary>
    /// <param name="amount"></param>
    private void OnDamageTaken(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        playerHPSlider.value = currentHP;
        CheckLowHealthPlayer(currentHP);
    }
    /// <summary>
    /// This method is called by an event when the local player will heal, updating the amount of heal
    /// </summary>
    /// <param name="amount"></param>
    private void OnPlayerHeal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        playerHPSlider.value = currentHP;
        CheckLowHealthPlayer(currentHP);
    }
    /// <summary>
    /// This method is called by an event when the local player get a new primary weapon
    /// </summary>
    /// <param name="weapon"></param>
    private void OnGetWeapon(A_Weapon weapon,int bulletsCurrentlyInWeapon)
    {
        currentWeapon = weapon;
        OnWeaponUpdateBullets(bulletsCurrentlyInWeapon);
        UpdatePrimaryWeaponUI();
    }
    /// <summary>
    /// This method is going to update the weapon UI only. 
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdatePrimaryWeaponUI()
    {
        PrimaryWeaponName.text = currentWeapon.name;
        PrimaryWeaponSprite.sprite = currentWeapon.GetSpriteItem();
    }
    /// <summary>
    /// This method is called when shooting/reloading or grabbing a new gun with a new number of bullets
    /// </summary>
    private void OnWeaponUpdateBullets(int amount)
    {
        currentBullets = amount;
        currentBulletsText.text = currentBullets.ToString();
    }
    /// <summary>
    /// This method is called by an event when the player grab a new HealtPack
    /// </summary>
    private void OnGetHealthPack()
    {
        UpdateHealthPackNumber(+1);
    }
    /// <summary>
    /// This method is called by an event when the local player use an healthPack;
    /// </summary>
    private void OnUseHealthPack()
    {
        UpdateHealthPackNumber(-1);
    }
    /// <summary>
    /// This method is called when using or grabbing a new healthpack;
    /// </summary>
    /// <param name="amount"></param>
    private void UpdateHealthPackNumber(int amount)
    {
        currentHealthPacks+=amount;  
        healthPackNumber.text=currentHealthPacks.ToString();
    }
    /// <summary>
    /// This method is called by an event from the server in order to execute on every client when a player is out of the game 
    /// </summary>
    private void UpdatePlayerCount(int playerLeftInGame)
    {
        playerCountText.text= playerLeftInGame.ToString();
    }
    /// <summary>
    /// This method is called when notificatying the player, like DeathZone or the winner of the game.
    /// </summary>
    /// <param name="message"></param>
    private void OnNotificationText(object message)
    {
        notificationText.text = message.ToString();
        notificationText.color = notificationColor;
    }
    /// <summary>
    /// The moethod is called when taking or healing damage in order to check if the player is lowHealth or not.
    /// </summary>
    /// <param name="hp"></param>
    private void CheckLowHealthPlayer(float hp)
    {
        if (hp > maxHP*0.25)
        {
            playerHPSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
        else
        {
            playerHPSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }
    /// <summary>
    /// This method is called by an event on local player death
    /// </summary>
    private void OnPlayerDeath()
    {
        playerUI.SetActive(false);
    }
    
    #endregion
}
