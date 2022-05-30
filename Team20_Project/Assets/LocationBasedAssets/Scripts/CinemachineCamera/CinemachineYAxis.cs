using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineYAxis : MonoBehaviour
{

   // Mantik su; hani x axis'te range var ya o range'i matematik olarak clamp ediyoruz.
   //Gidiyoruz kesilen alan disinda hareket etmiyoruz. bu kadar. Calismaz sandim ama calisti... :D
   public CinemachineFreeLook freeLookCam;
       private float kesMin=0;
       private float kesMax=0;
       private float alan;
       private float hiz=2;
    
       void Update()
       {
           alan += hiz * Input.GetAxis("Mouse Y");
           alan = Mathf.Clamp(alan, kesMin, kesMax);
           freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = alan;
       }
}
