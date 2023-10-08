using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMusicPlayer : MonoBehaviour
{
	void Awake()
	{
		if (FindObjectsOfType<SingletonMusicPlayer>().Length > 1)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(gameObject);
	}
}
