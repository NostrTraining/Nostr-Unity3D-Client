# Nostr-Unity3D-Client

What is Nostr?
[github.com/nostr-protocol/nostr](https://github.com/nostr-protocol/nostr)

This project is free to use, modify or do anything with. However, I do not recommend it as, for example, the keys are stored locally in plain text.

The purpose was to learn how the Nostr protocol works and better understand digital signing.

Here is an example of using it to send a public Nostr message with Nostr-Unity3D-Client (Note kind 1).

1. KeyManager.GenerateNewRandomPrivateKey();
2. NostrClient.instance.SendNote("public message", tags="[]", 1);

Example subscription for public notes.

1. --||--
2. NostrEventFilter filter = new NostrEventFilter();
3. filter.kinds.Add(1);
4. string content = NostrClient.instance.ReturnReqEventString(filter);
5. StartCoroutine(NostrClient.instance.SendEvent(content));
//Message receive EventHandler
//NostrClient.instance.note.ValueChange

-Simple
[Nostr Profile ( snort client link )](https://snort.social/p/npub1hk7y7fnnfl9sph0h9xezyvc43q6kk5q6ccxmlc2zdv93ndw8kauslmeqea)