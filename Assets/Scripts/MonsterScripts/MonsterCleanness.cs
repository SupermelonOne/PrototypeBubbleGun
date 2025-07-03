using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class MonsterCleanness : MonoBehaviour
{
    [SerializeField] private int pointsRewarded = 5;

    private float completePercentage = 0;

    List<DirtScript> dirtSpots = new List<DirtScript>();
    [SerializeField] private Sprite goodTexture;
    [SerializeField] private Sprite badTexture;
    [SerializeField] private SpriteRenderer status;
    public bool clean = false;
    public bool done = false;

    private int amountOfDirtSpots;

    private void Start()
    {
        List<DirtScript> customEdits = GetComponentsInChildren<DirtScript>().ToList<DirtScript>();
        foreach(var dirt in customEdits)
        {
            if (!dirtSpots.Contains(dirt))
            {
                dirtSpots.Add(dirt);
                amountOfDirtSpots++;
            }
        }
        //Debug.Log(amountOfDirtSpots);
    }

    public void CheckDirt()
    {
        if (amountOfDirtSpots <= 0)
        {
            clean = true;
            SetDone();
            MonsterEventBus.Invoke(new MonsterEventBus.DirtCleaned(pointsRewarded, ItemType.Munny));
            Debug.Log("aw yeah clean");
        }
    }
    public void RemoveDirt(DirtScript dirt)
    {
        dirtSpots.Remove(dirt);
        amountOfDirtSpots--;
        CheckDirt();
    }
    public void AddDirt(DirtScript dirt)
    {
        dirtSpots.Add(dirt);
        amountOfDirtSpots++;
    }

    public void GetSoaped()
    {
        foreach(var dirt in dirtSpots)
        {
            dirt.GetSoaped();
        }
    }
    public void DeSoaped()
    {
        foreach (var dirt in dirtSpots)
        {
            dirt.GetDeSoaped();
        }
    }

    public void SetDone()
    {
        status.sprite = goodTexture;
        done = true;
    }

    public void SetUndone()
    {
        status.sprite = badTexture;
        done = false;
    }
}
