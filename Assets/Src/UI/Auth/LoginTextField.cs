using UnityEngine;
using UnityEngine.UI;

namespace Assets.Src.UI.Auth
{
    public class LoginTextField : MonoBehaviour
    {
        public InputField LoginInput;
        public Button LoginButton;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            LoginButton.interactable = LoginInput.text != null && LoginInput.text.text.Length > 0;
        }
    }
}