using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneHit;

public class ReloadBomb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetReloadBomb()
    {
        MasterControl.Instance.ShowInterAd((success) =>
        {
            gameObject.SetActive(false);
        });
    }
}
