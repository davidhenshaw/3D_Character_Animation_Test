using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    int id;
    Vector3 initPos;
    bool isOpen;
    [SerializeField] float openDistance = 2;
    [SerializeField] float openTime = 2;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Open")]
    public void Open()
    {
        //Tween up
        if(!isOpen)
        {
            transform.DOMoveY(initPos.y + openDistance, openTime, false).SetEase(Ease.InOutQuad);
            isOpen = true;
        }

    }

    [ContextMenu("Close")]
    public void Close()
    {
        //Tween Down
        if(isOpen)
        {
            transform.DOMoveY(initPos.y, openTime, false).SetEase(Ease.InOutQuad);
            isOpen = false;
        }
        
    }
}
