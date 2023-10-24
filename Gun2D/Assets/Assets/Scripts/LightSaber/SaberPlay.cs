using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.NiceVibrations;
using DG.Tweening;
public class SaberPlay : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private SaberController saberController;
    [SerializeField]
    private GameObject[] blade;
    [HideInInspector]
    public bool isTouching;
    [HideInInspector]
    public bool isEnergy = false;

    public SpriteRenderer[] colorSaber;
    public ParticleSystem[] vfx;
    public Slider colorSlider;
    [SerializeField]
    private float r;
    [SerializeField]
    private float g;
    [SerializeField]
    private float b;

    [SerializeField]
    private FlashlightPlugin flash;
    [SerializeField]
    private GameObject flashImage;
    public HapticTypes hapticTypes = HapticTypes.HeavyImpact;
    private bool hapticsAllowed = true;
    public float shakeThreshold = 2.0f;
    private bool shakeDetected = false;
    [SerializeField]
    private GameObject noEnergy;    
    public AudioClip startClip;
    public AudioClip idleClip;
    public AudioClip stopClip;
    private AudioSource audioSource;
    [SerializeField]
    private Image powerUp;
    [SerializeField]
    private float speed;
    [HideInInspector]
    public Vector3 oldScale;
    public float lengthBlade;
    // Start is called before the first frame update
    void Start()
    {
        MMVibrationManager.SetHapticsActive(hapticsAllowed);
    }
    private void OnEnable()
    {
        oldScale = transform.localScale;
        colorSlider.onValueChanged.AddListener(OnColorChanged);
        audioSource = GetComponent<AudioSource>();
        powerUp.fillAmount = 1;
        for (int i = 0; i < blade.Length; i++)
        {
            blade[i].transform.localScale = new Vector3(1, 0, 1);
        }
        lengthBlade = 0;
        for (int i = 0; i < colorSaber.Length; i++)
        {
            colorSaber[i].color = new Color(r, g, b);
        }
        for (int i = 0; i < vfx.Length; i++)
        {
            var mainModule = vfx[i].main;
            mainModule.startColor = new Color(r, g, b);
        }
        isTouching = false;
        audioSource.clip = null;
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
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began || saberController.isColor)
            {
                if (!isTouching)
                {
                    audioSource.clip = startClip;
                    audioSource.Play();
                }
                isTouching = true;              
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled || saberController.isColor)
            {
                isTouching = false;                
            }
        }
        if (isTouching && !isEnergy)
        {           
            if (!audioSource.isPlaying)
            {
                idleSaber();
            }
            if (lengthBlade < 1) 
            {
                lengthBlade += Time.deltaTime * speed;
            }
            if (lengthBlade > 1) 
            {
                lengthBlade = 1;               
            }
            powerUp.fillAmount -= Time.deltaTime * 0.1f;
            startVFX();
            if (gameManager.isFlash) flash.TurnOn();
            if (!saberController.isColor)
            {
                if (Input.accelerationEventCount > 0)
                {
                    Vector3 acceleration = Input.acceleration;
                    float totalAcceleration = acceleration.magnitude;
                    if (totalAcceleration > shakeThreshold)
                    {
                        shakeDetected = true;
                        flash.TurnOff();
                    }
                }
                if (shakeDetected)
                {
                    if (gameManager.isFlash)
                    {
                        flash.TurnOn();
                    }
                    if (gameManager.isHaptic)
                    {
                        MMVibrationManager.Haptic(hapticTypes, true, true, this);
                    }
                    shakeDetected = false;
                }
            }
        }
        else
        {
            stopVFX();
            if (lengthBlade > 0) 
            {                
                lengthBlade -= Time.deltaTime * speed;
                audioSource.clip = stopClip;
                audioSource.loop = false;
                audioSource.Play();
            }
            if (lengthBlade < 0) 
            {
                lengthBlade = 0;
            }
            if (gameManager.isFlash) flash.TurnOff();
        }
        if (powerUp.fillAmount == 0)
        {
            isEnergy = true;
            noEnergy.SetActive(true);
            powerUp.fillAmount += Time.deltaTime * 0.1f;
            if (saberController.isColor)
            {
                saberController.SetColor();
            }
        }
        if (isEnergy && !noEnergy.activeInHierarchy)
        {
            if (powerUp.fillAmount < 1) 
            {
                powerUp.fillAmount += Time.deltaTime;
                isTouching = false;
            } 
            else
            {
                powerUp.fillAmount = 1;
                isEnergy = false;
            }
        }
        for (int i = 0; i < blade.Length; i++)
        {
            blade[i].transform.localScale = new Vector3(1, lengthBlade, 1);
        }
    }        
    public void startVFX()
    {
        for (int i = 0; i < vfx.Length; i++)
        {
            if (!vfx[i].isPlaying) vfx[i].Play();
        }
    }
    public void stopVFX()
    {
        for (int i = 0; i < vfx.Length; i++)
        {
            if (vfx[i].isPlaying) vfx[i].Stop();
        }
    }
    public void idleSaber()
    {
        audioSource.clip = idleClip;
        audioSource.loop = true;
        audioSource.Play();
    }
    void OnColorChanged(float value)
    {
        if (value * 6 <= 1)
        {
            b = value * 6;
        }
        else if (value * 6 <= 2)
        {
            r = 2 - value * 6;
        }
        else if (value * 6 <= 3)
        {
            g = value * 6 - 2;
        }
        else if (value * 6 <= 4)
        {
            b = 4 - value * 6;
        }
        else if (value * 6 <= 5)
        {
            r = value * 6 - 4;
        }
        else
        {
            g = 6 - value * 6;
        }
        Color newColor = new Color(r, g, b);
        ChangeBloomColor(newColor);
    }
    private void ChangeBloomColor(Color color)
    {
        for (int i = 0; i < colorSaber.Length; i++)
        {
            colorSaber[i].color = color;
        }
        for (int i = 0; i < vfx.Length; i++)
        {
            var mainModule = vfx[i].main;
            mainModule.startColor = color;
        }
    }
}
