using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using OneHit;
public class GunController : MonoBehaviour
{
    public Image onHide;
    public Image offHide;
    private bool isHide = false;
    //private bool isInfor = false;
    private bool isBG = false;
    [HideInInspector]
    public int numGun;

    public ToggleGroup toggleGroup;
    public bool isSingle;
    public bool isBurst;
    public bool isAuto = true;
    public bool isShake;
    private bool isCloud = false;
    private bool isRain = false;
    private bool isSnow = false;
    [SerializeField]
    private Transform boxBG;
    [SerializeField]
    private Transform cloud;
    [SerializeField]
    private Transform rain;
    [SerializeField]
    private Transform snow;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private ShopGun shopGun;
    [SerializeField] 
    private GameObject loading;
    [SerializeField]
    private GameObject setting;
    [SerializeField]
    private GameObject infor;
    [SerializeField]
    private GameObject Cloudy;
    [SerializeField]
    private GameObject Raining;
    [SerializeField]
    private GameObject Snowing;    
    public GameObject[] gun;
    public RectTransform[] icon;
    private Vector3 gunScale;
   
    void Start()
    {
        Application.targetFrameRate = 60;                  
    }
    private void OnEnable()
    {       
        numGun = shopGun.numberGun;
        gun[numGun].SetActive(true);
        gunScale = gun[numGun].GetComponent<GunPlay>().oldScale;
        toggleGroup.allowSwitchOff = false;
        foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        for (int i = 0; i < 2; i++)
        {
            icon[i].transform.position = new Vector3(-4.5f, icon[i].transform.position.y, 0);
        }
        for (int i = 2; i < 7; i++)
        {
            icon[i].transform.position = new Vector3(4.5f, icon[i].transform.position.y, 0);
        }
        icon[7].transform.position = new Vector3(icon[7].transform.position.x, -5f, 0);
        Sequence sequence = DOTween.Sequence();
        for (int i = 2; i < 5; i++)
        {
            sequence.Join(icon[i].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack));
        }
        if (isBG)
        {
            sequence.Join(icon[5].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack));
        }
        sequence.Join(icon[6].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(icon[0].DOAnchorPosX(85, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(icon[1].DOAnchorPosX(85, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(icon[7].DOAnchorPosY(220, 0.5f).SetEase(Ease.OutBack));
        sequence.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }   
    public void SetHide()
    {
        Sequence sequence = DOTween.Sequence();

        if (!isHide)
        {
            onHide.enabled = true;
            offHide.enabled = false;
            for (int i = 2; i < 5; i++)
            {
                sequence.Join(icon[i].DOAnchorPosX(85, 0.3f));
            }
            if (isBG)
            {
                sequence.Join(icon[5].DOAnchorPosX(85, 0.3f));
            }
            sequence.Join(icon[6].DOAnchorPosX(85, 0.3f));
            sequence.Join(icon[0].DOAnchorPosX(-85, 0.3f));
            sequence.Join(icon[7].DOAnchorPosY(-80, 0.3f));
            sequence.Play();
            gun[numGun].transform.DOScale(gunScale * 1.2f, 0.7f).SetEase(Ease.OutBack);
        }
        else
        {
            onHide.enabled = false;
            offHide.enabled = true;
            for (int i = 2; i < 5; i++)
            {
                sequence.Join(icon[i].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack));
            }
            if (isBG)
            {
                sequence.Join(icon[5].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack));
            }
            sequence.Join(icon[6].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(icon[0].DOAnchorPosX(85, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(icon[7].DOAnchorPosY(220, 0.5f).SetEase(Ease.OutBack));
            sequence.Play();
            gun[numGun].transform.DOScale(gunScale, 0.5f);
        }
        isHide = !isHide;
    }      
    public void SetSetting()
    {
        setting.SetActive(true);
    }    
    public void SetBack()
    {
        MasterControl.Instance.ShowInterAd((success)  => {

            if (isCloud)
            {
                SetCloud();
            }
            if (isRain)
            {
                SetRain();
            }
            if (isSnow)
            {
                SetSnow();
            }
            if (isBG)
            {
                SetBackground();
            }
            StartCoroutine(Loading());
        });
        
    }
    IEnumerator Loading()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        shopGun.gameObject.SetActive(true);
        gun[numGun].SetActive(false);
        gameObject.SetActive(false);
    }
    public void Infor()
    {
        infor.SetActive(true);
    }
    public void SetBackground()
    {
        if (isBG)
        {
            icon[5].DOAnchorPosX(85, 0.3f);
        }
        else
        {
            icon[5].DOAnchorPosX(-85, 0.5f).SetEase(Ease.OutBack);
        }
        isBG = !isBG;
    }
    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {           
            foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }           
            foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn && toggle != toggleGroup.ActiveToggles().FirstOrDefault())
                {
                    toggle.isOn = false;
                }
            }

            foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }
    }
    public void Single(bool single)
    {
        isSingle = single;
    }
    public void Burst(bool burst)
    {
        isBurst = burst;
    }
    public void Auto(bool auto)
    {
        isAuto = auto;
    }
    public void Shake(bool shake)
    {
        isShake = shake;
    } 
    public void SetCloud()
    {
        isCloud = !isCloud;
        if (isCloud)
        {
            isRain = true;
            isSnow = true;
            if (isRain)
            {
                SetRain();
            }
            if (isSnow)
            {
                SetSnow();
            }
            GameObject effect = (GameObject)Instantiate(Cloudy, cloud.position, cloud.rotation);
            effect.transform.SetParent(boxBG, false);
        }
        else
        {
            Transform effect = boxBG.Find(Cloudy.name + "(Clone)");
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }
        }       
    }
    public void SetRain()
    {
        isRain = !isRain;
        if (isRain)
        {
            isCloud = true;
            isSnow = true;
            if (isCloud)
            {
                SetCloud();
            }
            if (isSnow)
            {
                SetSnow();
            }
            GameObject effect = (GameObject)Instantiate(Raining, rain.position, rain.rotation);
            effect.transform.SetParent(boxBG, false);
        }
        else
        {
            Transform effect = boxBG.Find(Raining.name + "(Clone)");
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }
        }        
    }
    public void SetSnow()
    {
        isSnow = !isSnow;
        if (isSnow)
        {
            isCloud = true;
            isRain = true;
            if (isCloud)
            {
                SetCloud();
            }
            if (isRain)
            {
                SetRain();
            }
            GameObject effect = (GameObject)Instantiate(Snowing, snow.position, snow.rotation);
            effect.transform.SetParent(boxBG, false);
        }
        else
        {
            Transform effect = boxBG.Find(Snowing.name + "(Clone)");
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }
        }       
    }
}
