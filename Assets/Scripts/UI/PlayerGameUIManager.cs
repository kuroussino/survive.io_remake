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
    [SerializeField] GameObject playerAliveUI;
    [SerializeField] GameObject playerDeathUI;
    [SerializeField] TextMeshProUGUI scopeTextUI;
    [SerializeField] Slider playerHPSlider;
    [SerializeField] Image PrimaryWeaponSprite;
    [SerializeField] TextMeshProUGUI PrimaryWeaponName;
    [SerializeField] TextMeshProUGUI healthPackNumber;
    [SerializeField] TextMeshProUGUI currentBulletsText;
    [SerializeField] TextMeshProUGUI playerCountText;
    [SerializeField] Image armorImage;

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
    private Coroutine notificationCoroutine;
    [Tooltip("If set to true it will initialize the UI without any event, showin how would the UI look at the start of the game")]
    [SerializeField]bool debug=false;
    [Tooltip("Seconds needed to make the text of notification disappear")]
    [SerializeField] private float notificationMessageDisappear;
    #endregion

    #region Mono
    private void Start()
    {
#if !UNITY_EDITOR
        debugMode = false;
#endif
        if (debug)
        {
            InitPlayerUI(100, 1, currentWeapon);
        }

    }
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
        EventsManager.OnGetArmor += OnGetArmor;
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
        EventsManager.OnGetArmor -= OnGetArmor;
    }

    #endregion

    #region Custom Methods
    /// <summary>
    /// This method is called by an event as soon as the game starts to compile the player UI.
    /// </summary>
    private void InitPlayerUI(float maxHP,int scope,A_Weapon weapon)
    {
        playerAliveUI.SetActive(true);
        playerDeathUI.SetActive(false);
        playerHPSlider.maxValue = maxHP;
        playerHPSlider.value = maxHP;
        playerHPSlider.minValue= 0;
        playerHPSlider.value = maxHP;
        currentBullets = 0;
        notificationText.gameObject.SetActive(false);
        armorImage.enabled = false;
        OnWeaponUpdateBullets(currentBullets);
        UpdateHealthPackNumber(0);
        UpdatePrimaryWeaponUI();
    }
    /// <summary>
    /// This method is called by an event when the local player take damage
    /// </summary>
    /// <param name="amount"></param>
    private void OnDamageTaken(float newHealthAmount, float maxHealth)
    {
        newHealthAmount = Mathf.Clamp(newHealthAmount, 0, maxHealth);
        playerHPSlider.maxValue = maxHealth;
        playerHPSlider.value = newHealthAmount;
        CheckLowHealthPlayer(newHealthAmount, maxHealth);
    }
    /// <summary>
    /// This method is called by an event when the local player will heal, updating the amount of heal
    /// </summary>
    /// <param name="amount"></param>
    private void OnPlayerHeal(float newHealthAmount, float maxHealth)
    {
        newHealthAmount = Mathf.Clamp(newHealthAmount, 0, maxHealth);
        playerHPSlider.maxValue = maxHealth;
        playerHPSlider.value = newHealthAmount;
        CheckLowHealthPlayer(newHealthAmount, maxHealth);
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
        if (currentWeapon != null)
        {
            Color spritecolor = PrimaryWeaponSprite.color;
            spritecolor.a = 1;
            PrimaryWeaponSprite.color = spritecolor;
            PrimaryWeaponName.text = currentWeapon.name;
            PrimaryWeaponSprite.sprite = currentWeapon.GetSpriteItem();
        }
        else
        {
            Color spritecolor = PrimaryWeaponSprite.color;
            PrimaryWeaponName.text = "";
            PrimaryWeaponSprite.sprite = null;
            spritecolor.a= 0;
            PrimaryWeaponSprite.color = spritecolor;
        }

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
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        notificationCoroutine = StartCoroutine(NotificationDisappearing());
    }
    /// <summary>
    /// Coroutine that will make the text disappear, will refresh when called again
    /// </summary>
    /// <returns></returns>
    IEnumerator NotificationDisappearing()
    {
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(notificationMessageDisappear);
        notificationText.gameObject.SetActive(false);
    }
    /// <summary>
    /// The moethod is called when taking or healing damage in order to check if the player is lowHealth or not.
    /// </summary>
    /// <param name="hp"></param>
    private void CheckLowHealthPlayer(float currentHP, float maxHP)
    {
        if (currentHP > maxHP * 0.25)
        {
            playerHPSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
        else
        {
            playerHPSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }
    /// <summary>
    /// Method called by event to get armor shown in UI 
    /// </summary>
    private void OnGetArmor()
    {
        armorImage.enabled = true;
    }
    /// <summary>
    /// This method is called by an event on local player death
    /// </summary>
    private void OnPlayerDeath()
    {
        playerAliveUI.SetActive(false);
        playerDeathUI.SetActive(true);
    }
    
    #endregion
}
