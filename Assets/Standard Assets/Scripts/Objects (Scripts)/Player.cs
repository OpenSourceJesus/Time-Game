using Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : SingletonMonoBehaviour<Player>
{
	public Transform trs;
	public Rigidbody2D rb;
	public Collider2D col;
	public LineRenderer aimingVisualizer;
	public LayerMask whatOrbsCollideWith;
	public float moveSpeed;
	public float jumpSpeed;
	public float throwSpeed;
	public float minRangeToUseFullThrowSpeed;
	[HideInInspector]
	public Orb pickedUpOrb;
	[HideInInspector]
	public Orb orbIJustThrew;
	bool isGrounded;
	bool isJumping;
	List<Orb> orbsInPickupRange = new List<Orb>();
	Vector2 aimVector;

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
		HandlePickup ();
		HandleAim ();
		HandleThrow ();
	}

	void HandlePickup ()
	{
		if (GameManager.timeSpeed > 0 && InputManager.instance.interactInputActionRef.action.WasPressedThisFrame() && orbsInPickupRange.Count > 0)
		{
			pickedUpOrb = orbsInPickupRange[0];
			for (int i = 1; i < orbsInPickupRange.Count; i ++)
			{
				Orb orb = orbsInPickupRange[i];
				if (Vector2.Distance(trs.position, orb.trs.position) < Vector2.Distance(trs.position, pickedUpOrb.trs.position))
					pickedUpOrb = orb;
			}
			pickedUpOrb.col.enabled = false;
			pickedUpOrb.rb.simulated = false;
			pickedUpOrb.trs.SetParent(trs);
			pickedUpOrb.trs.localPosition = Vector3.zero;
		}
	}

	void HandleAim ()
	{
		if (pickedUpOrb == null)
		{
			aimingVisualizer.enabled = false;
			return;
		}
		aimVector = new Vector2();
		if (InputManager.UsingGamepad)
			aimVector = InputManager.instance.aimInputActionRef.action.ReadValue<Vector2>() * throwSpeed;
		else if (InputManager.UsingMouse)
			aimVector = Camera.main.ScreenToWorldPoint(InputManager.MousePosition.Value) - trs.position;
		aimVector = Vector2.ClampMagnitude(aimVector / minRangeToUseFullThrowSpeed, 1);
		Vector2 currentPosition = trs.position;
		Vector2 currentVelocity = aimVector * throwSpeed + rb.linearVelocity;
		List<Vector3> positions = new List<Vector3>();
		positions.Add(currentPosition);
		while (CameraScript.Instance.viewRect.Contains(currentPosition))
		{
			currentVelocity += Physics2D.gravity * Time.fixedDeltaTime;
			currentVelocity *= 1f - Time.fixedDeltaTime * pickedUpOrb.rb.linearDamping;
			currentPosition += currentVelocity * Time.fixedDeltaTime;
			RaycastHit2D hit = Physics2DExtensions.LinecastWithWidth(positions[positions.Count - 1], currentPosition, aimingVisualizer.startWidth, whatOrbsCollideWith);
			if (hit.collider != null)
			{
				positions.Add(hit.point);
				break;
			}
			if (positions.Contains(currentPosition))
				break;
			positions.Add(currentPosition);
		}
		aimingVisualizer.positionCount = positions.Count;
		aimingVisualizer.SetPositions(positions.ToArray());
		aimingVisualizer.enabled = true;
	}

	void HandleThrow ()
	{
		if (pickedUpOrb != null && InputManager.instance.throwInputActionRef.action.WasPressedThisFrame())
		{
			Physics2D.IgnoreCollision(col, pickedUpOrb.col, true);
			pickedUpOrb.col.enabled = true;
			pickedUpOrb.rb.simulated = true;
			pickedUpOrb.trs.SetParent(null);
			pickedUpOrb.rb.linearVelocity = aimVector * throwSpeed + rb.linearVelocity;
			StartCoroutine(ThrowRoutine (pickedUpOrb));
			orbIJustThrew = pickedUpOrb;
			pickedUpOrb = null;
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

	IEnumerator ThrowRoutine (Orb orb)
	{
		while (Physics2D.OverlapCircle(orb.trs.position, orb.col.bounds.extents.x, LayerMask.GetMask("Player")))
			yield return null;
		Physics2D.IgnoreCollision(col, orb.col, false);
	}
}
