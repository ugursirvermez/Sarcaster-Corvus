using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LocationGameManager : LocationRansingle<LocationGameManager>
{
    private LocationPlayer mevcut_player;

    public LocationPlayer MevcutPlayer
    {
        get
        {
            if (mevcut_player==null)
            {
                mevcut_player = gameObject.AddComponent<LocationPlayer>();
            }

            return mevcut_player;
        }
    }
    
}
