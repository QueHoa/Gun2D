using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.NiceVibrations;
using DG.Tweening;

public class GunPlay : MonoBehaviour
{
    [HideInInspector]
    public bool isTouching = false;
    [HideInInspector]
    public bool isReload = false;
           
    [SerializeField]
    private int numBurst;
    [SerializeField]
    private GameObject shootEffect;   
    [SerializeField]
    private Transform shootTran;    
    [SerializeField]
    private float shootCooldown;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private int chamber = 0;
    [SerializeField]
    private FlashlightPlugin flash;
    [SerializeField]
    private GameObject flashImage;
    public HapticTypes hapticTypes = HapticTypes.HeavyImpact;
    private bool hapticsAllowed = true;
    public float shakeThreshold = 2.0f;
    private bool shakeDetected = false;
    [SerializeField]
    private Text TextBullet;
    [SerializeField]
    private GameObject noBullet;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GunController gunController;
    [SerializeField]
    private Animator anim;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    [SerializeField]
    private Text uiName;
    public string nameInfo;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Text uiText; 
    public string fileName = "hehe.txt";
    private AudioSource audioSource;
    private int numBullet;
    [HideInInspector]
    public Vector3 oldScale;
    private float defaultTime;
    private float cooldownTimer = Mathf.Infinity;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        MMVibrationManager.SetHapticsActive(hapticsAllowed);       
    }
    private void OnEnable()
    {
        oldScale = transform.localScale;
        TextAsset content = Resources.Load<TextAsset>(fileName);
        uiText.text = content.text;
        uiName.text = nameInfo;
        image.sprite = sprite;
        audioSource = gameObject.GetComponent<AudioSource>();
        numBullet = chamber;
        defaultTime = shootCooldown;
        audioSource.clip = null;
        transform.localScale = new Vector3(oldScale.x * 0.8f, oldScale.y * 0.8f, oldScale.z * 0.8f);
        transform.DOScale(oldScale, 0.4f).SetEase(Ease.OutBack);
    }
    // Update is called once per frame
    void Update()
    {
        if (!gunController.isShake)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }
                if (gunController.isAuto)
                {
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began)
                    {
                        isTouching = true;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isTouching = false;
                    }
                }
                if (gunController.isSingle)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        isTouching = true;
                    }
                    else
                    {
                        isTouching = false;
                    }
                }
                if (gunController.isBurst)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        isTouching = true;
                    }
                    else
                    {
                        isTouching = false;
                    }
                }
            }
        }else
        {
            if (Input.accelerationEventCount > 0)
            {
                Vector3 acceleration = Input.acceleration;
                float totalAcceleration = acceleration.magnitude;
                if (totalAcceleration > shakeThreshold)
                {
                    shakeDetected = true;
                }
            }
            if (shakeDetected)
            {
                if (cooldownTimer >= shootCooldown && numBullet != 0)
                {
                    StartCoroutine(Shoot());
                }
                shakeDetected = false;
            }
        }
        if (!gunController.isBurst)
        {
            if (isTouching && cooldownTimer >= shootCooldown && numBullet != 0)
            {
                StartCoroutine(Shoot());
            }
        }
        else
        {
            if (isTouching && cooldownTimer >= shootCooldown && numBullet != 0)
            {
                StartCoroutine(ShootBurst());
            }
        }
        TextBullet.text ="x" + numBullet.ToString();
        
        if (!noBullet.activeInHierarchy && shootCooldown == Mathf.Infinity && isReload)
        {
            StartCoroutine(Reload());
        }
        cooldownTimer += Time.deltaTime;
    }
    IEnumerator Shoot()
    {
        cooldownTimer = 0;
        audioSource.clip = shootClip;
        audioSource.Play();
        if (gameManager.isEffect)
        {
            GameObject effect = (GameObject)Instantiate(shootEffect, shootTran.position, shootTran.rotation);
            Destroy(effect, defaultTime);
        }
        if (gameManager.isHaptic)
        {
            MMVibrationManager.Haptic(hapticTypes, true, true, this);
        }
        if (gameManager.isFlash)
        {
            flash.TurnOn();
        }
        flashImage.SetActive(true);                          
        anim.SetTrigger("shoot");
        numBullet--;       
        yield return new WaitForSeconds(0.05f);
        flashImage.SetActive(false);
        if (gameManager.isFlash)
        {
            flash.TurnOff();
        }
        if (numBullet == 0 && !isReload)
        {
            StartCoroutine(NoBullet());
        }
    }
    IEnumerator ShootBurst()
    {
        for (int i = 0; i < 5; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = false;
        }
        for (int i = 8; i < 11; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = false;
        }
        for (int i = 0; i < numBurst; i++)
        {
            StartCoroutine(Shoot());
            yield return new WaitForSeconds(shootCooldown);
            if (numBullet == 0)
            {
                break;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = true;
        }
        for (int i = 8; i < 11; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = true;
        }
    }    
    IEnumerator Reload()
    {
        shootCooldown = defaultTime;
        audioSource.clip = reloadClip;
        audioSource.Play();
        yield return new WaitForSeconds(reloadTime);
        isReload = false;
        numBullet = chamber;
        yield return 0;
    }
    IEnumerator NoBullet()
    {
        for (int i = 0; i < 5; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = false;
        }
        for (int i = 8; i < 11; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = false;
        }
        shootCooldown = Mathf.Infinity;
        yield return new WaitForSeconds(defaultTime);
        for (int i = 0; i < 5; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = true;
        }
        for (int i = 8; i < 11; i++)
        {
            gunController.icon[i].GetComponent<Button>().interactable = true;
        }
        noBullet.SetActive(true);
        isReload = true;

        yield return 0;
    }
}
