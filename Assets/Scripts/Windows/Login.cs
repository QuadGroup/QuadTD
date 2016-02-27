using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;

public class Login : MonoBehaviour {

    [Tooltip("IP address or domain name of the SmartFoxServer 2X instance")]
    public string Host = "127.0.0.1";

    [Tooltip("TCP port listened by the SmartFoxServer 2X instance; used for regular socket connection in all builds except WebGL")]
    public int TcpPort = 9933;

    [Tooltip("WebSocket port listened by the SmartFoxServer 2X instance; used for in WebGL build only")]
    public int WSPort = 8888;

    //[Tooltip("Name of the SmartFoxServer 2X Zone to join")]
    //string Zone = "BasicExamples";

    private SmartFox sfs;

    public string serverName;
    public InputField loginInputText;
    public InputField passwordInputText;

    void Awake()
    {
        Application.runInBackground = true;

    #if UNITY_WEBPLAYER
		    if (!Security.PrefetchSocketPolicy(Host, TcpPort, 500)) {
			    Debug.LogError("Security Exception. Policy file loading failed!");
		    }
    #endif

    }

    // Update is called once per frame
    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }

    public void OnLoginButtonClick()
    {
        // Set connection parameters
        ConfigData cfg = new ConfigData();
        cfg.Host = Host;

        #if !UNITY_WEBGL
                cfg.Port = TcpPort;
        #else
		        cfg.Port = WSPort;
        #endif
                cfg.Zone = serverName;

                // Initialize SFS2X client and add listeners
        #if !UNITY_WEBGL
                sfs = new SmartFox();
        #else
		        sfs = new SmartFox(UseWebSocket.WS);
        #endif
        // Set ThreadSafeMode explicitly, or Windows Store builds will get a wrong default value (false)
        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        
        // Connect to SFS2X
        sfs.Connect(cfg);
    }

    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);

        sfs = null;
    }

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            SmartFoxConnection.Connection = sfs;

            // Login
            //sfs.Send(new Sfs2X.Requests.LoginRequest(nameInputText.text));
            sfs.Send(new Sfs2X.Requests.LoginRequest(loginInputText.text, passwordInputText.text, sfs.CurrentZone));
        }
        else
        {
            // Remove SFS2X listeners and re-enable interface
            reset();

            // Show error message
           Debug.Log("Connection failed; is the server running at all?");
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface
        reset();

        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
        {
            // Show error message
            Debug.Log("Connection was lost; reason is: " + reason);
        }
    }

    private void OnLogin(BaseEvent evt)
    {
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"])
        {
            string loginErrorMessage = (string)evt.Params["errorMessage"];
            Debug.Log("Login error: " + loginErrorMessage);
        }
        else
        {
            User user = (User)evt.Params["user"];

            // Show system message
            string msg = "Connection established successfully\n";
            msg += "SFS2X API version: " + sfs.Version + "\n";
            msg += "Connection mode is: " + sfs.ConnectionMode + "\n";
            msg += "Logged in as " + user.Name;
            Debug.Log(msg);

            // Remove SFS2X listeners and re-enable interface
            reset();

            // Load lobby scene
            Application.LoadLevel("Lobby");
        }
    }

    private void OnLoginError(BaseEvent evt)
    {
        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();

        // Show error message
        Debug.Log("Login failed: " + (string)evt.Params["errorMessage"]);
    }

}
