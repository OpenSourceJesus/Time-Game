using UnityEngine;
using UnityEngine.SceneManagement;

public class Hazard : MonoBehaviour
{
	public Transform trs;
	public static Hazard instance;

	void Awake ()
	{
		instance = this;
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
