using System;
using System.Collections;
using Assets.Src.Battle;
using Assets.Src.Net;
using Assets.Src.Net.Handler;
using Assets.Src.Scenes;
using Assets.Src.Threading;
using Assets.Src.Tools;
using UnityEngine;

public class OctClient : MonoBehaviour
{


    private static OctClient _instance;

    private readonly GUIStyle style = new GUIStyle(GUIStyle.none);
    private bool isServerSelected;

    public static BattleGame CurrentGame { get; private set; }

    /// <summary>
    ///     Available after Awake() is called.
    /// </summary>
    public static GameObject GameObject
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }

            return _instance.gameObject;
        }
    }

    public static event Action PreparedForLogIn = delegate { };

    private void Awake()
    {
        // To be ready at Start() time.
        _instance = this;
        CurrentGame = new BattleGame();
    }

    private void Start()
    {
        //Enable debugging mode
        Debugger.WriteDebugToServer = false;

        //For buildnumber
        style.alignment = TextAnchor.UpperRight;
        style.normal.textColor = Color.white;

        if (!gameObject.GetComponent<Debugger>())
        {
            gameObject.AddComponent<Debugger>();
        }

        //Some settings for threading on mobile devices
        //            ThreadPool.SetMaxThreads(10, 10);

        //Setup threading manager
        ThreadManager.SetMainThread();

        //Connect to gameserver without using balancing server, when launching on facebook
        //Build agent will replace Buildnum with build number
        ConnectGameToServer();
    }

    private void Update()
    {
        MainThread.Update();

        SceneLoader.Update();

        //GameWorld.SynchronizationManager.SendPing();

        if (isServerSelected)
        {
            NetworkHandler.ProcessServerQueue();
            NetworkHandler.ProcessClientQueue();
        }
    }

    public void OnApplicationQuit()
    {
        NetworkHandler.Disconnect(true);

        ThreadManager.OutputRunningThreads();
        ThreadManager.AbortAll();

        //Allow last logs to write debug info
        MainThread.Update();
    }

    /// <summary>
    ///     Resets the game, throwing player back to loading screen
    /// </summary>
    public static void Reset()
    {
        Debug.Log("reset");
        MainThread.AddOnce(_Reset);
    }

    private static void _Reset(object[] obj)
    {
        if (_instance == null)
        {
            Debug.LogWarning("Already reset");
            return;
        }

        try
        {
            UserDataPermanentStorage.ClearGuid();
            //NetworkHandler.Disconnect(true);
            //Destroy(_instance.gameObject);
            //Destroy(PrefabFactory.gameObject);

            //_instance = null;
            //PrefabFactory = null;
            //loaderScreen = null;
            //pinger = null;

            SceneLoader.Reset();
            //LoadingScreen.DestroyScreen();
            //Destroy(loaderScreen);

            //Application.LoadLevel(0);
        }
        finally
        {
            ThreadManager.OutputRunningThreads();
        }
        // Reconnects after receiving sessioninvalid
        //_instance.Start();
    }

    public static void Quit()
    {
        Application.Quit();
        //Process.GetCurrentProcess().Kill();
    }

    private void OnLogoRemoveComplete()
    {
    }

    public static void StartMainThreadCoroutine(IEnumerator routine)
    {
        if (!CheckInstance())
        {
            return;
        }

        _instance.StartCoroutine(routine);
    }

    public static void RemoveMainThreadCoroutine(IEnumerator routine)
    {
        if (!CheckInstance())
        {
            return;
        }
        _instance.StopCoroutine(routine);
    }

    public static void StopAllMainThreadCoroutines()
    {
        if (!CheckInstance())
        {
            return;
        }
        _instance.StopAllCoroutines();
    }

    public static void ConnectGameToServer()
    {
        if (!CheckInstance())
        {
            return;
        }


        NetworkHandler.ConnectAndSendVersion(ConnectionConfig.Server, ConnectionConfig.Port, false);

        NetworkHandler.LostConnection += NetworkHandler_LostConnection;
        NetworkHandler.ServerError += NetworkHandler_ServerError;
        _instance.isServerSelected = true;
    }

    /// <summary>
    ///     CSessionKey packet handler
    /// </summary>
    /// <param name="sessionKey"></param>
    public static void CreateNewSession(ulong sessionKey)
    {
        NetworkHandler.SetSessionKey(sessionKey);

        //If player wasn't logged in
        //if (!CurrentGame.AccountController.IsLoggedIn)
        {
            //Login differs depending on platform
            //CurrentGame.AccountController.StartLoginSequence();
            PreparedForLogIn();
        }
    }

    public static void Reconnect()
    {
        Debug.Log("reconnect");
        try
        {
            NetworkHandler.Disconnect(true);
        }
        finally
        {
            _instance.Start();
        }
    }

    private static void NetworkHandler_LostConnection()
    {
        Debug.Log("IsRunningInMainThread = " + ThreadManager.IsRunningInMainThread);

        if (ThreadManager.IsRunningInMainThread)
        {
            _instance.LostConnection();
        }
        else
        {
            MainThread.AddOnce(nothing => _instance.LostConnection());
        }
    }

    public static void NetworkHandler_ServerError()
    {
        Debug.Log("IsRunningInMainThread = " + ThreadManager.IsRunningInMainThread);

        if (ThreadManager.IsRunningInMainThread)
        {
            _instance.ServerError();
        }
        else
        {
            MainThread.AddOnce(nothing => _instance.ServerError());
        }
    }

    private void ServerError()
    {
    }

    private void LostConnection()
    {
    }

    private void WrongPacket()
    {
    }


    private static bool CheckInstance()
    {
        if (_instance == null)
        {
            Debug.LogError("Scene misses PlatformClient object.");
            return false;
        }
        return true;
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "Walls"))
        {
            //                CurrentGame.CreateWorld(null);
            //                CurrentGame.AddPlayer();
        }
    }
}