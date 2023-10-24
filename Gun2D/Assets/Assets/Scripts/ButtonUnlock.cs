using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUnlock : MonoBehaviour
{
    [SerializeField]
    private GameObject bgUnlock;
    public bool unlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bgUnlock.SetActive(unlock);
    }
}
