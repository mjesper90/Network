using System.Collections;
using DTOs;
using GameClient;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Weapon Weapon;

    private Transform _t;
    private Rigidbody _rb;
    private bool _isGrounded = false;
    public User UserInfo = null;
    public bool IsLocal = false;

    public void Awake()
    {
        _t = transform;
        _rb = GetComponent<Rigidbody>();
        UserInfo = new User("Player_" + Random.Range(0, 1000), _t.position.x, _t.position.y, _t.position.z, 100f);

        if (GameController.Instance.Players.Count == 0)
        {
        }
        else
        {
            //Change color
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
        Weapon = Instantiate(Resources.Load(CONSTANTS.WeaponPrefab) as GameObject, _t.position, _t.rotation).GetComponent<Weapon>();
        Weapon.Owner = this;
        Weapon.transform.SetParent(_t);
        //Move weapon slightly
        Weapon.transform.localPosition = new Vector3(0.5f, 0f, 0f);
    }

    public void Update()
    {
        if (IsLocal)
        {
            //Check if player is grounded
            if (_t.position.y <= 0.5f)
            {
                _t.position = new Vector3(_t.position.x, 0.5f, _t.position.z);
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

    private void Rotation()
    {
        //Rotate transform with mouse
        float mouseX = Input.GetAxis("Mouse X");
        _t.Rotate(Vector3.up * mouseX);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            Debug.Log("Jump");
            _rb.AddForce(Vector3.up * CONSTANTS.JumpForce, ForceMode.Impulse);
            StartCoroutine(JumpCoroutine());
        }
    }

    public void Shoot()
    {
        if (IsLocal && Weapon != null && Input.GetMouseButtonDown(0))
        {
            MonoProjectile p = Weapon.GetComponent<Weapon>().PewPew(true);
            GameController.Instance.Projectiles.Add(p.Projectile.ID, p.gameObject);
            ClientInit.Instance.Client.Send(p.Projectile);
        }
    }

    private void Movement()
    {
        RaycastHit groundHit;
        //Check for ground
        if (!_isGrounded && Physics.Raycast(_t.position, Vector3.down, out groundHit, 0.6f))
        {
            if (groundHit.collider.gameObject.tag == "Ground")
            {
                Debug.Log("Grounded");
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
            movement += _t.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= _t.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= _t.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += _t.right;
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
