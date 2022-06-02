using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    
    void Start()
    {
        haberyarat();
        DateTime startimer= System.DateTime.Now.AddSeconds(2);
        haberiyolla("Sarcaster Corvus'un dünyasına hoş geldiniz!",startimer);
    }
    void OnApplicationQuit(){
    DateTime exitime=  System.DateTime.Now.AddMinutes(1);
     haberiyolla("Corvus'u bir başına bıraktın! Sensiz bu çılgın dünya çekilmez ki!", exitime);
    }
    
    void OnApplicationPause(bool isPause){
    /* if(isPause){
    DateTime pausetime=  System.DateTime.Now.AddMinutes(1);
    haberiyolla("Corvus'u bir başına bıraktın! Sensiz bu çılgın dünya çekilmez ki!", pausetime);
    } */
    }
    
    void haberyarat()
    {
        var haber = new AndroidNotificationChannel()
        {
            Id = "Sarcaster",
            Name = "Sarcaster Corvus",
            Importance = Importance.Default,
            Description = "Description",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(haber);
    }

    void haberiyolla(string text, DateTime timer)
    {
        var notification = new AndroidNotification();
        notification.Title = "Sarcaster Corvus";
        notification.Text = text;
        //AddMinutes ile dakikalık yapılabilir.
        notification.FireTime = timer;
        //Oyun için ikonlar üretince buraya ekelyeceğiz...
        //notification.SmallIcon = "";
        //notification.LargeIcon = "";
        AndroidNotificationCenter.SendNotification(notification, "Sarcaster");
    }
}