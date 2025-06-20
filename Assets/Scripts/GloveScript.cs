using UnityEngine;

public class GloveScript : MonoBehaviour
{
    public bool grabbing = false;
    public bool holding = false;
    public Transform origin;

    public void SetOrigin(Transform newOrigin)
    {
        origin = newOrigin;
    }

    public void Activate()
    {
        grabbing = true;
    }
    public void DeActivate()
    {
        grabbing = false;   
    }

    public void Grab()
    {
        holding = true;
    }
    public void Release()
    {
        holding = false;
    }
    public Vector3 GetOrigin()
    {
        return origin.position;
    }
}

//very nice very cool -Elin