using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// In control of the gameplay loop and communicates with the ball and paddles
public class Game : MonoBehaviour
{
    [SerializeField]
    LivelyCamera livelyCamera;

    // Configuration fields to connect the ball and both paddles
    [SerializeField]
    Ball ball;

    [SerializeField]
    Paddle bottomPaddle, topPaddle;

    [SerializeField, Min(0f)]
    Vector2 arenaExtents = new Vector2(10f, 10f);

    [SerializeField, Min(2)]
    int pointsToWin = 3;

    [SerializeField]
    TextMeshPro countdownText;

    [SerializeField, Min(1f)]
    float newGameDelay = 3f;

    float countdownUntilNewGame;

    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;


   //public void Jump(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        Debug.Log("Jump! " + context.phase);
    //    }
    //}

    // Game awakens the ball should start a new game
    void Awake() => countdownUntilNewGame = newGameDelay;

    void StartNewGame()
    {
        //InitInput();

        ball.StartNewGame();
        bottomPaddle.StartNewGame();
        topPaddle.StartNewGame();
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    //private void InitInput()
    //{
    //    playerInput = GetComponent<PlayerInput>();

    //    playerInputActions = new PlayerInputActions();
    //    playerInputActions.Player.Enable();
    //    //playerInputActions.Player.Jump.performed += Jump;
    //}

    void Update()
    {
        Paddle.SetInputVector(playerInputActions.Player.Move.ReadValue<Vector2>());

        // Move both paddles
        bottomPaddle.Move(ball.Position.x, arenaExtents.x);
        topPaddle.Move(ball.Position.x, arenaExtents.x);

        if (countdownUntilNewGame <= 0f)
        {
            UpdateGame();
        }
        else
        {
            UpdateCountdown();
        }
    }

    void UpdateGame()
    {
        // Ball move and then update its visualization.
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }

    void UpdateCountdown()
    {
        countdownUntilNewGame -= Time.deltaTime;
        if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            if (displayValue < newGameDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }

    void BounceYIfNeeded()
    {
        float yExtents = arenaExtents.y - ball.Extents;

        if (ball.Position.y < -yExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            BounceY(-yExtents, bottomPaddle, topPaddle);
        }
        else if (ball.Position.y > yExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            BounceY(yExtents, topPaddle, bottomPaddle);
        }
    }

    void BounceY(float boundary, Paddle defender, Paddle attacker)
    {
        // How long ago the bounce happened
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        // Ball's X position when the bounce happened
        float bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;

        BounceXIfNeeded(bounceX);
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        livelyCamera.PushXZ(ball.Velocity);
        ball.BounceY(boundary);

        if (defender.HitBall(bounceX, ball.Extents, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else
        {
            livelyCamera.JostleY();
            if (attacker.ScorePoint(pointsToWin))
            {
                EndGame();
            }
        }
    }

    void BounceXIfNeeded(float x)
    {
        float xExtents = arenaExtents.x - ball.Extents;

        if (x < -xExtents)
        {
            ball.BounceX(-xExtents);
        }
        else if (x > xExtents)
        {
            ball.BounceX(xExtents);
        }
    }

    void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("GAME OVER");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
    }
}
