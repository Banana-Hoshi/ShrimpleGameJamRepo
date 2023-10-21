using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
	public PlayerInput inputMap;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _jumpForce = 1;
    private Vector2 _moveDir;
    private bool isGrounded;

	void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
	{
		InputAction move = inputMap.currentActionMap.FindAction("Move");
		InputAction jump = inputMap.currentActionMap.FindAction("Jump");
		move.performed += SetMovementDirection;
		jump.performed += Jump;
    }

    private void OnDisable()
	{
		InputAction move = inputMap.currentActionMap.FindAction("Move");
		InputAction jump = inputMap.currentActionMap.FindAction("Jump");
		move.performed -= SetMovementDirection;
        jump.performed -= Jump;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * (_moveDir.y * Time.deltaTime * _moveSpeed), Space.Self);
        transform.Translate(Vector3.right * (_moveDir.x * Time.deltaTime * _moveSpeed), Space.Self);
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, GetComponent<Collider>().bounds.extents.y);
    }
    public void SetMovementDirection(InputAction.CallbackContext ctx)
    {
        _moveDir = ctx.ReadValue<Vector2>();
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded) _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpForce, _rigidbody.velocity.z);
    }
}
