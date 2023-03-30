using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    public static Loading instance;
    public void Awake()
    {
        instance = this;
    }
    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
