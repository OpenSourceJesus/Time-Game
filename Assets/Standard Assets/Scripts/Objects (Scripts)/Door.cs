using UnityEngine;

public class Door : PhysicsObject
{
	public Transform otherPos;
	public float speed;
	[HideInInspector]
	public bool isWeightpadPressed;
	Vector2 initPos;

	public override void Awake ()
	{
		base.Awake ();
		initPos = rb.position;
		otherPos.SetParent(null);
	}

	public override void Update ()
	{
		if (GameManager.timeSpeed == 0)
			return;
		else if (GameManager.timeSpeed > 0)
		{
			if (isWeightpadPressed)
				rb.position = Vector2.Lerp(rb.position, otherPos.position, speed * Time.deltaTime * (1f / Vector2.Distance(rb.position, otherPos.position)));
			else
				rb.position = Vector2.Lerp(rb.position, initPos, speed * Time.deltaTime * (1f / Vector2.Distance(rb.position, initPos)));
		}
		base.Update ();
	}
}