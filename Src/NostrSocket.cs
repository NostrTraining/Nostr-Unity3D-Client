using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using WebSocketSharp;
public class NostrSocket
{

    string url;
    public WebSocket client;

    CancellationToken cancellationToken;

    public Thread listenThread;

    
    public NostrSocket(string url)
    {
        this.url = url;
        this.client = new WebSocket(url);
        this.cancellationToken = new CancellationToken();

        client.OnMessage += (sender, e) =>
        {
            NostrClient.instance.NostrReceived.Add(e.Data);
        };

        client.OnClose += (sender, e) => { };

        client.Connect();

    }

  

    public async void Broadcast(string json)
    {
        while (client.ReadyState != WebSocketState.Open) {
            if(client.ReadyState == WebSocketState.Closed) { Thread.CurrentThread.Abort(); }
            if(client.ReadyState == WebSocketState.Closing) { Thread.CurrentThread.Abort();  }
        }
        client.Send(json);
    }

    public void Close()
    {
        client.Close();  
        
    }

}
