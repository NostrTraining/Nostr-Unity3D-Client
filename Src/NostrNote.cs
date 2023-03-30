using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class NostrNote 
{
    public Dictionary<string, JsonData> note = new Dictionary<string, JsonData>();

    public event EventHandler<NoteEventArgs> ValueChanged;

    public void AddNewNote(JsonData data)
    {
        string id = data["id"].ToString();
        if (note.ContainsKey(id)) return;

        note.Add(id, data);
        
        if(note.Count > 30)
        {
           note.Remove(note.Keys.First());
        }

        if (ValueChanged != null)
        {
            NoteEventArgs eventArgs = new NoteEventArgs(id, data);
            ValueChanged(this, eventArgs);
        }
    }
   
    public void OnValueChanged(Object sender, NoteEventArgs e)
    {
        EventHandler<NoteEventArgs> handler = ValueChanged;
        if (null != handler) handler(this, e);
    }

}

public class NoteEventArgs : EventArgs
{
    public string id { get; set; }
    public JsonData data { get; set; }

    public NoteEventArgs(string p, JsonData d)
    {
        id = p;
        data = d;
    }
}
