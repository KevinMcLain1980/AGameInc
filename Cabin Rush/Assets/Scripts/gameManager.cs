using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Menus")]
    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPause;
    [SerializeField] GameObject MenuWin;
    [SerializeField] GameObject MenuLose;

    [Header("Player UI")]
    public Slider playerHPBar;
    public Slider playerStaminaBar;
    public Slider playerOxygenBar;
    public GameObject PlayerDmgPanel;

    public bool isPaused;
    public GameObject Player;

    float timeScaleOriginal;
    int GameGoalCount;

    void Awake()
    {
        instance = this;
        Player = GameObject.FindWithTag("Player");
        timeScaleOriginal = Time.timeScale;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (MenuActive == null)
            {
                statePause();
                MenuActive = MenuPause;
                MenuActive.SetActive(true);
            }
            else if (MenuActive == MenuPause)
            {
                stateUnPaused();
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnPaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MenuActive.SetActive(false);
        MenuActive = null;
    }

    public void UpdateGameGoal(int amount)
    {
        GameGoalCount += amount;

        if (GameGoalCount <= 0)
        {
            statePause();
            MenuActive = MenuWin;
            MenuActive.SetActive(true);
        }
    }

    public void Loser()
    {
        statePause();
        MenuActive = MenuLose;
        MenuActive.SetActive(true);
    }

    // ✅ UI Update Methods
    public void UpdatePlayerHealth(float currentHealth, float maxHealth)
    {
        if (playerHPBar != null)
            playerHPBar.value = currentHealth / maxHealth;
    }

    public void UpdatePlayerStamina(float currentStamina, float maxStamina)
    {
        if (playerStaminaBar != null)
            playerStaminaBar.value = currentStamina / maxStamina;
    }

    public void UpdatePlayerOxygen(float currentOxygen, float maxOxygen)
    {
        if (playerOxygenBar != null)
            playerOxygenBar.value = currentOxygen / maxOxygen;
    }
}
