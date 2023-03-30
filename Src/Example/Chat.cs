using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject chatWindow;
    public TextMeshProUGUI npub; //REMOVE
    public TMP_InputField input_field; //REMOVE

    public GameObject prefab_login;
    public GameObject prefab_loading_big;

    public GameObject prefab_profileWindow;

    public GameObject prefab_chatBox;

    public static Chat instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(chatWindow);

        string[] keys = GetKeys();
        if(keys != null)
        {
            byte[] raw_key;
            string prefix;
            Bech32.Decode(keys[0], out prefix, out raw_key);
            KeyManager.LoadPrivateKey(raw_key);
            LoadMyProfile();
            OpenChat();
        }
        else
        {
            GameObject obj_login = Instantiate(prefab_login, chatWindow.transform);
            Login script_login = obj_login.GetComponent<Login>();

            script_login.btn_generate_random_key.GetComponent<Button>().onClick.AddListener(() =>
            {

                KeyManager.GenerateNewRandomPrivateKey();
                FileManager.SaveKey(Application.persistentDataPath + "/storage/nsec.key", new string[] { KeyManager.nsec });
                script_login.Kill();
                LoadMyProfile();
                OpenChat();

            });
            script_login.btn_generate_pow_key.GetComponent<Button>().onClick.AddListener(() =>
            {
                string pow = script_login.inputfield_pow.text;
                if(pow == string.Empty) { return; }
                pow = pow.ToLower();
                script_login.Kill();
                Instantiate(prefab_loading_big, chatWindow.transform);

                KeyManager.POW = true;
                StartCoroutine(GenerateNewPowKey(pow));
            });
        }
      
    }

    public void LoadMyProfile(string n = null, string h = null)
    {
        if(n == null) { n = KeyManager.npub; }
        if(h == null) { h = KeyManager.hexpub.ToLower(); }
        NostrClient.instance.LoadAllRelays();
        NostrClient.instance.LoadUserData(h);
        if (ProfileWindow.instance != null)
        {
            ProfileWindow.instance.SetNPUB(n, h);
        }
        else
        {
            GameObject obj = Instantiate(prefab_profileWindow, chatWindow.transform);
            obj.GetComponent<ProfileWindow>().SetNPUB(n, h);
        }

       
        
    }

    public void OpenChat()
    {
        GameObject chatBox = Instantiate(prefab_chatBox, chatWindow.transform);
    }
    
    public IEnumerator GenerateNewPowKey(string pow)
    {
        //
        System.Threading.Thread powT = new System.Threading.Thread(() => KeyManager.PowPrivateKey(pow));
        powT.Start();
        yield return new WaitUntil(() => { return KeyManager.POW == false; });
        FileManager.SaveKey(Application.persistentDataPath + "/storage/nsec.key", new string[] { KeyManager.nsec });
        Loading.instance.Kill();
        LoadMyProfile();
        OpenChat();

    }

    string[] GetKeys()
    {
        if (System.IO.Directory.Exists(Application.persistentDataPath + "/storage"))
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/storage/nsec.key"))
            {
                string[] keys = FileManager.LoadKeys(Application.persistentDataPath + "/storage/nsec.key");
                if (keys.Length > 0) return keys;
            }
        }
        else
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/storage");
        }
        return null;
    }


    public void SendToChannel()
    {
        string content = input_field.text;
        if (content == string.Empty) return;

        NostrClient.instance.sockets.Add(NostrClient.instance.relays[0], new NostrSocket(NostrClient.instance.relays[0]));
        string tags = "[[\"e\",\"" + "32bdfc9532d959c05d1d8541f0c6eaad75a62c8cbc6fea5ab33d1a6593b1e729" + "\",\"wss://nostr.massmux.com\",\"root\"]]";
        NostrClient.instance.SendNote(content, tags, 42);
    }

    public void Send()
    {

        string content = input_field.text;
        if (content == string.Empty) return;

        NostrClient.instance.sockets.Add(NostrClient.instance.relays[0], new NostrSocket(NostrClient.instance.relays[0]));
        string tags = "[]";
        NostrClient.instance.SendNote(content, tags, 1);

    }

}
