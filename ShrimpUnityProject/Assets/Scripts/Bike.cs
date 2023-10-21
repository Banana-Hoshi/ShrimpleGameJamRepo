using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bike : MonoBehaviour
{
	public Rigidbody rb;
	public float accel = 50f;
	public float rotAccel = 50f;
	public float maxSpeed = 50f;
	public float fixForce = 5f;

	private void Awake() {
		rb.centerOfMass = transform.position;
		Controls.Init();
	}

	float tilt;
	private void FixedUpdate() {
		//side tilt
		tilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
		if (Mathf.Abs(tilt) > 2f)
			rb.AddRelativeTorque((Vector3.forward * -tilt).normalized * fixForce);
		else
			rb.AddRelativeTorque(Vector3.forward * -tilt);

		//front tilt
		tilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);
		if (Mathf.Abs(tilt) > 2f)
			rb.AddRelativeTorque((Vector3.right * -tilt).normalized * fixForce);
		else
			rb.AddRelativeTorque(Vector3.right * -tilt);

		//clamp speed
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
	}

	WaitForFixedUpdate fixedUp = new WaitForFixedUpdate();
	IEnumerator Move() {
		while (_input.x > -10000f) {
			rb.velocity += transform.forward * _input.y * accel * Time.fixedDeltaTime;

			rb.AddRelativeTorque(Vector3.up * _input.x * rotAccel);

			yield return fixedUp;
		}
	}

	private void OnEnable() {
		Controls.controls.Game.Move.started += StartMove;
		Controls.controls.Game.Move.performed += ChangedMove;
		Controls.controls.Game.Move.canceled += StopMove;
	}

	private void OnDisable()
	{
		Controls.controls.Game.Move.started -= StartMove;
		Controls.controls.Game.Move.performed -= ChangedMove;
		Controls.controls.Game.Move.canceled -= StopMove;
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
