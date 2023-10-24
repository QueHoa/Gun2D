using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
    public static MessagePanel Instance;
    private void Start()
    {
        Instance = this;
    }
    public GameObject popup;
    public void Show()
    {
        popup.SetActive(true);
    }
}
