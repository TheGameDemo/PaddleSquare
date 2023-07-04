using UnityEngine;
using static UnityEditor.PlayerSettings;

// In control of the gameplay loop and communicates with the ball and paddles
public class Game : MonoBehaviour
{
    // Configuration fields to connect the ball and both paddles
    [SerializeField]
    Ball ball;

    [SerializeField]
    Paddle bottomPaddle, topPaddle;

    [SerializeField, Min(0f)]
    Vector2 arenaExtents = new Vector2(10f, 10f);

    [SerializeField, Min(2)]
    int pointsToWin = 3;

    // Game awakens the ball should start a new game
    void Awake() => StartNewGame();

    void StartNewGame()
    {
        ball.StartNewGame();
        bottomPaddle.StartNewGame();
        topPaddle.StartNewGame();
    }

    void Update()
    {
        // Move both paddles
        bottomPaddle.Move(ball.Position.x, arenaExtents.x);
        topPaddle.Move(ball.Position.x, arenaExtents.x);

        // Ball move and then update its visualization.
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }

    void BounceYIfNeeded()
    {
        float yExtents = arenaExtents.y - ball.Extents;

        if (ball.Position.y > yExtents)
        {
            BounceY(yExtents, topPaddle, topPaddle);           
        }
        else if(ball.Position.y < -yExtents)
        {
            BounceY(-yExtents, bottomPaddle, bottomPaddle);
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
        ball.BounceY(boundary);

        if (defender.HitBall(bounceX, ball.Extents, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else if (attacker.ScorePoint(pointsToWin))
        {
            StartNewGame();
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

}
