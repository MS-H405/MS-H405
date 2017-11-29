using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (T)FindObjectOfType(typeof(T));
      
				if (instance == null)
                { 
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();                
				}
			}

			return instance;
		}
	}
}
