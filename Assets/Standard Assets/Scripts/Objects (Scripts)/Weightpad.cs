using UnityEngine;

public class Weightpad : MonoBehaviour
{
	public Door[] doors = new Door[0];
	
	void OnCollisionEnter2D (Collision2D collision)
	{
		foreach (Door door in doors)
			door.isWeightpadPressed = true;
	}

	void OnCollisionStay2D (Collision2D collision)
	{
		OnCollisionEnter2D (collision);
	}

	void OnCollisionExit2D (Collision2D collision)
	{
		foreach (Door door in doors)
			door.isWeightpadPressed = false;
	}
}