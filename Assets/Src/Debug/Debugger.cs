//#define NOLOG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

public partial class Debugger : MonoBehaviour
{
    public float ButtonHeight;

    public float ButtonWidth;

    public float AnimateSpeed = 30;

    private String ButtonText = "Debug";

    public string ScriptName = "Debugger";

    public int DebugWindowID = 5555;

    public static bool ConsoleModeOn { get; private set; }

    public static bool InterceptDebugMessages
    {
        get { return interceptDebugMessages; }
        set
        {
            interceptDebugMessages = value;
            if (value)
            {
                Application.RegisterLogCallback(LogCallback);
            }
            else
            {
                Application.RegisterLogCallback(null);
            }
        }
    }

    private static bool interceptDebugMessages;

    public static bool WriteToDebugger { get; set; }
    public static bool WriteDebugToServer { get; set; }
    public static bool WriteDebugToConsole { get; set; }

    private float buttonYPosition;

    private Rect windowRect;

    private const float Margin = 2f;

    private bool isDragging;

    private bool isMinimizing;

    private bool isMaximizing;

    private bool isMaximized;

    private bool isFixed;

    private GUIStyle buttonStyle, textStyle;

    private Vector2 scrollViewVector;

    private float deltaY;

    private static readonly List<Message> logList;

    private static readonly List<Message> logListFiltered;

    private static readonly Dictionary<int, int> logListIndices = new Dictionary<int, int>();

    private static readonly Dictionary<int, int> logListFilteredIndices = new Dictionary<int, int>();

    static Debugger()
    {
        logList = new List<Message>();
        logListFiltered = new List<Message>();

        WriteToDebugger = true;

        SetFilter((DebugType) 0x7FFFFFFF);
    }

    #region Updates, GUI

    // Use this for initialization
    private void Start()
    {
        InterceptDebugMessages = true;

        this.windowRect = new Rect(0, 0, Screen.width, 0);

        this.ButtonHeight = Screen.height*0.04f;
        this.ButtonWidth = this.ButtonHeight*3;

        this.buttonYPosition = Screen.height - this.ButtonHeight;

        this.AddConsoleCommands();

        CrashReport[] reports = CrashReport.reports;

        if (reports != null)
        {
            foreach (CrashReport crashReport in reports)
            {
                Log(
                    string.Format("Crash report from {0}:\n{1}", crashReport.time, crashReport.text),
                    DebugType.Exception,
                    sendToServer: true,
                    writeToUnityConsole: true);
                crashReport.Remove();
            }
        }
        else
        {
            Log("No crash reports found");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        this.UpdateConsole();
#if UNITY_FLASH
    //No touchscreen at all
        if (isDragging && Input.GetMouseButtonUp(0)) // mouse
        {
            isDragging = false;
            if (Input.mousePosition.y > Screen.height * 0.1 && Input.mousePosition.y < Screen.height * 0.94)
            {
                isFixed = true;
                isMaximized = false;
            }
            else if (Input.mousePosition.y > Screen.height * 0.1)
            {
                isMaximizing = true;
            }
            else
            {
                isMinimizing = true;
            }
        }
#else
        //Who knows, maybe someone wants to play on android without touchscreen
        if (this.isDragging && Input.GetMouseButtonUp(0)) // mouse
        {
            this.isDragging = false;
            if (Input.mousePosition.y > Screen.height*0.1 && Input.mousePosition.y < Screen.height*0.94)
            {
                this.isFixed = true;
                this.isMaximized = false;
            }
            else if (Input.mousePosition.y > Screen.height*0.1)
            {
                this.isMaximizing = true;
            }
            else
            {
                this.isMinimizing = true;
            }
        }
        else if (this.isDragging && Input.multiTouchEnabled
                 && (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)) //touchscreen
        {
            Touch touch = Input.GetTouch(0);
            this.isDragging = false;
            if (touch.position.y > Screen.height*0.1 && touch.position.x < Screen.height*0.9)
            {
                this.isFixed = true;
                this.isMaximized = false;
            }
            else if (touch.position.y > Screen.height*0.1)
            {
                this.isMaximizing = true;
            }
            else
            {
                this.isMinimizing = true;
            }
        }
#endif
    }

    public static readonly Dictionary<string, Action<string>> consoleCommands = new Dictionary<string, Action<string>>();

    private void UpdateConsole()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        if (consoleKeyboard != null && consoleKeyboard.done &&
            (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor))
        {
            if (consoleKeyboard.text == "q" || consoleKeyboard.text == "Q" || consoleKeyboard.text == "")
            {
                SwitchConsoleMode();
            }
            else
            {
                var lower = consoleKeyboard.text.ToLower();
                var key = lower.Split(' ')[0];
                if (consoleCommands.ContainsKey(key))
                {
                    consoleCommands[key](lower);
                    consoleKeyboard = TouchScreenKeyboard.Open("");
                }
                else
                {
                    AddConsoleRequest(consoleKeyboard.text);
//                    new GSRequestEnvelope(consoleKeyboard.text).Send();
                    consoleKeyboard = TouchScreenKeyboard.Open("");
                }
            }
        }
#endif
    }

