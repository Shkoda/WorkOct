using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Src.Battle.GUI
{
    internal class LoginForm : MonoBehaviour
    {
        public static string login = "";
        private string password = "", rePass = "", message = "";

        private bool registrationForm = false;

        public void OnGUI()
        {
            if (message != "")
                GUILayout.Box(message);

            if (registrationForm)
            {
                GUILayout.Label("Login");
                login = GUILayout.TextField(login);
                GUILayout.Label("Password");
                password = GUILayout.PasswordField(password, "*"[0]);
                GUILayout.Label("Re-password");
                rePass = GUILayout.PasswordField(rePass, "*"[0]);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Back"))
                    registrationForm = false;

                if (GUILayout.Button("Register"))
                {
                    message = "";

                    if (login == "" || name == "" || password == "")
                        message += "Please enter all the fields \n";
                    else
                    {
                        if (password == rePass)
                        {
                            WWWForm form = new WWWForm();
                            form.AddField("login", login);
                            form.AddField("name", name);
                            form.AddField("password", password);
                            //                        WWW w = new WWW("http://f6-preview.awardspace.com/unitytutorial.com/registrationForm.php", form);
                            //                        StartCoroutine(registerFunc(w));
                        }
                        else
                            message += "Your Password does not match \n";
                    }
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("User:");
                login = GUILayout.TextField(login);
                GUILayout.Label("Password:");
                password = GUILayout.PasswordField(password, "*"[0]);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Login"))
                {
                    message = "";

                    if (login == "" || password == "")
                        message += "Please enter all the fields \n";
                    else
                    {
                        WWWForm form = new WWWForm();
                        form.AddField("login", login);
                        form.AddField("password", password);
                        //                    WWW w = new WWW("http://f6-preview.awardspace.com/unitytutorial.com/login.php", form);
                        //                    StartCoroutine(login(w));
                    }
                }

                if (GUILayout.Button("Register"))
                    registrationForm = true;

                GUILayout.EndHorizontal();
            }
        }
    }
}