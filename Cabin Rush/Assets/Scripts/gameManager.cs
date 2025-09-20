using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Menus")]
    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPause;
    [SerializeField] GameObject MenuWin;
    [SerializeField] GameObject MenuLose;

    [Header("Player UI")]
    public Image playerHPBar;
    public Image playerStaminaBar;
    public Image playerOxygenBar;
    public GameObject PlayerDmgPanel;

    public bool isPaused;
    public GameObject Player;

    private PlayerControls controls;
    private float timeScaleOriginal;
    private int GameGoalCount;

    private void Awake()
    {
        instance = this;
        Player = GameObject.FindWithTag("Player");
        timeScaleOriginal = Time.timeScale;

        controls = new PlayerControls();
        controls.Player.Cancel.performed += ctx => HandlePauseToggle();
    }

    private void OnEnable() => controls?.Enable();
    private void OnDisable() => controls?.Disable();

    private void HandlePauseToggle()
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

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnPaused()
    {
        isPaused = false;
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
}
