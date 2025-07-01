using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkScript : MonoBehaviour
{
    [SerializeField] private float blinkspeed = 1;
    public bool isBlinking = false;
    private float eyelidValue = 0;
    private Image eyelids;
    private void Start()
    {
        eyelids = GetComponent<Image>();
    }

    private void Update()
    {
        if (isBlinking)
        {
            if (eyelidValue < 1)
            {
                eyelidValue += Time.deltaTime * blinkspeed;
                if (eyelidValue > 1)
                    eyelidValue = 1;
                eyelids.color = new Color(eyelids.color.r, eyelids.color.g, eyelids.color.b, eyelidValue);
            }
        }
        else
        {
            if (eyelidValue > 0)
            {
                eyelidValue -= Time.deltaTime * blinkspeed;
                if (eyelidValue < 0)
                {
                    eyelidValue = 0;
                }
                eyelids.color = new Color(eyelids.color.r, eyelids.color.g, eyelids.color.b, eyelidValue);
            }

        }
    }
}