    private static void LogCallback(string logMessage, string stackTrace, LogType type)
    {
        if (WriteDebugToConsole)
        {
            return;
        }

        if (WriteToDebugger)
        {
            _Log(logMessage + stackTrace, type == LogType.Exception ? DebugType.Exception : DebugType.UnityConsole);
        }

        if (WriteDebugToServer)
        {
//            if (type == LogType.Exception || type == LogType.Error)
//            {
//                new SDebugMessageEnvelope(
//                    string.Format(
//                        "Type: {0}.\r\nMessage: {1}.\r\nStackTrace: {2}.\r\nUTC time: {3}\r\n\r\n",
//                        type,
//                        logMessage,
//                        stackTrace,
//                        DateTime.UtcNow)).Send();
//            }
        }

#if UNITY_EDITOR
        if (logMessage.Equals("The compiler this script was imported with is not available anymore."))
        {
            Debug.LogWarning("Aborting player!");
//            EditorApplication.isPlaying = false;
        }
#endif
    }

    private string sendLogsName = "Send Logs";

    private void OnGUI()
    {
#if UNITY_EDITOR
        if (GUI.Button(new Rect(Screen.width - 80, Screen.height - 60, 80, 30), "Save Logs"))
        {
            SaveAllLogs();
        }
#endif
        if (GUI.Button(new Rect(Screen.width - 80, Screen.height - 30, 80, 30), sendLogsName))
        {
            sendLogsName = this.SendAllLogsToServer();
        }

        if (this.buttonStyle == null)
        {
            this.buttonStyle = GUI.skin.button;
            this.textStyle = GUI.skin.box;

            int fontSize = 0;
            this.buttonStyle.fontSize = fontSize;
            this.textStyle.fontSize = fontSize;

            textHeight = (this.textStyle.fontSize == 0 ? 25f : this.textStyle.fontSize*1.5f);
            textHeightWithMargin = textHeight + Margin;

            this.textStyle.alignment = TextAnchor.MiddleLeft;
        }

        if (
            GUI.RepeatButton(
                new Rect(
                    Screen.width*0.5f - this.ButtonWidth*0.5f,
                    this.buttonYPosition + 2*Margin,
                    this.ButtonWidth,
                    this.ButtonHeight),
                new GUIContent(this.ButtonText),
                this.buttonStyle))
        {
            if (!this.isDragging)
            {
#if UNITY_FLASH
                deltaY = buttonYPosition + Input.mousePosition.y;
#else
                if (Input.multiTouchEnabled && Input.touchCount == 1) // touch 
                {
                    this.deltaY = this.buttonYPosition + Input.GetTouch(0).position.y;
                }
                else if (Input.GetMouseButton(0)) //mouse
                {
                    this.deltaY = this.buttonYPosition + Input.mousePosition.y;
                }
#endif
            }

            this.isMaximizing = false;
            this.isMinimizing = false;
            this.isMaximized = false;
            this.isFixed = false;
            this.isDragging = true;
        }

        if (this.isMaximizing)
        {
            this.buttonYPosition -= this.AnimateSpeed;
            if (this.buttonYPosition < 0)
            {
                this.buttonYPosition = 0;
                this.isMaximized = true;
                this.isMaximizing = false;
            }
        }

        if (this.isMinimizing)
        {
            this.buttonYPosition += this.AnimateSpeed;
            if (this.buttonYPosition + this.ButtonHeight > Screen.height)
            {
                this.buttonYPosition = Screen.height - this.ButtonHeight;
                this.isMinimizing = false;
            }
        }

        if (this.isDragging)
        {
            this.buttonYPosition = -Input.mousePosition.y + this.deltaY;
            if (this.buttonYPosition < 0)
            {
                this.buttonYPosition = 0;
            }

            if (this.buttonYPosition + this.ButtonHeight > Screen.height)
            {
                this.buttonYPosition = Screen.height - this.ButtonHeight;
            }
        }

        this.windowRect.Set(this.windowRect.x, this.buttonYPosition, Screen.width, Screen.height - this.buttonYPosition);
        if (this.isFixed || this.isDragging || this.isMaximizing || this.isMinimizing || this.isMaximized)
        {
            GUI.Window(this.DebugWindowID, this.windowRect, this.OnDebugWindow, "");
        }
    }

