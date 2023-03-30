using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

public class UserMetaData {
    public Dictionary<string, JsonData> metadata = new Dictionary<string, JsonData>();

    public event EventHandler<UserMetaEventArgs> ValueChanged;
    int MAX_SIZE = 100;
    public void NewMetaData(JsonData data)
    {
        string pub = data["pubkey"].ToString();
        if (metadata.ContainsKey(pub))
        {


            int o_time = int.Parse(metadata[pub]["created_at"].ToString());
            int n_time = int.Parse(data["created_at"].ToString());

            if (o_time > n_time) return;

            metadata.Remove(pub);
        }

        metadata.Add(pub, data);

        if (ValueChanged != null)
        {
            UserMetaEventArgs eventArgs = new UserMetaEventArgs(pub, data);
            ValueChanged(this, eventArgs);
        }
    }

    public JsonData GetMetaData(bool removeAfter = false)
    {
        return null;
    }

    public void OnValueChanged(Object sender, UserMetaEventArgs e)
    {
        EventHandler<UserMetaEventArgs> handler = ValueChanged;       
        if (null != handler) handler(this, e);
    }

}

public class UserMetaEventArgs : EventArgs
{
    public string pubkey { get; set; }
    public JsonData data { get; set; }

    public UserMetaEventArgs(string p, JsonData d)
    {
        pubkey = p;
        data = d;
    }
}
