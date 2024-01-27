using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Cinemachine;

public class Controller : MonoBehaviour
{
    // Input system
    private Gameplay controls = null;
    [HideInInspector] public Vector2 movement = Vector2.zero;
    private PlayerInput plInput;
    public static string currentDevice;
    private Vector2 direction = Vector2.zero;


    // Components
    private Animator anim;
    private Rigidbody2D rb = null;
    private MenuDrop menuDrop;
    private CinemachineVirtualCamera cmv;

    // WEAPON
    private Transform gun;
    private RotGun gunScript;
    private bool isReadyToFire = true;

    // Movement
    private bool isGrounded;
    public float playerSpeed = 0;
    [Tooltip ("How much player speed is reduced in the air")] public float airSpeedMod = 0.56f;
    public float jumpPower = 0;
    public LayerMask groundLayer;
    public static bool control; // Variable for cutscenes => Turn off/on movement ability
    private bool alive;
    private GameObject actingObject = null;
    private bool isActing = false;

    public Transform particleOrigin;
    public BoxRay groundRay;
    public SegmentRay stepRay;
    public GameObject[] deathEffects;

    [Tooltip ("Max time beetween input and jump")] public float bufferingTime = 0.1f;
    public float stepHeight = 0.1f;
    private float jumpPressTime = float.NegativeInfinity;

    private List<Platform> contactedPlatforms;


    // Effects && Sounds
    public GameObject GroundEffect;

    void Awake() {
        // Input init
        controls = new Gameplay();
        plInput = gameObject.GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        contactedPlatforms = new List<Platform> ();
        control = alive = true;
        menuDrop = GameObject.FindGameObjectWithTag("MenuDrop").GetComponent<MenuDrop>();
        cmv = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cmv.Follow = transform;
        gun = GameObject.Find("Gun").transform;
        gunScript = gun.GetComponent<RotGun>();
    }

    private void FixedUpdate()
    {
        if (control && alive)
        {
            TryJump ();
            ApplyStep ();
            GroundCheck ();
            MovePlayer ();    
            RotateGun ();
        }
        else {
            anim.SetBool ("IsMoving", false);
            rb.velocity = Vector2.zero;
        }
        foreach (Platform platform in contactedPlatforms) {
            rb.velocity += new Vector2 (platform.velocity.x, 0);
        }

        anim.SetFloat ("VerticalSpeed", rb.velocity.y);
        anim.SetBool ("IsGrounded", isGrounded);

        currentDevice = plInput.currentControlScheme;
    }

    private void RotateGun() {
        if (currentDevice == "Keyboard") {
            var mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var angle = Vector2.Angle(Vector2.right, mousePosition - gun.position);
            gun.eulerAngles = new Vector3(0f, 0f, gun.position.y < mousePosition.y ? angle : -angle);
        }
        else if (currentDevice == "Gamepad") {
            var angle = Vector2.Angle(Vector2.right, new Vector2(direction.x * 10, direction.y * 10) - new Vector2(gun.position.x, gun.position.y));
            gun.eulerAngles = new Vector3(0f, 0f, gun.position.y < direction.y * 10 ? angle : -angle);
        }

        if (isReadyToFire) {
            isReadyToFire = false;
            StartCoroutine(FireAndReload());
        }
    }

    IEnumerator FireAndReload() {
        gunScript.Fire();
        yield return new WaitForSeconds(gunScript.reloadTime);
        isReadyToFire = true;
    }

