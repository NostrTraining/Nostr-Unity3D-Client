using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NBitcoin.Secp256k1;
using System.Linq;
using System.Threading;
public static class KeyManager
{


    public static ECPrivKey eCPrivKey;
    public static string npub;
    public static string hexpub;
    public static string nsec;

    public static bool POW = false;

    public static void GenerateNewRandomPrivateKey()
    {

        byte[] hash = GenerateSha256String(GenerateByLen(10));
        eCPrivKey = ECPrivKey.Create(hash);
        byte[] pubkey = eCPrivKey.CreateXOnlyPubKey().ToBytes();
        hexpub = GetStringFromHash(pubkey);
        npub = Bech32.Encode("npub",pubkey);
        Debug.Log(npub);

        nsec = Bech32.Encode("nsec", hash);

        

    }

    public static void LoadPrivateKey(byte[] key)
    {
        eCPrivKey = ECPrivKey.Create(key);
        byte[] pubkey = eCPrivKey.CreateXOnlyPubKey().ToBytes();
        hexpub = GetStringFromHash(pubkey);
        npub = Bech32.Encode("npub", pubkey);
        Debug.Log(npub);

        nsec = Bech32.Encode("nsec", key);
    }

    public static void  PowPrivateKey(string pow)
    {
        Debug.Log("MINING POW KEY");
        while (true)
        {
            
            byte[] hash = GenerateSha256String(GenerateByLen(10));
            ECPrivKey t_eCPrivKey = ECPrivKey.Create(hash);
            string t_npub = Bech32.Encode("npub", t_eCPrivKey.CreateXOnlyPubKey().ToBytes());

            if (t_npub.Substring(t_npub.Length - pow.Length) == pow)
            {
                eCPrivKey = t_eCPrivKey;
                byte[] pubkey = t_eCPrivKey.CreateXOnlyPubKey().ToBytes();
                hexpub = GetStringFromHash(pubkey);
                npub = t_npub;

                Debug.Log("Public pow key found: " + npub);
                nsec = Bech32.Encode("nsec", hash);
                break;
            }

        }

        POW = false;
    }

    public static string GetStringFromHash(byte[] hash)
    {
        return string.Join(string.Empty, hash.Select(x => x.ToString("X2")).ToArray());
    }

    private static readonly System.Random Random = new System.Random();

    public static string GenerateByLen(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static byte[] GenerateSha256String(string inputString)
    {
        var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(inputString);
        return sha256.ComputeHash(bytes);

    }
    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    public static void Sign(string hash)
    {
        
    }

}
