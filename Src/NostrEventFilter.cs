using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NostrEventFilter
{
    public List<string> ids;
    public List<string> authors;
    public List<int> kinds;
    public List<string> e;
    public List<string> p;
    public int since = -1;
    public int until;
    public int limit = 1;


    public NostrEventFilter()
    {
        ids = new List<string>();
        authors = new List<string>();
        kinds = new List<int>();
        e = new List<string>();
        p = new List<string>();
    }

    public string ToJson()
    {
        string result = "{";
        if(ids.Count > 0)
        {
            result += "\"ids\":[";
            for(int i = 0; i < ids.Count; i++)
            {
                result += "\"" + ids[i] + "\"";

                if(i == ids.Count - 1)
                {
                    result += "]";
                }
                result += ",";
            }
        }
        if (authors.Count > 0)
        {
            result += "\"authors\":[";
            for (int i = 0; i < authors.Count; i++)
            {
                result += "\"" + authors[i] + "\"";

                if (i == authors.Count - 1)
                {
                    result += "]";
                }
                result += ",";
            }
        }
        if (kinds.Count > 0)
        {
            result += "\"kinds\":[";
            for (int i = 0; i < kinds.Count; i++)
            {
                result += kinds[i];

                if (i == kinds.Count - 1)
                {
                    result += "]";
                }
                result += ",";
            }
        }
        if (since != -1)
        {
            result += "\"since\":"+since+",";
        }
        result += "\"limit\":"+ limit + ",";



        if (result[result.Length - 1] != '}')
        {
            result = result.Remove(result.Length - 1, 1);
            result += "}";
        }
        return result;


    }
}
