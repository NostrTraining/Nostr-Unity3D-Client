using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;

public class ProfileWindow : MonoBehaviour
{


    public static ProfileWindow instance;
    public TextMeshProUGUI txt_npub;

    public TextMeshProUGUI nameANDusername;
    public TextMeshProUGUI nip5;
    public TextMeshProUGUI description;
    public Image profilepicture;

    public string profilePictureURL;

    public Button EditAndSaveBTN;
    public Image editANDSaveImg;

    public Sprite editSprite;
    public Sprite saveSprite;

    public TMP_InputField i_name;
    public TMP_InputField i_username;
    public TMP_InputField i_nip5;
    public TMP_InputField i_description;
    public TMP_InputField i_imageURL;


    public string _npub;
    public string _hpub;

    public Color noNameColor;
    public Color NameColor;

    public Sprite temp_sprite_profilepic;
    public GameObject editBTN;

    private void Awake()
    {
        instance = this;
        NostrClient.instance.usersMetaData.ValueChanged += MetaDataEvent;
    }

    public void OpenInBrowser()
    {
        Application.OpenURL("https://snort.social/p/" + _npub);
    }
    public void MetaDataEvent(System.Object sender, UserMetaEventArgs e)
    {
        LitJson.JsonData data = LitJson.JsonMapper.ToObject(e.data["content"].ToString());
        if (e.pubkey == _hpub)
        {
            ICollection<string> keys = data.Keys;
            #region Username
            string name = string.Empty;
            string username = string.Empty;
            if (keys.Contains("display_name"))
            {

                name = data["display_name"].ToString();
                i_username.text = data["display_name"].ToString();
            }

            if (keys.Contains("name"))
            {

                username = data["name"].ToString();
                i_name.text = data["name"].ToString();
            }

            if (name != string.Empty || username != string.Empty)
            {
                nameANDusername.color = NameColor;
                nameANDusername.text = name + "@" + username;
            }
            #endregion
            #region Description
            if (keys.Contains("about"))
            {
                description.text = data["about"].ToString();
                i_description.text = data["about"].ToString();
            }
            else
            {
                description.text = "";
            }
            #endregion
            #region ProfilePic
            if (keys.Contains("picture"))
            {
                i_imageURL.text = data["picture"].ToString();
                StopAllCoroutines();
                if(data["picture"].ToString() == string.Empty) { profilepicture.sprite = temp_sprite_profilepic;  return; }
                StartCoroutine(DownloadImage(data["picture"].ToString()));
            }
            else
            {
                profilepicture.sprite = temp_sprite_profilepic;
            }
            #endregion

        }

    }

    public IEnumerator DownloadImage(string MediaUrl)
    {
        Uri uri = new Uri(MediaUrl);
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

       
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
        {

            Debug.Log(request.error);
            Debug.Log(request.downloadHandler.error);

        }       
        else {

            string[] splitParams = MediaUrl.Split('?');
            string[] split = splitParams[0].Split('.');
            if (split[split.Length - 1] == "gif")
            {
                profilepicture.sprite = temp_sprite_profilepic;
                StartCoroutine(UniGif.GetTextureListCoroutine(request.downloadHandler.data, (gifTexList, loopCount, width, height) => { ImageListToSpriteArray(gifTexList); }));
            }
            else
            {
               // System.IO.MemoryStream imageStream = new System.IO.MemoryStream(request.downloadHandler.data);
                
                Texture2D texture = new Texture2D(1,1);
                ImageConversion.LoadImage(texture, request.downloadHandler.data);
                if(texture.width < 32)
                {
                    profilepicture.sprite = temp_sprite_profilepic;
                }
                else
                    profilepicture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f,.5f));

            }

        }
    }

    public void SetNPUB(string npub, string hpub)
    {
        if(npub == KeyManager.npub)
        {
            EditAndSaveBTN.gameObject.SetActive(true);
            editANDSaveImg.sprite = editSprite;

            SetEditCommand();
        }
        else
        {
            editBTN.SetActive(false);
        }

        txt_npub.text = npub;
        _npub = npub;
        _hpub = hpub;

        nameANDusername.color = noNameColor;
        nameANDusername.text = "I dont even have a name";
        description.text = "";
        profilepicture.sprite = temp_sprite_profilepic;

        if (NostrClient.instance.usersMetaData.metadata.ContainsKey(_hpub))
        {
            MetaDataEvent(this, new UserMetaEventArgs(_hpub, NostrClient.instance.usersMetaData.metadata[_hpub]));
        }
    }

    void SetEditCommand()
    {
        EditAndSaveBTN.onClick.RemoveAllListeners();
        EditAndSaveBTN.onClick.AddListener(() =>
        {
            SetSaveCommand();
            editANDSaveImg.sprite = saveSprite;
            EditSaveSetActive(true);
        });
    }

    void EditSaveSetActive(bool edit)
    {
        i_name.gameObject.SetActive(edit);
        i_username.gameObject.SetActive(edit);
        i_nip5.gameObject.SetActive(edit);
        i_description.gameObject.SetActive(edit);
        i_imageURL.gameObject.SetActive(edit);


        nameANDusername.gameObject.SetActive(!edit);
        nip5.gameObject.SetActive(!edit);
        description.gameObject.SetActive(!edit);
    }
    void SetSaveCommand()
    {
        EditAndSaveBTN.onClick.RemoveAllListeners();
        EditAndSaveBTN.onClick.AddListener(() =>
        {
            SetEditCommand();

            string content = "{\\\"name\\\":\\\"" + i_name.text + "\\\", \\\"display_name\\\":\\\""+i_username.text+"\\\",\\\"picture\\\":\\\""+i_imageURL.text+"\\\",\\\"about\\\":\\\""+i_description.text+"\\\"}";
            string tags = "[]";
            NostrClient.instance.usersMetaData.metadata.Remove(_hpub);
            NostrClient.instance.SendNote(content, tags, 0);

            editANDSaveImg.sprite = editSprite;
            EditSaveSetActive(false);

            
        });
    }
    private void ImageListToSpriteArray(List<UniGif.GifTexture> t)
    {
        Sprite[] sprites = new Sprite[t.Count];
        for(int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = Sprite.Create(t[i].m_texture2d, new Rect(0, 0, t[i].m_texture2d.width, t[i].m_texture2d.height), new Vector2(.5f, .5f));
        }

        StartCoroutine(LoopProfilePics(t[0].m_delaySec, sprites));
    }
    private IEnumerator LoopProfilePics(float t, Sprite[] p)
    {

        int i = 0;
        t = t >= 0.01f ? t : 0.01f;

        while (true)
        {
            profilepicture.sprite = p[i];
            i++;
            if(i >= p.Length) { i = 0; }
            yield return new WaitForSeconds(t);
        }

    }
    public void Kill()
    {
        NostrClient.instance.usersMetaData.ValueChanged -= MetaDataEvent;
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

}
