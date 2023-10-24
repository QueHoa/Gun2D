using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using OneHit;

public class ShopBomb : MonoBehaviour
{
    [HideInInspector]
    public int numberBomb = 0;
    public GameObject uiBombplay;
    public GameObject uiMode;
    public GameObject uiMainLobby;
    public GameObject loading;
    public ButtonUnlock[] button;
    public string[] bombName;
    public GameObject[] lerp;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (button[i].unlock == true)
            {
                bool isUnlock = PlayerPrefs.GetInt(bombName[i] + "_Unlocked", 0) == 1;
                button[i].unlock = !isUnlock;
            }
        }

        lerp[0].transform.localScale = new Vector3(0.7f, 0.7f, 1);
        lerp[1].transform.localScale = new Vector3(0.7f, 0.7f, 1);
        lerp[2].transform.position = new Vector3(-3.5f, lerp[2].transform.position.y, 0);

        lerp[0].transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        lerp[1].transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        lerp[2].GetComponent<RectTransform>().DOAnchorPosX(31, 0.4f).SetEase(Ease.OutBack);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void Select(int numBomb)
    {
        if (!button[numBomb].unlock)
        {
            MasterControl.Instance.ShowInterAd((success) =>
            {
                numberBomb = numBomb;
                StartCoroutine(Loading());
            });
        }
        else
        {
            MasterControl.Instance.ShowRewardAd((success) =>
            {
                UnlockBomb(numBomb);
                numberBomb = numBomb;
                StartCoroutine(Loading());
            });
        }
                
    }
    public void UnlockBomb(int bomb)
    {
        string itemName = bombName[bomb];
        PlayerPrefs.SetInt(itemName + "_Unlocked", 1);
        button[bomb].unlock = false;
    }
    public void SetBack()
    {       
        StartCoroutine(LoadingBack());
    }
    IEnumerator Loading()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        uiBombplay.SetActive(true);
        gameObject.SetActive(false);
    }
    IEnumerator LoadingBack()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        uiMode.SetActive(true);
        uiMainLobby.SetActive(true);
        gameObject.SetActive(false);
    }
}
