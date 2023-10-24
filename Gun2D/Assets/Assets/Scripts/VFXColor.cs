using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXColor : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] vfx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        for (int i = 0; i < vfx.Length; i++)
        {
            var mainModule = vfx[i].main;
            mainModule.startColor = gameObject.GetComponent<ParticleSystem>().main.startColor.color;
        }
    }
}
