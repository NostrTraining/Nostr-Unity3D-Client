using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using LitJson;

public class NostrClient : MonoBehaviour
{

    public string[] relays = { "wss://nostr.massmux.com", "wss://relay.snort.social", "wss://relay.nostr.band" };
    public Dictionary<string, NostrSocket> sockets = new Dictionary<string, NostrSocket>();
    public List<String> NostrReceived = new List<string>();

    public static NostrClient instance;
    public UserMetaData usersMetaData;
    public NostrNote note;
    
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        note = new NostrNote();
        usersMetaData = new UserMetaData();

        DontDestroyOnLoad(this.gameObject);
       
    }
    private void Update()
    {
        if (NostrReceived.Count <= 0) return;
        JsonData data = JsonMapper.ToObject(NostrReceived[0]);
        NostrReceived.RemoveAt(0);

        if(data[0].ToString() == "EVENT")
        {
            if (data[1].ToString() == "get_user_metadata")
            {

                usersMetaData.NewMetaData(data[2]);

          
            }
            else if(data[1].ToString() == "GlobalChat")
            {
                note.AddNewNote(data[2]);
            }
        }
        else
        {
            Debug.Log(data.ToJson());
        }


    }
    public void LoadUserData(string user_ID)
    {
        NostrEventFilter filter = new NostrEventFilter();
        filter.authors.Add(user_ID);
        filter.limit = 1;
        filter.kinds.Add(0);
        string package = ReturnReqEventString(filter);
        StartCoroutine(SendEvent(package));

    }

    string ReturnReqEventString(NostrEventFilter filter)
    {
        return "[\"REQ\",\"get_user_metadata\"," + filter.ToJson() + "]";
    }
    public void LoadAllRelays()
    {
        for(int i = 0; i < relays.Length; i++)
        {
            string url = relays[i];
            if (sockets.ContainsKey(url)) { continue; }
            
            NostrSocket socket = new NostrSocket(url);
            sockets.Add(url, socket);
        }
    }
    public void OnApplicationQuit()
    {
        sockets[relays[0]].Close();
        
    }
    public void SendNote(string content, string tags, int kind)
    {
        NostrEvent nEvent = new NostrEvent();
        nEvent.pubkey = KeyManager.hexpub.ToLower();

        DateTime currentTime = DateTime.UtcNow;
        nEvent.created_at = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        nEvent.kind = kind;
        nEvent.tags = tags;
        nEvent.content = content;

        string json = nEvent.ToRawJson();
        nEvent.id = KeyManager.GetStringFromHash(KeyManager.GenerateSha256String(json)).ToLower();
        Span<byte> buf = stackalloc byte[64];
        KeyManager.eCPrivKey.SignBIP340(KeyManager.StringToByteArray(nEvent.id)).WriteToSpan(buf);
        nEvent.sig = KeyManager.GetStringFromHash(buf.ToArray()).ToLower();


        string package = "[\"EVENT\"," + nEvent.ToJson() + "]";

        StartCoroutine(SendEvent(package));
    }
    public IEnumerator SendEvent(string eJson)
    {
        for (int i = 0; i < relays.Length; i++)
        {
            if (sockets.ContainsKey(relays[i]))
            {
                NostrSocket socket = sockets[relays[i]];   
                yield return new WaitUntil(() => { return (socket.client.ReadyState != WebSocketSharp.WebSocketState.Connecting && socket.client.ReadyState != WebSocketSharp.WebSocketState.New); });

                if (socket.client.ReadyState == WebSocketSharp.WebSocketState.Open)
                {
                    Thread broadcast = new Thread(() => socket.Broadcast(eJson));
                    broadcast.Start();
                }
                else
                {
                    Debug.Log(socket.client.ReadyState);
                }
            }
        }
    }

}
