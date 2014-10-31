using Assets.Src.Net.Envelopes.Server;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Src.UI.Auth
{
    public class LoginButton : MonoBehaviour
    {
        public InputField LoginInputField;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
         
        }

        public void sendLoginToServer()
        {
            string login = LoginInputField.text.text;
            if (login.Length == 0)
                return;
            Debugger.Log("LoginButton.sendLoginToServer "+login);
            new SNewPlayerEnvelope(login).Send();
        }

    }
}