
using UnityEngine;
using UnityEngine.UI;
using MyGame.NetworkSetup;
using MyGame;
using MyGame.DTOs;

public class CanvasController : MonoBehaviour
{
    private Button[] _buttons;

    public Button LoginButton;
    public Button QueueButton;
    public Button OptionsButton;
    public Button ExitButton;

    public static CanvasController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        _buttons = GetComponentsInChildren<Button>();

        LoginButton = _buttons[0];
        QueueButton = _buttons[1];
        OptionsButton = _buttons[2];
        ExitButton = _buttons[3];
    }

    public void Login()
    {
        string username = "username_" + Random.Range(0, 1000);
        GameController.Instance.LocalPlayer.SetUser(new User(username));
        ClientInit.Instance.SendLogin(username, "password");
    }

    public void Options()
    {

    }

    public void Queue()
    {
        ClientInit.Instance.SendQueue();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
