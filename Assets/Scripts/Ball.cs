using UnityEngine;

public class Ball : MonoBehaviour {
    [SerializeField, Min(0f)]
    float
        constantXSpeed = 8f,
        constantYSpeed = 10f;

    Vector2 position, velocity;

    public void UpdateVisualization() => transform.localPosition = new Vector3(position.x, 0f, position.y);

    public void Move() => position += velocity * Time.deltaTime;

    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();
        velocity = new Vector2(constantXSpeed, -constantYSpeed);
    }

}