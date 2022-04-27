using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    [SerializeField] CapsuleCollider hitbox;
    [SerializeField] Vector3 scaleOffset;
    [SerializeField] float hitboxScaleFactor = 0.2f;
    float scaleIncrement = 0.25f;
    float initRadius;
    float radiusOffset = 0;

    Vector3 initScale;

    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
        initRadius = hitbox.radius;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            StretchVertically(scaleIncrement);
        }

        if(Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            StretchVertically(-1*scaleIncrement);
        }
    }

    void StretchVertically(float amount)
    {
        scaleOffset.y += amount;

        //Stretch y scale and move up
        transform.localScale = new Vector3(initScale.x + scaleOffset.x, 
                                            initScale.y + scaleOffset.y, 
                                            initScale.z + scaleOffset.z);

        radiusOffset = scaleOffset.y * hitboxScaleFactor;

        hitbox.radius = initRadius + radiusOffset;
    }
}
