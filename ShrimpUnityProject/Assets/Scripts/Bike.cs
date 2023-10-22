using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Bike : MonoBehaviour
{
	public Transform axle;
	public PlayerInput inputMap;
	public Rigidbody rb;
	public float accel = 50f;
	public float maxSpeed = 50f;
	public float sideDrag = 10f;
	public float rotAccel = 50f;
	public float maxTurn = 5f;
	public float fixForce = 5f;
	public float maxTilt = 30f;

	public Transform[] spinyThings;
	public float spinySpeed = 5f;

	public float tiltBadAngle = 30f;
	public float tiltBadTimer = 2f;
	public event Action tooMuchTilt;
	public Image square;


	float jalepenioModeTimer = 0f;
	float mushroomModeTimer = 0f;
	float fishModeTimer = 0f;

	float tiltTimer = -1f;
	private void Awake() {
		rb.centerOfMass = transform.position;
		tiltTimer = 0f;
		square.color = Color.yellow;
	}

	float tilt;
	Vector2 direction;

	private void FixedUpdate() {
		if (jalepenioModeTimer > 0f) {
			jalepenioModeTimer -= Time.fixedDeltaTime;
		}
		if (mushroomModeTimer > 0f) {
			mushroomModeTimer -= Time.fixedDeltaTime;
		}
		if (fishModeTimer > 0f) {
			fishModeTimer -= Time.fixedDeltaTime;
		}

		direction = new Vector2(rb.velocity.x, rb.velocity.z);
		if (fishModeTimer > 0f) {
			tilt = 0f;
		}
		else {
			tilt = Vector2.Angle(direction, new Vector2(transform.forward.x, transform.forward.z));
			if (tilt > 90f) {
				tilt = 180 - tilt;
			}
		}

		//clamp speed and add directional drag
		if (jalepenioModeTimer > 0f) {
			direction = Vector2.ClampMagnitude(direction, Mathf.Lerp(maxSpeed * 2f, sideDrag, tilt / 90f));
		}
		else {
			direction = Vector2.ClampMagnitude(direction, Mathf.Lerp(maxSpeed, sideDrag, tilt / 90f));
		}

		rb.velocity = new Vector3(direction.x, 0f, direction.y) + Vector3.up * rb.velocity.y;

		tilt = direction.magnitude;
		foreach (Transform trans in spinyThings) {
			trans.Rotate(Vector3.right, tilt * spinySpeed * Time.fixedDeltaTime, Space.Self);
		}

		//side tilt
		tilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
		if (fishModeTimer > 0f)
				rb.AddRelativeTorque((Vector3.forward * -tilt).normalized * fixForce * 0.1f);
		else if (Mathf.Abs(tilt) > 2f) {
			rb.AddRelativeTorque((Vector3.forward * -tilt).normalized * fixForce);
			rb.AddTorque(Vector3.up * tilt);
		}
		else
			rb.AddRelativeTorque(Vector3.forward * -tilt);
		direction.y = tilt;

		if (Mathf.Abs(tilt) > tiltBadAngle) {
			tiltTimer += Time.fixedDeltaTime;
			if (tiltTimer > tiltBadTimer) {
				tiltTimer -= tiltBadTimer;
				tooMuchTilt?.Invoke();
			}
			square.color = Color.Lerp(Color.yellow, Color.red, tiltTimer / tiltBadTimer);
		}

		//front tilt
		tilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);
		if (Mathf.Abs(tilt) > 2f)
			rb.AddRelativeTorque((Vector3.right * -tilt).normalized * fixForce);
		else
			rb.AddRelativeTorque(Vector3.right * -tilt);
		direction.x = tilt;

		Vector3 euler = transform.TransformDirection(direction.x, 0f, direction.y);

		tilt = Mathf.SmoothStep(maxTurn, maxTurn * 0.5f, direction.magnitude / maxSpeed);
		rb.angularVelocity = new Vector3(Mathf.Lerp(rb.angularVelocity.x, 0f, Mathf.Sign(rb.angularVelocity.x) * euler.x / maxTilt),
			Mathf.Clamp(rb.angularVelocity.y, -tilt, tilt),
			Mathf.Lerp(rb.angularVelocity.z, 0f, Mathf.Sign(rb.angularVelocity.z) * euler.z / maxTilt));
	}

	WaitForFixedUpdate fixedUp = new WaitForFixedUpdate();
	IEnumerator Move() {
		Vector3 euler = axle.localRotation.eulerAngles;
		while (_input.x > -10000f || jalepenioModeTimer > 0f) {
			if (jalepenioModeTimer > 0f) {
				rb.velocity += Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward * (accel * Time.fixedDeltaTime);
				if (_input.x > -10000f)
					rb.angularVelocity += Vector3.up * (_input.x * rotAccel * 0.5f * Time.fixedDeltaTime);
			}
			else if (fishModeTimer > 0f) {
				rb.velocity += Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward * (_input.y * accel * 0.5f * Time.fixedDeltaTime);
				rb.angularVelocity += Vector3.up * (_input.x * rotAccel * 0.1f * Time.fixedDeltaTime);
			}
			else {
				rb.velocity += Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward * (_input.y * accel * Time.fixedDeltaTime);
				rb.angularVelocity += Vector3.up * (_input.x * rotAccel * Time.fixedDeltaTime);
			}
			axle.localRotation = Quaternion.Euler(euler.x, euler.y + _input.x * 15f, euler.z);

			yield return fixedUp;
		}
		axle.localRotation = Quaternion.Euler(euler);
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
		if (mushroomModeTimer > 0f) {
			_input = -input.ReadValue<Vector2>();
		}
		else {
			_input = input.ReadValue<Vector2>();
		}
	}

	private void StopMove(InputAction.CallbackContext input) {
		_input = Vector2.negativeInfinity;
	}

	public void SetJalepenio(float time) {
		jalepenioModeTimer = time;
		if (_input.x < -10000f) {
			StartCoroutine(Move());
		}
	}
	public void SetMushroom(float time) {
		mushroomModeTimer = time;
	}
	public void SetFish(float time) {
		fishModeTimer = time;
	}
}
