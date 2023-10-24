using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Mode : MonoBehaviour
{
    public GameObject[] icon;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        icon[0].transform.localScale = new Vector3(0.7f, 0.7f, 1);
        icon[1].transform.position = new Vector3(4, icon[1].transform.position.y, icon[1].transform.position.z);
        icon[2].transform.position = new Vector3(-4, icon[2].transform.position.y, icon[2].transform.position.z);
        icon[3].transform.position = new Vector3(4, icon[3].transform.position.y, icon[3].transform.position.z);
        icon[4].transform.position = new Vector3(-3.5f, icon[4].transform.position.y, 0);
        icon[5].transform.position = new Vector3(3.5f, icon[5].transform.position.y, 0);

        icon[0].transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.4f).SetEase(Ease.OutBack);
        icon[1].transform.DOMoveX(0, 0.4f).SetEase(Ease.OutBack);
        icon[2].transform.DOMoveX(0, 0.4f).SetEase(Ease.OutBack);
        icon[3].transform.DOMoveX(-0.07f, 0.4f).SetEase(Ease.OutBack);
        icon[4].GetComponent<RectTransform>().DOAnchorPosX(140, 0.4f).SetEase(Ease.OutBack);
        icon[5].GetComponent<RectTransform>().DOAnchorPosX(-140, 0.4f).SetEase(Ease.OutBack);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
