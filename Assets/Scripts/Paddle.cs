using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    TextMeshPro scoreText;

    int score;

    [SerializeField, Min(0f)]
    float
        extents = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f;

    float targetingBias;

    [SerializeField]
    bool isAI;

    void ChangeTargetingBias() =>
    targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);

    // The same as Ball Move.
    // A target and arena extents both in the X dimension.
    public void Move(float target,float arenaExtents)
    {
        Vector3 p = transform.localPosition;
        p.x = isAI ? AdjustByAI(p.x, target) : AdjustByPlayer(p.x);
        float limit = arenaExtents - extents;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        transform.localPosition = p;
    }

    // Takes an X position and target and returns a new X.
    // This is a dumb reactive AI without any prediction, its difficulty only depends on its speed.
    float AdjustByAI(float x,float target)
    {
        target += targetingBias * extents;

        // If it is on the left side of the target it simply moves right at maximum speed until it matches the target,
        // otherwise it moves left in the same way.
        if (x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }

    float AdjustByPlayer(float x)
    {
        bool goRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool goLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        return x;
    }

    public bool HitBall(float ballX, float ballExtents, out float hitFactor)
    {
        ChangeTargetingBias();

        // hitFactor describes where the ball hit relative to the paddle's center and extents.
        // In Pong this determines the angle at which the ball bounces off the paddle.
        hitFactor =
            (ballX - transform.localPosition.x)/
            (extents - ballExtents);
        return -1 <= hitFactor && hitFactor <= 1f;
    }

    void SetScore(int newScore)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        SetScore(score + 1);
        return score >= pointsToWin;
    }
}