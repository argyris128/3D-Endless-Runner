using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject GameOverMenu;
    public GameObject GameScore;
    public GameObject PauseMenu;

    private SwipeInput swipeInput;
    private PlayerControls controls;
    [SerializeField] private Rigidbody rb;
    private Coroutine moveCoroutine = null;
    private Coroutine crouchCoroutine = null;

    public float moveDuration = 0.2f;
    public float crouchDuration = 0.2f;
    public float jumpForce = 7f;
    public Transform[] Lanes;
    private bool isGrounded = true;
    public FloatingText scoreFloatingText;
    private float moveElapsed = 0f;

    private int currLane;
    public int CurrLane {   // 0 = left, 1 = middle, 2 = right
        get => currLane;
        set {
            currLane = Mathf.Clamp(value, 0, 2);
        }
    } 

    private enum Action
    {
        Left, Right
    }


    void Awake()
    {
        GameOverMenu.SetActive(false);
        GameScore.SetActive(true);
        PauseMenu.SetActive(false);

        swipeInput = GetComponent<SwipeInput>();

        Physics.gravity = new Vector3(0, -20f, 0);
        controls = new PlayerControls();

        if(PlayerPrefs.HasKey("CurrentLane"))
            CurrLane = PlayerPrefs.GetInt("CurrentLane");
        else
            CurrLane = 1;

        transform.position = Lanes[CurrLane].position; 
    }

    void Start()
    {
        GameManager.Instance.LoadGame();

        controls.Player.Move.performed += ctx =>
        {
            HandleMovement(ctx.ReadValue<Vector2>());
        };

        swipeInput.OnMove += direction =>
        {
            HandleMovement(direction);
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        foreach(Transform lane in Lanes)
        {
            lane.position = new Vector3(lane.position.x, transform.position.y, lane.position.z);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(GameManager.Instance.GameIsRunning)
                PauseGame();
            else
                UnpauseGame();
        }
    }

    private void HandleMovement(Vector2 input)
    {
        if(GameManager.Instance.GameIsRunning)
        {
            if (input.y > 0) {
                Jump();
            }

            if (input.y < 0) {
                crouchCoroutine ??= StartCoroutine(Crouch());
            }

            if (input.x < 0) {
                if(CurrLane > 0)
                {
                    if(moveCoroutine == null)
                        moveCoroutine = StartCoroutine(MoveAnimation(GetNextPosition(Action.Left)));
                    else if(moveElapsed > moveDuration * 0.6) {
                        StopCoroutine(moveCoroutine);
                        moveCoroutine = StartCoroutine(MoveAnimation(GetNextPosition(Action.Left)));
                    }
                }
                    
            }        

            if (input.x > 0) {
                if(CurrLane < 2) {
                    if(moveCoroutine == null)
                        moveCoroutine = StartCoroutine(MoveAnimation(GetNextPosition(Action.Right)));
                    else if(moveElapsed > moveDuration * 0.6) {
                        StopCoroutine(moveCoroutine);
                        moveCoroutine = StartCoroutine(MoveAnimation(GetNextPosition(Action.Right)));
                    }
                }
            }
        }
    }

    private IEnumerator Crouch()
    {
        rb.AddForce(Vector3.down * 1000f, ForceMode.Acceleration);
        Vector3 targetScale = new Vector3(1f, 0.5f, 1f);
        StartCoroutine(CrouchAnimation(Vector3.one, targetScale));

        yield return new WaitForSeconds(0.7f);

        StartCoroutine(CrouchAnimation(transform.localScale, Vector3.one));
        rb.AddForce(Vector3.down * 1000f, ForceMode.Acceleration);
        crouchCoroutine = null;     
    }

    private IEnumerator CrouchAnimation(Vector3 startScale, Vector3 targetScale)
    {
        float time = 0f;

        while(time < crouchDuration)
        {
            time += Time.deltaTime;

            float t = time / moveDuration;
            t = Mathf.SmoothStep(0, 1, t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator MoveAnimation(Vector3 targetPosition) {
        Vector3 startPosition = transform.position;
        moveElapsed = 0f;

        while(moveElapsed < moveDuration) {
            moveElapsed += Time.deltaTime;

            float t = moveElapsed / moveDuration;
            t = Mathf.SmoothStep(0, 1, t);

            targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);

            rb.MovePosition(newPosition);
            
            yield return null; 
        }

        moveElapsed = 0f;
        transform.position = targetPosition;
        moveCoroutine = null;
    }

    private void Jump() {
        if(isGrounded == true)
        {
            StartCoroutine(CrouchAnimation(transform.localScale, Vector3.one));

            if(crouchCoroutine != null) {
                StopCoroutine(crouchCoroutine);
            }

            crouchCoroutine = null;

            Vector3 velocity = rb.linearVelocity;
            velocity.y = jumpForce;
            rb.linearVelocity = velocity;
        }   
    }

    private Vector3 GetNextPosition(Action action) {
        if(action == Action.Left) {
            CurrLane--;
        } else if(action == Action.Right) {
            CurrLane++;
        }

        return Lanes[CurrLane].position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if(other.gameObject.CompareTag("Coin")) {
            GameManager.Instance.RemoveCurrObject(other.gameObject);
            Destroy(other.gameObject);
            Score.score += 5;
            scoreFloatingText.Enable(5);
        }

        if(other.gameObject.CompareTag("Trophy")) {
            GameManager.Instance.RemoveCurrObject(other.gameObject);
            Destroy(other.gameObject);
            Score.score += 20;
            scoreFloatingText.Enable(20);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void PlayerDeath()
    {
        Destroy(gameObject);
        GameManager.Instance.GameIsRunning = false;
        GameScore.SetActive(false);
        GameOverMenu.SetActive(true);
        GameOverMenu.transform.Find("ScoreText").GetComponent<TMP_Text>().text = "Score: " + Score.score;
    }

    public void PauseGame()
    {
        GameManager.Instance.GameIsRunning = false;
        PauseMenu.SetActive(true);
    }

    public void UnpauseGame()
    {
        GameManager.Instance.GameIsRunning = true;
        PauseMenu.SetActive(false);
    }

}