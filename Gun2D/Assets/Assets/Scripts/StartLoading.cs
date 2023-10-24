using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using OneHit;

public class StartLoading : MonoBehaviour
{
    [SerializeField]
    private Image loadingBar;
    [SerializeField]
    private AnimationCurve curve;
    // Start is called before the first frame update
    void Start()
    {
        loadingBar.fillAmount = 0;
        StartCoroutine(Loading());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Loading()
    {
        float t = 0f;

        while (t < 0.8f)
        {
            t += Time.deltaTime * 0.12f;
            loadingBar.fillAmount = curve.Evaluate(t);
            yield return 0;
        }
        MasterControl.Instance.ShowOpenAd((sucess) =>
        {
            SceneManager.LoadScene("MasterLevel");
        });        
    }
}
