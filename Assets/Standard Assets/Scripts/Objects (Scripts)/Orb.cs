using Extensions;
using UnityEngine;

public class Orb : PhysicsObject
{
	public float timeSpeedChange;

	public override Vector2 GetVelocity ()
	{
		if (Player.instance.pickedUpOrb == this)
			return Player.instance.rb.linearVelocity;
		return base.GetVelocity();
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if (!other.isTrigger && Player.instance.orbIJustThrew != this)
			ChangeTimeSpeed (timeSpeedChange);
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (!other.isTrigger && rb.simulated)
		{
			ChangeTimeSpeed (-timeSpeedChange);
			Player.instance.orbIJustThrew = null;
		}
	}

	public static void ChangeTimeSpeed (float amt)
	{
		float prevTimeSpeed = GameManager.timeSpeed;
		GameManager.timeSpeed += amt;
		if (MathfExtensions.Sign(prevTimeSpeed) != MathfExtensions.Sign(GameManager.timeSpeed))
			foreach (PhysicsObject physicsObject in PhysicsObject.instances)
			{
				if (GameManager.timeSpeed > 0)
				{
					if (physicsObject.isPushable)
					{
						physicsObject.rb.bodyType = RigidbodyType2D.Dynamic;
						if (physicsObject.frames.Count > 0)
							physicsObject.rb.linearVelocity = physicsObject.frames[^1].vel;
					}
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
