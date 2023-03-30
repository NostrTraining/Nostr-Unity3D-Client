using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Login : MonoBehaviour
{


    public TextMeshProUGUI btn_generate_random_key;
    public TextMeshProUGUI btn_generate_pow_key;
    public TMP_InputField inputfield_pow;


    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