    private void OnMovePerformed(InputAction.CallbackContext value) => movement = value.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext value) => movement = Vector2.zero;
    private void OnDirectionPerformed(InputAction.CallbackContext value) => direction = value.ReadValue<Vector2>();
    private void OnDirectionCanceled(InputAction.CallbackContext value) => direction = Vector2.zero;
    
    void TryJump () {
        if (isGrounded && (Time.time - jumpPressTime) < bufferingTime)  {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }

    private void MovePlayer () {
        if (movement.x != 0) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign (movement.x);
            transform.localScale = scale;
        }
        anim.SetBool ("IsMoving", movement.x != 0);
        rb.velocity = new Vector2(movement.x * playerSpeed * Time.fixedDeltaTime * 10 * (isGrounded ? 1 : airSpeedMod), rb.velocity.y);
    }

    private void GroundCheck () {
        isGrounded = groundRay.Raycast (groundLayer);
    }

    public void ApplyStep () {
        if (rb.velocity.y > 0) return;

        float step = CalulateStep ();
        if (step > stepHeight || step == 0) return;
        transform.Translate (Vector3.up * step);
    }

    private float CalulateStep () {
        stepRay.Raycast (groundLayer, out RaycastHit2D hit);
        if (Physics2D.Raycast (stepRay.start.position - Vector3.down * hit.distance, Vector2.left * Mathf.Sign (transform.localScale.x), 0.1f, groundLayer).collider != null) {
            return 0;
        }
        return stepRay.distance - hit.distance;
    }

    void OnDrawGizmos () {
        Gizmos.color = new Color (0.2f, 0.5f, 1f);
        groundRay.Draw ();

        Gizmos.color = new Color (0.2f, 1f, 0.2f);
        stepRay.Draw ();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (((groundLayer >> collision.gameObject.layer) & 1) == 1) {
            Instantiate(GroundEffect, particleOrigin.transform.position, Quaternion.identity);
        }
        
        if (collision.gameObject.TryGetComponent<Platform> (out Platform platform)) {
            contactedPlatforms.Add (platform);
        }

        if (collision.gameObject.tag == "Respawn") {
            alive = false;
            StartCoroutine(DeathAnim());
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Interactive") {
            if (isActing) {
                actingObject.BroadcastMessage("HideHint");
            }
            else {
                actingObject = collision.gameObject;
                actingObject.BroadcastMessage("ShowHint");
            }
        }
    }

    private void Act(InputAction.CallbackContext value) {
        if (actingObject != null) {
            if (actingObject.GetComponent<Drone>() != null && control == false) {
                actingObject.SendMessage("Repair");
            }
            else actingObject.SendMessage("Act");
            isActing = true;
            actingObject.BroadcastMessage("HideHint");
        }
    }

    private void Put(InputAction.CallbackContext value) {
        if (actingObject != null && isActing) {
            actingObject.SendMessage("Stop");
            isActing = false;
        }
    }

    IEnumerator DeathAnim() {
        for (int i = 0; i < deathEffects.Length; i++) {
            Instantiate(deathEffects[i], transform.position, transform.rotation);
        }
        menuDrop.PlayDeathAnim();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent<Platform> (out Platform platform)) {
            contactedPlatforms.Remove (platform);
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Interactive" && !isActing) {
            actingObject = collision.gameObject;
            actingObject.BroadcastMessage("HideHint");
            actingObject = null;
        }
    }

    void PressJump (InputAction.CallbackContext value) => jumpPressTime = Time.time;
    void ReleaseJump (InputAction.CallbackContext value) => jumpPressTime = float.NegativeInfinity;

    // Movement inputs init
    private void OnEnable() {
        controls.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Direction.performed += OnDirectionPerformed;
        controls.Player.Direction.canceled += OnDirectionCanceled;
        controls.Player.Jump.performed += PressJump;
        controls.Player.Jump.canceled += ReleaseJump;
        controls.Player.Act.performed += Act;
        controls.Player.Put.performed += Put;
        controls.Player.Exit.performed += menuDrop.OpenPanel;
    }

    private void OnDisable() {
        controls.Disable();
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Direction.performed -= OnDirectionPerformed;
        controls.Player.Direction.canceled -= OnDirectionCanceled;
        controls.Player.Jump.performed -= PressJump;
        controls.Player.Jump.canceled -= ReleaseJump;
        controls.Player.Exit.performed -= menuDrop.OpenPanel;
    }
}