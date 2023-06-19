using UnityEngine;
using UnityEngine.UI;
using MyShooter.NetworkSetup;
using NetworkLib.Common.DTOs;
using MyShooter.GameControl;

namespace MyShooter.UI
{
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
            LoginButton.onClick.AddListener(Login);
            QueueButton = _buttons[1];
            QueueButton.onClick.AddListener(Queue);
            OptionsButton = _buttons[2];
            OptionsButton.onClick.AddListener(Options);
            ExitButton = _buttons[3];
            ExitButton.onClick.AddListener(Exit);
        }

        public void Login()
        {
            string username = "username_" + Random.Range(0, 1000);
            GameController.Instance.LocalPlayer.Username = username;
            ClientInit.Instance.SendLogin(username, "password");
        }

        public void Options()
        {
            ClientInit.Instance.Client.Send(new TestMessage("test"));
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
}