using System.Collections;
using DTOs;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Weapon Weapon;
    public bool IsLocal = false;

    public bool InGame = false;
    public bool LoggedIn = false;
    private Rigidbody _rb;
    private bool _isGrounded = false;
    private User _user = null;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _user = new User("Player_" + Random.Range(0, 1000));

        if (GameController.Instance.Players.Count == 0)
        {
        }
        else
        {
            //Change color on other players
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
        Weapon = Instantiate(Resources.Load(CONSTANTS.WeaponPrefab) as GameObject, transform.position, transform.rotation).GetComponent<Weapon>();
        Weapon.Owner = this;
        Weapon.transform.SetParent(transform);
        //Move weapon slightly
        Weapon.transform.localPosition = new Vector3(0.5f, 0f, 0f);
    }

    public void Update()
    {
        if (IsLocal)
        {
            //Check if player is grounded
            if (transform.position.y <= 0.5f)
            {
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            }
            else if (!_isGrounded)
            {
                _rb.AddForce(Vector3.down * CONSTANTS.Gravity * Time.deltaTime * _rb.mass, ForceMode.Acceleration);
            }
            Shoot();
            Movement();
            Rotation();
        }
    }

    public void SetUser(DTOs.User user)
    {
        _user = user;
    }

    public DTOs.User GetUser()
    {
        return _user;
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * CONSTANTS.JumpForce, ForceMode.Impulse);
            StartCoroutine(JumpCoroutine());
        }
    }

    private void Rotation()
    {
        //Rotate transform with mouse
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Shoot()
    {
        if (Weapon != null && Input.GetMouseButtonDown(0))
        {
            MonoProjectile p = Weapon.GetComponent<Weapon>().PewPew(true);
        }
    }

    private void Movement()
    {
        RaycastHit groundHit;
        //Check for ground
        if (!_isGrounded && Physics.Raycast(transform.position, Vector3.down, out groundHit, 0.6f))
        {
            if (groundHit.collider.gameObject.tag == "Ground")
            {
                _isGrounded = true;
            }
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //WASD Movement 
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
        }
        movement.Normalize();
        movement *= CONSTANTS.PlayerSpeed;
        _rb.velocity = new Vector3(movement.x, _rb.velocity.y, movement.z);
    }

    //Jump coroutine
    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _isGrounded = false;
    }

    //Lerp movement
    public void LerpMovement(Vector3 vector3)
    {
        StartCoroutine(LerpMovementCoroutine(vector3, CONSTANTS.ServerSpeed));
    }

    //Lerp movement coroutine
    private IEnumerator LerpMovementCoroutine(Vector3 vector3, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, vector3, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = vector3;
    }
}
