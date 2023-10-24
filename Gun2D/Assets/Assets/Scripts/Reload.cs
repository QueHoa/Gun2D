using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneHit;
public class Reload : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        MasterControl.Instance.HideBanner();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetReload()
    {
        MasterControl.Instance.ShowInterAd((success) =>
        {
            MasterControl.Instance.ShowBanner();
            gameObject.SetActive(false);
        });
    }
}
