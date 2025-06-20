using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtTestScript : MonoBehaviour
{
    [SerializeField] private Material material;
    private float dirtValue = 0;
    private bool up = true;
    private bool swap = false;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        Debug.Log(dirtValue);
        if (up)
        {
            dirtValue += Time.deltaTime;
        }
        else
        {
            dirtValue -= Time.deltaTime;
        }
        if (dirtValue > 0 && dirtValue < 1)
        {
            swap = false;
            material.SetFloat("_DirtValue", dirtValue);
        }
        else
        {
            if (!swap)
            {
                swap = true;
                up = !up;
            }

        }
    }
}
