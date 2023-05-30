using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Doors : MonoBehaviour
{
    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Open()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            transform.DOBlendableLocalRotateBy(new Vector3(0, 0, 90), 1f);

        }
        else
        {
            transform.DOBlendableLocalRotateBy(new Vector3(0, 0, -90), 1f);

        }
    }
}
