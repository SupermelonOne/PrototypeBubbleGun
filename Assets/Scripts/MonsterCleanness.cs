using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterCleanness : MonoBehaviour
{
    List<DirtScript> dirtSpots = new List<DirtScript>();
    [SerializeField] private Sprite goodTexture;
    [SerializeField] private Sprite badTexture;
    [SerializeField] private SpriteRenderer status;

    private void Start()
    {
        List<DirtScript> customEdits = GetComponentsInChildren<DirtScript>().ToList<DirtScript>();
        foreach(var dirt in customEdits)
        {
            if (!dirtSpots.Contains(dirt))
            {
                dirtSpots.Add(dirt);
            }
        }
    }

    public void CheckDirt()
    {
        foreach (var dirt in dirtSpots)
        {
            if (dirt == null)
                dirtSpots.Remove(dirt);
            break;
        }
        if (dirtSpots.Count <= 0)
        {
            Debug.Log("awyeah clean");
            if (status != null)
                status.sprite = goodTexture;
        }
    }
    public void RemoveDirt(DirtScript dirt)
    {
        dirtSpots.Remove(dirt);
    }
}
