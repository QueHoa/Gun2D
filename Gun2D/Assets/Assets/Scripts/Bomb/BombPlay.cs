using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.NiceVibrations;
using DG.Tweening;

public class BombPlay : MonoBehaviour
{    
    [HideInInspector]
    public bool isReload = false;
    [HideInInspector]
    public bool s3 = false;
    [HideInInspector]
    public bool s5 = false;
    [HideInInspector]
    public bool s10 = false;

    [SerializeField]
    private GameObject bombEffect;
    [SerializeField]
    private GameObject buttonTime;
    [SerializeField]
    private float shootCooldown;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private FlashlightPlugin flash;
    [SerializeField]
    private GameObject flashImage;
    public HapticTypes hapticTypes = HapticTypes.HeavyImpact;
    private bool hapticsAllowed = true;
    [SerializeField]
    private GameObject noBullet;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private BombController bombController;
    public AudioClip countdownClip;
    public AudioClip activeClip;
    public AudioClip explodeClip;
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
    public string fileName = "hehe";
    private AudioSource audioSource;    
    private int numBullet;
    [HideInInspector]
    public Vector3 oldScale;
    private float cooldownTimer;
    private Tweener tweener;
    // Start is called before the first frame update
    void Start()
    {
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
        numBullet = 1;
        transform.localScale = new Vector3(oldScale.x * 0.8f, oldScale.y * 0.8f, oldScale.z * 0.8f);
        transform.DOScale(oldScale, 0.4f).SetEase(Ease.OutBack);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }          
        }
        if (s3)
        {
            cooldownTimer = 3;
            buttonTime.SetActive(false);
            StartCoroutine(Explode());
            s3 = false;
        }
        if (s5)
        {
            cooldownTimer = 5;
            buttonTime.SetActive(false);
            StartCoroutine(Explode());
            s5 = false;
        }
        if (s10)
        {
            cooldownTimer = 10;
            buttonTime.SetActive(false);
            StartCoroutine(Explode());
            s10 = false;
        }        
        if (!noBullet.activeInHierarchy && isReload)
        {
            StartCoroutine(Reload());
        }
    }
    IEnumerator Explode()
    {
        for (int i = 0; i < 5; i++)
        {
            bombController.icon[i].GetComponent<Button>().interactable = false;
        }
        for (int i = 7; i < 10; i++)
        {
            bombController.icon[i].GetComponent<Button>().interactable = false;
        }
        tweener = transform.DOScale(oldScale * 0.9f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        if (cooldownTimer > 2)
        {
            audioSource.clip = countdownClip;
            audioSource.loop = true;
            audioSource.Play();
            yield return new WaitForSeconds(cooldownTimer - 2);
        }
        audioSource.clip = activeClip;
        audioSource.loop = true;
        audioSource.Play();
        yield return new WaitForSeconds(2);
        audioSource.clip = explodeClip;
        audioSource.loop = false;
        audioSource.Play();
        tweener.Kill();
        transform.localScale = oldScale;
        if (gameManager.isEffect)
        {
            GameObject effect = (GameObject)Instantiate(bombEffect, Vector3.zero, Quaternion.Euler(Vector3.zero));
            Destroy(effect, shootCooldown);
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
    IEnumerator Reload()
    {
        audioSource.clip = reloadClip;
        audioSource.Play();
        buttonTime.SetActive(true);
        yield return new WaitForSeconds(reloadTime);
        isReload = false;
        numBullet = 1;
        yield return 0;
    }
    IEnumerator NoBullet()
    {
        yield return new WaitForSeconds(shootCooldown);
        for (int i = 0; i < 5; i++)
        {
            bombController.icon[i].GetComponent<Button>().interactable = true;
        }
        for (int i = 7; i < 10; i++)
        {
            bombController.icon[i].GetComponent<Button>().interactable = true;
        }
        noBullet.SetActive(true);
        isReload = true;
        yield return 0;
    }
}
