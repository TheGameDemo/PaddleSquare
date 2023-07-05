using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public static Vector2 inputVector;

    public static void SetInputVector(Vector2 vector)=>inputVector = vector;

    [SerializeField]
    TextMeshPro scoreText;

    int score;

    [SerializeField, Min(0f)]
    float
        minExtents = 4f,
        maxExtents = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f;

    float extents, targetingBias;

    [SerializeField]
    bool isAI;

    static readonly int
        emissionColorId = Shader.PropertyToID("_EmissionColor"),
        faceColorId = Shader.PropertyToID("_FaceColor"),
        timeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");

    [SerializeField]
    MeshRenderer goalRenderer;

    [SerializeField, ColorUsage(true, true)]
    Color goalColor = Color.white;

    Material goalMaterial, paddleMaterial, scoreMaterial;

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
        return x + inputVector.x * speed * Time.deltaTime;
    }

    public bool HitBall(float ballX, float ballExtents, out float hitFactor)
    {
        ChangeTargetingBias();

        // hitFactor describes where the ball hit relative to the paddle's center and extents.
        // In Pong this determines the angle at which the ball bounces off the paddle.
        hitFactor =
            (ballX - transform.localPosition.x)/
            (extents - ballExtents);

        bool success = -1f <= hitFactor && hitFactor <= 1f;
        if (success)
        {
            paddleMaterial.SetFloat(timeOfLastHitId, Time.time);
        }
        return success;
    }

    void SetScore(int newScore, float pointsToWin = 1000f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
        scoreMaterial.SetColor(faceColorId, goalColor * (newScore / pointsToWin));
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        goalMaterial.SetFloat(timeOfLastHitId, Time.time);
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }

    void SetExtents(float newExtents)
    {
        extents = newExtents;
        Vector3 s = transform.localScale;
        s.x = 2f * newExtents;
        transform.localScale = s;
    }

    void Awake()
    {
        inputVector = new Vector2(0, 0);
        goalMaterial = goalRenderer.material;
        goalMaterial.SetColor(emissionColorId, goalColor);
        paddleMaterial = GetComponent<MeshRenderer>().material;
        scoreMaterial = scoreText.fontMaterial;
        SetScore(0);
    }
}