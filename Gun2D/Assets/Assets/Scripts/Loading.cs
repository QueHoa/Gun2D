using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public AnimationCurve curve;
    public Image bgLoading;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        time = 0;        
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time < 0.8f)
        {
            if (time < 0.2f)
            {
                float a = curve.Evaluate(time + 0.8f);
                bgLoading.color = new Color(1f, 1f, 1f, a);
            }
            if (time > 0.6f)
            {
                float a = curve.Evaluate(1.6f - time);
                bgLoading.color = new Color(1f, 1f, 1f, a);
            }
        }
        if(time > 0.79f)
        {
            gameObject.SetActive(false);
        }
    }
}