    private void SaveAllLogs()
    {
        if (!Directory.Exists(@"C:\Logs\Slototherapy\"))
        {
            Directory.CreateDirectory(@"C:\Logs\Slototherapy\");
        }

        using (
            var file =
                new FileStream(
                    @"C:\Logs\Slototherapy\" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year +
                    "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".log", FileMode.Create))
        {
            using (var writer = new StreamWriter(file))
            {
                var res = PackLogs();

                writer.Write(res);
            }
        }
    }

    private static string PackLogs()
    {
        string res = "";
        foreach (var message in logList)
        {
            res += message.ToString();
            res += "\r\n";
        }
        return res;
    }

    public string SendAllLogsToServer()
    {
        var log = PackLogs();
        var rnd = UnityEngine.Random.Range(0, 10000000);
        var name = rnd.ToString("D6");
//        new SLogEnvelope(log, name).Send();

        return name;
    }

    private static float _totalTextHeight;

    private void OnDebugWindow(int windowId)
    {
        //return;
        //var textHeight = textStyle.fontSize == 0 ? 25f : textStyle.fontSize * 1.5f;
        const float windowHeaderHeight = 17f;

        float scrollViewHeight = this.windowRect.height - Margin*2 - windowHeaderHeight - this.ButtonHeight;

        if (scrollViewHeight > 0)
        {
            this.scrollViewVector =
                GUI.BeginScrollView(
                    new Rect(
                        Margin*2,
                        this.ButtonHeight + windowHeaderHeight + Margin,
                        this.windowRect.width - 6,
                        scrollViewHeight),
                    this.scrollViewVector,
                    new Rect(0, 0, this.windowRect.width - 22, _totalTextHeight + 100 + Margin));

            for (int i = 0; i < logListFiltered.Count; i++)
            {
                if (logListFiltered[i].Top > this.scrollViewVector.y + scrollViewHeight)
                {
                    break;
                }

                if (logListFiltered[i].Top + logListFiltered[i].Height > this.scrollViewVector.y)
                {
                    GUI.Box(
                        new Rect(
                            0,
                            logListFiltered[i].Top + Margin,
                            this.windowRect.width - 21 - Margin,
                            logListFiltered[i].Height - Margin),
                        logListFiltered[i].ToString(),
                        this.textStyle);
                }
            }

            GUI.EndScrollView();

            if (GUI.Button(
                new Rect(2*Margin, 2*Margin, this.ButtonWidth, this.ButtonHeight),
                new GUIContent("clear"),
                this.buttonStyle))
            {
                Clear();
            }

            if (GUI.Button(
                new Rect(3*Margin + this.ButtonWidth, 2*Margin, this.ButtonWidth, this.ButtonHeight),
                new GUIContent("console"),
                this.buttonStyle))
            {
                this.SwitchConsoleMode();
            }
        }
    }

#if UNITY_IPHONE || UNITY_ANDROID
    private TouchScreenKeyboard consoleKeyboard;
#endif

    private void SwitchConsoleMode()
    {
        if (!ConsoleModeOn)
        {
            _backupFilter = _filter;
            SetFilter(DebugType.Console);
#if UNITY_IPHONE || UNITY_ANDROID
            consoleKeyboard = TouchScreenKeyboard.Open("");
#endif
            ConsoleModeOn = true;
        }
        else
        {
            ConsoleModeOn = false;
            SetFilter(_backupFilter);
#if UNITY_IPHONE || UNITY_ANDROID
            consoleKeyboard = null;
#endif
        }
    }

    public static void AddConsoleRequest(string command)
    {
        ConsoleLog(command, false);
    }

    public static void AddConsoleResponse(string command)
    {
        ConsoleLog(command, true);
    }

    private static void ConsoleLog(string message, bool isResponse)
    {
        if (message == null)
        {
            return;
        }

        var msg = new ConsoleMessage(
            message,
            isResponse,
            (message.Split('\n').Length)*lineHeight + tableHeight,
            _totalTextHeight + Margin);

        if (WriteToDebugger)
        {
            logList.Add(msg);
            //logListIndices.Add(msg.GetHashCode(), logList.Count);
        }

        if ((_filter & DebugType.Console) != 0 || _filter == 0)
        {
            logListFiltered.Add(msg);
            //logListFilteredIndices.Add(msg.GetHashCode(), logList.Count);
            _totalTextHeight += msg.Height;
        }
    }

    #endregion

    private static DebugType _backupFilter;

    private static DebugType _filter;

    private static float lineHeight = 13f;

    private static float tableHeight = 15f;

    private static float textHeight = 25f;

    private static float textHeightWithMargin = 27f;

    public static void SetFilter(DebugType filter)
    {
        if (ConsoleModeOn)
        {
            _backupFilter = filter;
            return;
        }

        _filter = filter;

        //Update filtered list
        logListFiltered.Clear();
        logListFilteredIndices.Clear();
        _totalTextHeight = 0;

        foreach (Message message in logList)
        {
            if ((message.DebugType & filter) != 0)
            {
                logListFiltered.Add(message);
                if (!logListFilteredIndices.ContainsKey(message.GetHashCode()))
                {
                    logListFilteredIndices.Add(message.GetHashCode(), logListFiltered.Count);
                }

                _totalTextHeight += message.Height;
            }
        }
    }

    public static void Watch(int watchId, string msg, DebugType debugType = DebugType.Main, bool sendToServer = false)
    {
#if NOLOG
        return;
#endif
        var message = new WatchMessage(
            watchId,
            msg,
            debugType,
            (msg.Split('\n').Length)*lineHeight + tableHeight,
            _totalTextHeight + Margin);

        //        var index = logList.IndexOf(message);
        int index;
        logListIndices.TryGetValue(message.GetHashCode(), out index);
        //Normalize index
        index--;

        if (index != -1)
        {
            //Manual lock
            //Just "watch" here, so if anyone wants to write while it's locked - bad for him, he won't get a chance.
            bool lockAcquired = false;
            try
            {
                lockAcquired = Monitor.TryEnter(logList);
                if (lockAcquired)
                {
                    ((WatchMessage) logList[index]).UpdateMessage(msg);
                }
            }
            finally
            {
                if (lockAcquired)
                {
                    Monitor.Exit(logList);
                }
            }
        }
        else
        {
//            if (sendToServer || WriteDebugToServer)
//            {
//                new SDebugMessageEnvelope(
//                    string.Format(
//                        "D.Watch(). Message: {0}.\r\nWatchId: {1}.\r\nUTC time: {2}\r\n\r\n",
//                        msg,
//                        watchId,
//                        DateTime.UtcNow)).Send();
//            }

            lock (logList)
            {
                logList.Add(message);
            }
        }

        if ((_filter & debugType) != 0 || _filter == 0)
        {
            int index2;
            logListFilteredIndices.TryGetValue(message.GetHashCode(), out index2);
            //Normalize index
            index2--;

            if (index2 != -1)
            {
                //Manual lock (see above)
                bool lockAcquired = false;
                try
                {
                    lockAcquired = Monitor.TryEnter(logListFiltered);
                    if (lockAcquired)
                    {
                        ((WatchMessage) logListFiltered[index2]).UpdateMessage(msg);
                    }
                }
                finally
                {
                    if (lockAcquired)
                    {
                        Monitor.Exit(logListFiltered);
                    }
                }
            }
            else
            {
                lock (logListFiltered)
                {
                    logListFiltered.Add(message);
                    logListFilteredIndices.Add(message.GetHashCode(), logListFiltered.Count);
                    _totalTextHeight += message.Height;
                }
            }
        }
    }

    public static void Log(
        object message,
        DebugType debugType = DebugType.Main,
        bool writeToUnityConsole = false,
        bool sendToServer = false,
        String stackTrace = "")
    {
#if NOLOG
        return;
#endif
        if (message == null)
        {
            message = "Message sent to debugger is null";
        }

        _Log(message.ToString(), debugType);

        if (writeToUnityConsole || WriteDebugToConsole)
        {
            Debug.Log(message + stackTrace);
        }
    }

    private static void _Log(string message, DebugType debugType)
    {
        if (message == null)
        {
            return;
        }

        var msg = new Message(
            message,
            debugType,
            (message.Split('\n').Length)*lineHeight + tableHeight,
            _totalTextHeight + 2*Margin);

        if (WriteToDebugger)
        {
            int index;
            logListIndices.TryGetValue(message.GetHashCode(), out index);
            index--;

            if (index != -1)
            {
                lock (logList)
                {
                    logList[index].count++;
                }
            }
            else
            {
                lock (logList)
                {
                    logList.Add(msg);
                    logListIndices.Add(msg.GetHashCode(), logList.Count);
                }
            }
        }

        if ((_filter & debugType) != 0 || _filter == 0)
        {
            int index;
            logListFilteredIndices.TryGetValue(message.GetHashCode(), out index);
            index--;

            if (index == -1)
            {
                lock (logListFiltered)
                {
                    logListFiltered.Add(msg);
                    logListFilteredIndices.Add(msg.GetHashCode(), logListFiltered.Count);
                    _totalTextHeight += msg.Height;
                }
            }
        }
    }

    /// <summary>
    ///     Clear all messages.
    /// </summary>
    public static void Clear()
    {
#if NOLOG
        return;
#endif
        lock (logList)
        {
            lock (logListFiltered)
            {
                logListFiltered.Clear();
                logList.Clear();

                logListFilteredIndices.Clear();
                logListIndices.Clear();
                _totalTextHeight = 0;
            }
        }
    }
}

[Flags]
public enum DebugType
{
    Main = 0x01,

    UnityConsole = 0x02,

    Exception = 0x04,

    NetworkHandler = 0x10,

    NetworkInterface = 0x20,

    Console = 0x400,

    Threading = 0x4000,
}