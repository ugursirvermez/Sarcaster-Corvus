using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Temel bir sınıf oluşturmak adına, yani türetilecek random itemler için bir 
// abstract class'ımız var.
public abstract class LocationRansingle<T> : MonoBehaviour where T: MonoBehaviour
{
   private static T instance;

   //Burası kritik bir nokta; baktığımızda random itemler üretilmişse ve bunlar instance edilmişse
   //bunları bul eğer boş değilse yok et çünkü yeniden instance edilecek zaten.
   public static T Instance
   {
      get
      {
         if (instance == null)
         {
            instance = FindObjectOfType<T>();
         }
         DontDestroyOnLoad(instance);
         return instance;
      }
   }
}
