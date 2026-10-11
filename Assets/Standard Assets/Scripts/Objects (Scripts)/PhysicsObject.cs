using UnityEngine;
using System.Collections.Generic;

public class PhysicsObject : MonoBehaviour
{
	public Transform trs;
	public Rigidbody2D rb;
	public Collider2D col;
	public List<Frame> frames = new List<Frame>();
	public static List<PhysicsObject> instances = new List<PhysicsObject>();

	public virtual void Awake ()
	{
		instances.Add(this);
	}

	void OnDestroy ()
	{
		instances.Remove(this);
	}

	void Update ()
	{
		if (GameManager.timeSpeed == 0)
			return;
		else if (GameManager.timeSpeed > 0)
			frames.Add(new Frame(rb));
		else
			while (frames.Count > 0)
			{
				Frame frame = frames[^1];
				if (GameManager.time <= frame.time)
				{
					frames.RemoveAt(frames.Count - 1);
					rb.position = frame.pos;
					rb.linearVelocity = frame.vel;
				}
				else
					return;
			}
	}

	public struct Frame
	{
		public float time;
		public Vector2 pos;
		public Vector2 vel;
		
		public Frame (Rigidbody2D rb)
		{
			time = GameManager.time;
			pos = rb.position;
			vel = rb.linearVelocity;
		}
	}
}
