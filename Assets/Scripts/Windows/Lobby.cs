using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;

public class Lobby : MonoBehaviour {

    private SmartFox sfs;
    private bool shuttingDown;
    //public GameObject gameListItem;
    public CanvasGroup chatControls;
    public Text chatText;

    public GameObject newsPrefab;
    public RectTransform newsCotent;
    public Text newsTitleText;
    public ScrollRectSnap newsScroll;

    private string[] newsTitles;
    private int currentNewsId = 0;

    public RectTransform FriendsContainer;
    public GameObject FriendItem;
    //public string addBuddyName;
    public InputField FriendInput;
    private List<FriendsListItem> friendsList = new List<FriendsListItem>();

    void Awake()
    {
        Application.runInBackground = true;

        if (SmartFoxConnection.IsInitialized)
        {
            sfs = SmartFoxConnection.Connection;
        }
        else
        {
            Application.LoadLevel("Login");
            return;
        }

        Debug.Log("Logged in as " + sfs.MySelf.Name);

        // Register event listeners
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
        sfs.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);

        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        // Populate list of available games
        populateGamesList();

        // Disable chat controls until the lobby Room is joined successfully
        chatControls.interactable = false;

        // Join the lobby Room (must exist in the Zone!)
        sfs.Send(new JoinRoomRequest("Lobby"));        
    }

    // Update is called once per frame
    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();

        if (currentNewsId != newsScroll.currentPage)
        {
            currentNewsId = newsScroll.currentPage;
            newsTitleText.text = newsTitles[currentNewsId];
        }
    }

    void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();
    }

    private void printSystemMessage(string message)
    {
        chatText.text += "<color=#00ff00>" + message + "</color>\n";
    }

    private void printUserMessage(User user, string message)
    {
        chatText.text += "<b>" + (user == sfs.MySelf ? "You" : user.Name) + ":</b> " + message + "\n";
    }

    private void populateGamesList()
    {
        // For the gamelist we use a scrollable area containing a separate prefab button for each Game Room
        // Buttons are clickable to join the games
        List<Room> rooms = sfs.RoomManager.GetRoomList();

        foreach (Room room in rooms)
        {
            // Show only game rooms
            // Also password protected Rooms are skipped, to make this example simpler
            // (protection would require an interface element to input the password)
            if (!room.IsGame || room.IsHidden || room.IsPasswordProtected)
            {
                continue;
            }

            Debug.Log("Rooms:" + room.Name + " roomId:" + room.Id);
        }
    }

    public void OnSendMessageButtonClick()
    {
        InputField msgField = (InputField)chatControls.GetComponentInChildren<InputField>();

        if (msgField.text != "")
        {
            // Send public message to Room
            sfs.Send(new Sfs2X.Requests.PublicMessageRequest(msgField.text));

            msgField.text = "";
        }
    }

    public void GetLobbyData()
    {
        SFSObject sfsobj = new SFSObject();
        sfs.Send(new ExtensionRequest("LobbyNews", sfsobj,sfs.LastJoinedRoom,false));
        GetFriendsList();
    }

    IEnumerator LoadNews(string[] urls,string[] titles)
    {
        newsTitles = new string[titles.Length];
        for (int i=0;i<urls.Length;i++)
        {
            Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
            WWW www = new WWW(urls[i]);
            yield return www;

            www.LoadImageIntoTexture(texture);
            GameObject newsGO = Instantiate(newsPrefab) as GameObject;
            newsGO.transform.SetParent(newsCotent);
            newsGO.transform.position = new Vector3(0, 0, 0);
            newsGO.transform.rotation = Quaternion.identity;
            newsGO.transform.localScale = new Vector3(1, 1, 1);
            newsGO.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            newsTitles[i] = titles[i];
        }
        newsTitleText.text = newsTitles[0];

        yield return null;
    }

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners
        reset();

        if (shuttingDown == true)
            return;

        // Return to login scene
        Application.LoadLevel("Login");
    }
    private void OnPublicMessage(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
        string message = (string)evt.Params["message"];

        printUserMessage(sender, message);
    }

    //----------------------------------------------------------
    // Room event listeners
    //----------------------------------------------------------

    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];

        // If we joined a Game Room, then we either created it (and auto joined) or manually selected a game to join
        if (room.IsGame)
        {
            // Remove SFS2X listeners
            reset();

            // Load game scene
            Application.LoadLevel("Game");
        }
        else
        {
            // Show system message
            printSystemMessage("\nYou joined a " + room.Name);

            // Enable chat controls
            chatControls.interactable = true;
            printSystemMessage("\n Welcom in Game QuadTD");
            Debug.Log(sfs.LastJoinedRoom.Name);
            GetLobbyData();
            SetPlayerOnline();
        }
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        // Show error message
        printSystemMessage("Room join failed: " + (string)evt.Params["errorMessage"]);
    }
    
    private void OnUserEnterRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        // Show system message
        printSystemMessage("User " + user.Name + " entered the Lobby");
        foreach(FriendsListItem friendItem in FriendsContainer.GetComponentsInChildren<FriendsListItem>())
        {
            if (friendItem.GetName() == user.Name) friendItem.SetData(user.Name, "True");
        }
        
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        if (user != sfs.MySelf)
        {
            // Show system message
            printSystemMessage("User " + user.Name + " left the Lobby");

            foreach (FriendsListItem friendItem in FriendsContainer.GetComponentsInChildren<FriendsListItem>())
            {
                if (friendItem.GetName() == user.Name) friendItem.SetData(user.Name, "False");
            }
        }
    }

    private void OnRoomAdded(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];

        // Update view (only if room is game)
        if (room.IsGame)
        {
            populateGamesList();
        }
    }

    public void OnRoomRemoved(BaseEvent evt)
    {
        // Update view
        populateGamesList();
    }
    
    public void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        if (cmd == "news")
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            StartCoroutine(LoadNews(responseParams.GetUtfStringArray("NewsLinks"), responseParams.GetUtfStringArray("NewsTitles")));
        }
        if (cmd == "FriendInfo")
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            Debug.Log(responseParams.GetUtfString("FriendInfo"));

        }
        if (cmd == "FriendsList")
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            ISFSArray friendsArray =  responseParams.GetSFSArray("FriendsList");
            BuildFriendList(friendsArray);
        }
        if (cmd == "FriendAdd")
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            bool addFriendStatus = responseParams.GetBool("AddFriendStatus");
            if (addFriendStatus)
            {
                GameObject itemGO = Instantiate(FriendItem,new Vector3(0,0,0),Quaternion.identity) as GameObject;
                itemGO.transform.SetParent(FriendsContainer);
                itemGO.transform.localScale = new Vector3(1, 1, 1);
                itemGO.GetComponent<FriendsListItem>().SetData(responseParams.GetUtfString("FriendName"), responseParams.GetBool("FriendStatus").ToString());
            }
        }
        if (cmd == "FriendRemove")
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            bool removeFriendStatus = responseParams.GetBool("RemoveFriendStatus");
            if (removeFriendStatus)
            {
                foreach (FriendsListItem friendItem in FriendsContainer.GetComponentsInChildren<FriendsListItem>())
                {
                    if (friendItem.GetName() == responseParams.GetUtfString("FriendName")) Destroy(friendItem.gameObject);
                }
            }
        }
    }

    //----------------------------------------------------------
    // Buddy event listeners
    //----------------------------------------------------------

    public void BuildFriendList(ISFSArray friendsArray)
    {
        
        /*Debug.Log("FriendsCount:"+friendsArray.Count);*/
        foreach (FriendsListItem item in FriendsContainer.GetComponentsInChildren<FriendsListItem>())
        {
            Destroy(item);
        }

        for(int i=0;i<friendsArray.Count;i++)
		{
			ISFSObject friendValues = friendsArray.GetSFSObject(i);
			string friendname = friendValues.GetUtfString("name");
            string friendstatus = (Convert.ToBoolean(friendValues.GetInt("isOnline"))).ToString();

            GameObject itemGO = Instantiate(FriendItem,new Vector3(0,0,0),Quaternion.identity) as GameObject;
            itemGO.transform.SetParent(FriendsContainer);
            itemGO.transform.localScale = new Vector3(1, 1, 1);
            itemGO.GetComponent<FriendsListItem>().SetData(friendname, friendstatus);
            //Debug.Log("Friend:" + friendname+" Status:"+friendstatus);
        }
    }

    private void onFriendsError(BaseEvent evt)
    {
        Debug.Log("FriendsError: " + (string)evt.Params["errorMessage"]);
    }

    public void GetFriendsList()
    {
        SFSObject sfsobj = new SFSObject();
        sfsobj.PutUtfString("FriendsEvent", "GetFriendsList");
        sfs.Send(new ExtensionRequest("FriendsManager", sfsobj));
    }
    public void AddFriend()
    {
        if (FriendInput.text != "")
        {
            SFSObject sfsobj = new SFSObject();
            sfsobj.PutUtfString("FriendsEvent", "AddFriend");
            sfsobj.PutUtfString("FriendName", FriendInput.text);

            sfs.Send(new ExtensionRequest("FriendsManager", sfsobj));
        }
    }
    public void RemoveFriend()
    {
        if (FriendInput.text != "")
        {
            Debug.Log("Remove:" + FriendInput.text);
            SFSObject sfsobj = new SFSObject();
            sfsobj.PutUtfString("FriendsEvent", "RemoveFriend");
            sfsobj.PutUtfString("FriendName", FriendInput.text);

            sfs.Send(new ExtensionRequest("FriendsManager", sfsobj));
        }
    }
    public void SetPlayerOnline()
    {
        Debug.Log("Set player online");
        SFSObject sfsobj = new SFSObject();
        sfsobj.PutUtfString("PlayerStatusEvent", "SetPlayerStatus");
        sfsobj.PutBool("PlayerStatus", true);

        sfs.Send(new ExtensionRequest("PlayerStatus", sfsobj));
    }

    public void ExitButton()
    {
        sfs.Disconnect();
        Application.Quit();
    }
    public void StartButton()
    {
        Application.LoadLevel(2);
    }
}
