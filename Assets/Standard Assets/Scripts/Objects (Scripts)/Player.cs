using Extensions;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	public Transform trs;
	public Rigidbody2D rb;
	public float moveSpeed;
	public float jumpSpeed;
	bool isGrounded;
	bool isJumping;
	List<Orb> orbsInPickupRange = new List<Orb>();

	void Update ()
	{
		float move = InputManager.instance.moveInputActionRef.action.ReadValue<float>();
		if (InputManager.instance.jumpInputActionRef.action.WasPressedThisFrame() && isGrounded && !isJumping)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
			isJumping = true;
		}
		else if (rb.linearVelocity.y <= 0)
			isJumping = false;
		else if (InputManager.instance.jumpInputActionRef.action.WasReleasedThisFrame() && isJumping)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
			isJumping = false;
		}
		if (MathfExtensions.Sign(move) != 0)
			trs.localScale = new Vector3(MathfExtensions.Sign(move), 1, 1);
		if (InputManager.instance.resetInputActionRef.action.triggered)
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
		if (GameManager.timeSpeed > 0 && InputManager.instance.interactInputActionRef.action.WasPressedThisFrame() && orbsInPickupRange.Count > 0)
		{
			Orb closestOrb = orbsInPickupRange[0];
			for (int i = 1; i < orbsInPickupRange.Count; i ++)
			{
				Orb orb = orbsInPickupRange[i];
				if (Vector2.Distance(trs.position, orb.trs.position) < Vector2.Distance(trs.position, closestOrb.trs.position))
					closestOrb = orb;
			}
			closestOrb.col.enabled = false;
			closestOrb.rb.simulated = false;
			closestOrb.trs.SetParent(trs);
			closestOrb.trs.localPosition = Vector3.zero;
		}
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		ContactPoint2D contactPnt = coll.GetContact(0);
		if (contactPnt.normal.y > Vector2.one.normalized.x * .75f && contactPnt.point.y < trs.position.y)
		{
			isGrounded = true;
			if (!isJumping && rb.linearVelocity.y > 0)
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
		}
	}

	void OnCollisionStay2D (Collision2D coll)
	{
		OnCollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll)
	{
		if (isGrounded && !isJumping && rb.linearVelocity.y > 0)
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
		isGrounded = false;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		Orb orb = other.GetComponent<Orb>();
		if (orb != null)
			orbsInPickupRange.Add(orb);
	}

	void OnTriggerExit2D (Collider2D other)
	{
		Orb orb = other.GetComponent<Orb>();
		if (orb != null)
			orbsInPickupRange.Remove(orb);
	}
}
