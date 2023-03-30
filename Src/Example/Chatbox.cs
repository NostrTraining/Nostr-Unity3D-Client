using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chatbox : MonoBehaviour
{

    public GameObject prefab_message;
    public Transform content;

    public UnityEngine.UI.ScrollRect scrollRect;

    public TMPro.TMP_InputField inputfield;
    public string lastID = "";
    public enum ChatType
    {
        PublicChannel,
        GlobalChat,
        FriendChat
    }
    private void Update()
    {

        float y = Mathf.Max(0, scrollRect.normalizedPosition.y - Time.deltaTime * 0.1f);
        scrollRect.normalizedPosition = new Vector2(0, y);
        
    }
    public ChatType selectedChat = ChatType.GlobalChat;

   
    void Awake()
    {
        NostrEventFilter eventFilter = new NostrEventFilter();
        eventFilter.kinds.Add(1);
        eventFilter.limit = 5;

        string request = "[\"REQ\", \"GlobalChat\"," + eventFilter.ToJson() + "]";
        NostrClient.instance.note.ValueChanged += NoteEvent;
        StartCoroutine(NostrClient.instance.SendEvent(request));
    }

    /* EVENTI JOKA KÄSITTELEE UUDEN VASTAAN OTETUN NOTEN / MUOTOILEE VIESTIN UI:EEN PAREMMIN SOPIVAMMAKSI */
    public void NoteEvent(System.Object sender, NoteEventArgs e)
    {

        string s = e.data["content"].ToString();
        s = System.Text.RegularExpressions.Regex.Replace(s, @"[^\u0020-\u007F]+", string.Empty);

        if(string.IsNullOrWhiteSpace(s) || string.IsNullOrEmpty(s))
        {
            return;
        }

        if (lastID == e.id) return;
        lastID = e.id;
        GameObject message = Instantiate(prefab_message, content);
        string h = e.data["pubkey"].ToString();
        string n = Bech32.Encode("npub", KeyManager.StringToByteArray(e.data["pubkey"].ToString()));
        string u = "<b><u>" + n.Substring(0,15) + "</b></u>";
        
        if (NostrClient.instance.usersMetaData.metadata.ContainsKey(h)){
            LitJson.JsonData d = LitJson.JsonMapper.ToObject(NostrClient.instance.usersMetaData.metadata[h]["content"].ToString());
            string d_name = "";
            string d_username = "";

            ICollection<string> keys = d.Keys;
            if (keys.Contains("display_name"))
            {

                d_name = d["display_name"].ToString();
            }

            if (keys.Contains("name"))
            {

                d_username = d["name"].ToString();
            }

            if (d_name != string.Empty || d_username != string.Empty)
            {
                u = "<b><u>" + d_name + "@" + d_username + "</b></u>";
            }
            else
            {
                u = "<b><u>" + n.Substring(0, 15) + "</b></u>";
            }
        }
        else
        {
            NostrClient.instance.LoadUserData(h);
        }

        message.transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            Chat.instance.LoadMyProfile(n, h);
        });
        message.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = u;
        message.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = System.Text.RegularExpressions.Regex.Replace(s, @"[^\u0020-\u007F]+", string.Empty);

        if (content.childCount > 20)
        {
            Destroy(content.GetChild(0).gameObject);
        }
    }
}
