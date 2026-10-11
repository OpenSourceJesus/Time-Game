using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
	public static float time;
	public static float timeSpeed;

	void Awake ()
	{
#if UNITY_EDITOR
		time = 0;
		timeSpeed = 0;
#endif
	}

	void Update ()
	{
		time += Time.deltaTime * timeSpeed;
	}
}