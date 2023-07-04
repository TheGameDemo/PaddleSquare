using UnityEngine;

public class LivelyCamera : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        springStrength = 100f,
        dampingStrength = 10f,
        jostleStrength = 40f,
        pushStrength = 1f;

    Vector3 anchorPosition, velocity;

    void Awake() => anchorPosition = transform.localPosition;

    public void JostleY() => velocity.y += jostleStrength;

    public void PushXZ(Vector2 impulse)
    {
        velocity.x += pushStrength * impulse.x;
        velocity.z += pushStrength * impulse.y;
    }

    void LateUpdate()
    {
        // We implement the spring by using the current displacement of the camera scaled by the spring strength as acceleration.
        // We also slow down movement via a negative acceleration equal to the current velocity scaled by the damping strength.
        Vector3 displacement = anchorPosition - transform.localPosition;
        Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
        velocity += acceleration * Time.deltaTime;
        transform.localPosition += velocity * Time.deltaTime;
    }
}