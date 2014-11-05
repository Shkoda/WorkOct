using Assets.Src.Net.Envelopes.Server;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginFormController : MonoBehaviour
{
//    public Canvas LoginCanvas;
    public GameObject LoginPanel;
    public InputField LoginInput;
    public Button LoginButton;
    public Animator LoginWindowAnimator;

    public RoomListController RoomListController;

    // Use this for initialization
    private void Start()
    {
        LoginButton.interactable = false;
    }

    // Update is called once per frame
    private void Update()
    {
      UpdateLoginButtonAvailability();
    }

    public void UpdateLoginButtonAvailability()
    {
        LoginButton.interactable = LoginInput.text != null && LoginInput.text.text.Length > 0;
    }

    public void sendLoginToServer()
    {
        string login = LoginInput.text.text;
        if (login.Length == 0)
            return;
        Debugger.Log("LoginButton.sendLoginToServer " + login);
        new SNewPlayerEnvelope(login).Send();
        LoginWindowAnimator.SetBool("LoggedIn", true);
    }

    public void DisableLoginForm()
    {
        Debugger.Log("hiding login form...");
        LoginPanel.SetActive(false);
        RoomListController.SetRoomPanelEnabled(true);
    }

 
}