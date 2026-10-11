using Extensions;
using UnityEngine;

public class Orb : PhysicsObject
{
	public float timeSpeedChange;
	
	void OnTriggerEnter2D (Collider2D other)
	{
		ChangeTimeSpeed (timeSpeedChange);
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (rb.simulated)
			ChangeTimeSpeed (-timeSpeedChange);
	}

	void ChangeTimeSpeed (float amt)
	{
		float prevTimeSpeed = GameManager.timeSpeed;
		GameManager.timeSpeed += amt;
		if (MathfExtensions.Sign(prevTimeSpeed) != MathfExtensions.Sign(GameManager.timeSpeed))
			foreach (PhysicsObject physicsObject in PhysicsObject.instances)
			{
				if (GameManager.timeSpeed > 0)
				{
					physicsObject.rb.bodyType = RigidbodyType2D.Dynamic;
					if (physicsObject.frames.Count > 0)
						physicsObject.rb.linearVelocity = physicsObject.frames[^1].vel;
				}
				else
				{
					physicsObject.rb.bodyType = RigidbodyType2D.Static;
					physicsObject.rb.simulated = true;
					physicsObject.trs.SetParent(null);
					physicsObject.col.enabled = true;
				}
			}
	}
}
