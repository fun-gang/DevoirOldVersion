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
    private Vector2 movement = Vector2.zero;


    // Components
    private Animator anim;
    private Rigidbody2D rb = null;
    private MenuDrop menuDrop;
    private CinemachineVirtualCamera cmv;


    // Movement
    private bool isGrounded;
    public float playerSpeed = 0;
    [Tooltip ("How much player speed is reduced in the air")] public float airSpeedMod = 0.56f;
    public float jumpPower = 0;
    public LayerMask groundLayer;
    public static bool control; // Variable for cutscenes => Turn off/on movement ability
    private bool alive;
    private GameObject actingObject = null;
    private Animator deathAnim;

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
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        contactedPlatforms = new List<Platform> ();
        control = alive = true;
        menuDrop = GameObject.FindGameObjectWithTag("MenuDrop").GetComponent<MenuDrop>();
        deathAnim = GameObject.FindGameObjectWithTag("DeathAnim").GetComponent<Animator>();
        cmv = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cmv.Follow = transform;
    }

    private void FixedUpdate()
    {
        if (control && alive)
        {
            TryJump ();
            ApplyStep ();
            GroundCheck ();
            MovePlayer ();    
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
    }

    private void OnMovePerformed(InputAction.CallbackContext value) => movement = value.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext value) => movement = Vector2.zero;
    
    
    
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
            if (collision.gameObject.GetComponent<Gun>() != null) {
                actingObject = collision.gameObject;
            }
            else if (collision.gameObject.GetComponent<CarryingObject>() != null) {
                actingObject = collision.gameObject;
            }
            else actingObject = collision.gameObject;
        }
    }

    private void Act(InputAction.CallbackContext value) {
        if (actingObject != null) {
            actingObject.SendMessage("Act");
        }
    }

    private void Put(InputAction.CallbackContext value) {
        if (actingObject != null) {
            actingObject.SendMessage("Stop");
            actingObject = null;
        }
    }

    IEnumerator DeathAnim() {
        for (int i = 0; i < deathEffects.Length; i++) {
            Instantiate(deathEffects[i], transform.position, transform.rotation);
        }
        deathAnim.Play("Close");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent<Platform> (out Platform platform)) {
            contactedPlatforms.Remove (platform);
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Interactive") {
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
        controls.Player.Jump.performed -= PressJump;
        controls.Player.Jump.canceled -= ReleaseJump;
        controls.Player.Exit.performed -= menuDrop.OpenPanel;
    }
}