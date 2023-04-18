using System.Collections;
using DTOs;
using GameClient;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 2000f;

    private Transform _t;
    private Rigidbody _rb;
    private bool _isGrounded = false;
    private User _user;
    public bool IsLocal = false;

    public void Awake()
    {
        _t = transform;
        _rb = GetComponent<Rigidbody>();
        _user = new User("Player_" + Random.Range(0, 1000), _t.position.x, _t.position.y, _t.position.z);
        _user.Health = 100f;

        if (GameController.Instance.Players.Count == 0)
        {
            GameController.Instance.Players.Add(_user.Username, gameObject);
            IsLocal = true;
        }
        else
        {
            //Change color
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }

    public void Update()
    {
        if (IsLocal)
        {
            //Gravity with deltaTime
            _rb.AddForce(Vector3.down * 9.82f * Time.deltaTime * _rb.mass, ForceMode.Acceleration);
            Movement();
            Rotation();
        }
    }

    private void Rotation()
    {
        //Rotate transform with mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _t.Rotate(Vector3.up * mouseX);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            Debug.Log("Jump");
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(JumpCoroutine());
        }
    }

    public User GetUser()
    {
        _user = new User(_user.Username, _t.position.x, _t.position.y, _t.position.z, _user.Health);
        return _user;
    }

    private void Movement()
    {
        //Check for ground
        if (!_isGrounded && Physics.Raycast(_t.position, Vector3.down, 1f))
        {
            Debug.Log("Grounded");
            _isGrounded = true;
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //WASD Movement
        if (Input.GetKey(KeyCode.W))
        {
            _t.position += _t.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _t.position -= _t.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _t.position -= _t.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _t.position += _t.right * speed * Time.deltaTime;
        }
    }

    //Jump coroutine
    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _isGrounded = false;
    }

    public void LerpMovement(Vector3 vector3)
    {
        StartCoroutine(LerpMovementCoroutine(vector3, CONSTANTS.ServerSpeed));
    }

    private IEnumerator LerpMovementCoroutine(Vector3 vector3, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = _t.position;
        while (elapsedTime < time)
        {
            _t.position = Vector3.Lerp(startingPos, vector3, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _t.position = vector3;
    }
}
