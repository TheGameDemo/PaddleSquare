using UnityEngine;

public class Ball : MonoBehaviour 
{
    // In order to move Ball needs to keep track of both its position and its velocity.
    // As this is effectively a 2D game we'll use Vector2 fields for this.
    [SerializeField, Min(0f)]
    float
        maxXSpeed = 20f,
        startXSpeed = 8f,
        constantYSpeed = 10f,
        extents = 0.5f;         // A proper bounce happens when the ball's edge touches a boundary, not its center.
                                // So we need to know the ball's size.

    Vector2 position, velocity;

    // We might end up making various adjustments to the ball's position and velocity each update,
    // so let's not set its Transform.localPosition all the time.
    public void UpdateVisualization() => transform.localPosition = new Vector3(position.x, 0f, position.y);

    // We won't make the ball won't move on its own,
    // but instead make it perform standard movement via a public Move method.
    public void Move() => position += velocity * Time.deltaTime;

    // Sets things up for a new game.
    public void StartNewGame()
    {
        // The ball starts at the center of the arena.
        position = Vector2.zero;

        // Updates its visualization to match.
        UpdateVisualization();

        // Uses the configured velocity.
        // Make the velocity's Y component negative so it moves toward the player first.
        velocity = new Vector2(startXSpeed, -constantYSpeed);

        gameObject.SetActive(true);
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }

    // Bounce in a single dimension for a given boundary.
    // The final position is simply equal to double the boundary minus the current position.
    // Also, the velocity in that dimension flips.
    // The position and velocity in the other dimension are unaffected.
    public void BounceX(float boundary)
    {
        position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
    }

    public void BounceY(float boundary)
    {
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
    }

    // The ball itself won't decide when to bounce, so its extents and position must be publicly accessible.
    public float Extents => extents;

    public Vector2 Position => position;

    // To find the exact moment of the bounce the ball's velocity must be known.
    public Vector2 Velocity => velocity;

    void Awake() => gameObject.SetActive(false);

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
    }
}