using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] UIgame;
    public GameObject uiMode;
    public GameObject uiMainLobby;
    public GameObject loading;
    public GameObject setting;
    public FlashlightPlugin flash;
    [SerializeField]
    private GameObject storm;
    [SerializeField]
    private Image onHaptic;
    [SerializeField]
    private Image offHaptic;
    [SerializeField]
    private Image onEffect;
    [SerializeField]
    private Image offEffect;
    [SerializeField]
    private Image onFlash;
    [SerializeField]
    private Image offFlash;
    [HideInInspector]
    public bool isHaptic = true;
    [HideInInspector]
    public bool isEffect = true;
    [HideInInspector]
    public bool isFlash = true;
    private int efect = 1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }
    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (uiMode.activeInHierarchy && efect != 0)
        {
            GameObject effect = Instantiate(storm, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform, false);
            efect = 0;
        }
        if(!uiMode.activeInHierarchy)
        {
            Transform effect = transform.Find(storm.name + "(Clone)");
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }
        }
    }
    public void SetShopSaber()
    {
        StartCoroutine(LoadingGun());   
    }
    public void SetShopGun()
    {
        StartCoroutine(LoadingBomb());        
    }
    public void SetShopBomb()
    {
        StartCoroutine(LoadingSaber());        
    }
    IEnumerator LoadingGun()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        UIgame[0].SetActive(true);
        uiMode.SetActive(false);
        efect = 1;
        uiMainLobby.SetActive(false);
    }
    IEnumerator LoadingBomb()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        UIgame[1].SetActive(true);
        uiMode.SetActive(false);
        efect = 1;
        uiMainLobby.SetActive(false);
    }
    IEnumerator LoadingSaber()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        UIgame[2].SetActive(true);
        uiMode.SetActive(false);
        efect = 1;
        uiMainLobby.SetActive(false);
    }
    public void SetSetting()
    {
        setting.SetActive(true);
    }
    public void SetEffect()
    {
        if (isEffect)
        {
            onEffect.enabled = false;
            offEffect.enabled = true;
        }
        else
        {
            onEffect.enabled = true;
            offEffect.enabled = false;
        }
        isEffect = !isEffect;
    }
    public void SetHaptic()
    {
        if (isHaptic)
        {
            onHaptic.enabled = false;
            offHaptic.enabled = true;
        }
        else
        {
            onHaptic.enabled = true;
            offHaptic.enabled = false;
        }
        isHaptic = !isHaptic;
    }
    public void SetFlash()
    {
        if (isFlash)
        {
            onFlash.enabled = false;
            offFlash.enabled = true;
        }
        else
        {
            onFlash.enabled = true;
            offFlash.enabled = false;
        }
        isFlash = !isFlash;
    }
}
