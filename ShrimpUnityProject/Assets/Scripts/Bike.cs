using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bike : MonoBehaviour
{
	public PlayerInput inputMap;
	public Rigidbody rb;
	public float accel = 50f;
	public float maxSpeed = 50f;
	public float sideDrag = 10f;
	public float rotAccel = 50f;
	public float maxTurn = 5f;
	public float fixForce = 5f;
	public float maxTilt = 30f;

	public float tiltBadAngle = 30f;
	public float tiltBadTimer = 2f;
	public event Action tooMuchTilt;

	float tiltTimer = 0f;
	private void Awake() {
		rb.centerOfMass = transform.position;
		tiltTimer = 0f;
	}

	float tilt;
	Vector2 direction;
	private void FixedUpdate() {
		direction = new Vector2(rb.velocity.x, rb.velocity.z);
		tilt = Vector2.Angle(direction, new Vector2(transform.forward.x, transform.forward.z));
		if (tilt > 90f) {
			tilt = 180 - tilt;
		}
		direction = Vector2.ClampMagnitude(direction, Mathf.Lerp(maxSpeed, sideDrag, tilt / 90f));

		//clamp speed and add directional drag
		rb.velocity = new Vector3(direction.x, 0f, direction.y) + Vector3.up * rb.velocity.y;

		tilt = Mathf.SmoothStep(maxTurn, maxTurn * 0.5f, direction.magnitude / maxSpeed);

		rb.angularVelocity = new Vector3(Mathf.Lerp(rb.angularVelocity.x, 0f, Mathf.Sign(rb.angularVelocity.x) * transform.eulerAngles.x / maxTilt),
			Mathf.Clamp(rb.angularVelocity.y, -tilt, tilt),
			Mathf.Lerp(rb.angularVelocity.z, 0f, Mathf.Sign(rb.angularVelocity.z) * transform.eulerAngles.z / maxTilt));

		//side tilt
		tilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
		if (Mathf.Abs(tilt) > 2f)
			rb.AddRelativeTorque((Vector3.forward * -tilt).normalized * fixForce);
		else
			rb.AddRelativeTorque(Vector3.forward * -tilt);

		if (Mathf.Abs(tilt) > tiltBadAngle) {
			tiltTimer += Time.fixedDeltaTime;
			if (tiltTimer > tiltBadTimer) {
				tiltTimer -= tiltBadTimer;
				tooMuchTilt?.Invoke();
			}
		}

		//front tilt
		tilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);
		if (Mathf.Abs(tilt) > 2f)
			rb.AddRelativeTorque((Vector3.right * -tilt).normalized * fixForce);
		else
			rb.AddRelativeTorque(Vector3.right * -tilt);
	}

	WaitForFixedUpdate fixedUp = new WaitForFixedUpdate();
	IEnumerator Move() {
		while (_input.x > -10000f) {
			rb.velocity += Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward * (_input.y * accel * Time.fixedDeltaTime);

			rb.angularVelocity += Vector3.up * (_input.x * rotAccel * Time.fixedDeltaTime);

			yield return fixedUp;
		}
	}

	private void OnEnable() {
		InputAction move = inputMap.currentActionMap.FindAction("Move");
		move.started += StartMove;
		move.performed += ChangedMove;
		move.canceled += StopMove;
	}

	private void OnDisable()
	{
		InputAction move = inputMap.currentActionMap.FindAction("Move");
		move.started -= StartMove;
		move.performed -= ChangedMove;
		move.canceled -= StopMove;
	}

	Vector2 _input = Vector2.negativeInfinity;

	private void StartMove(InputAction.CallbackContext input) {
		_input = input.ReadValue<Vector2>();
		StartCoroutine(Move());
	}

	private void ChangedMove(InputAction.CallbackContext input) {
		_input = input.ReadValue<Vector2>();
	}

	private void StopMove(InputAction.CallbackContext input) {
		_input = Vector2.negativeInfinity;
	}
}
