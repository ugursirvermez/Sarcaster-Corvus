using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSafehouse : MonoBehaviour
{
    private Vector3 startpoint;
    [SerializeField] private int xp = 50;

    private void Start()
    {
        startpoint = transform.position;
    }

    private void OnMouseDown()
    {
        LocationGameManager.Instance.MevcutPlayer.xpartsın(xp);
        Destroy(gameObject);
    }
}
