using Assets.Src.Net.Envelopes.Server;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginFormController : MonoBehaviour
{
    public Canvas LoginCanvas;
    public InputField LoginInput;
    public Button LoginButton;
    public Animator LoginWindowAnimator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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

    public void hideLoginForm()
    {
        Debugger.Log("hiding login form...");
        LoginCanvas.enabled = false;

    }

    public void AaaOoo()
    {
        
    }

}
