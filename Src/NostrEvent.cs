using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NostrEvent
{
    public string id;
    public string pubkey;
    public long created_at;
    public int kind;
    public string tags = "[]";
    public string content;
    public string sig;
    
    public string ToJson()
    {
        string result = "";
        result = "{\"id\":\"" + id + "\",\"pubkey\":\"" + pubkey + "\",\"created_at\":" + created_at + ",\"kind\":" + kind + ",\"tags\":" + tags + ",\"content\":\"" + content + "\",\"sig\":\"" + sig + "\"}";

        return result;
    }

    public string ToRawJson()
    {
        string result = "";

        result = "[0,\"" + pubkey + "\"," + created_at + "," + kind + "," + tags + ",\"" + content + "\"]";

        return result;
    }
    /*public string Req()
    {
        string result = "[\"" + Event + "\",\"" + Id + "\"," + Filter + "]";
        return result;
    }*/
}
