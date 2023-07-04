using UnityEngine;

public class LivelyCamera : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        jostleStrength = 40f,
        pushStrength = 1f;

    Vector3 velocity;

    public void JostleY() => velocity.y += jostleStrength;

    public void PushXZ(Vector2 impulse)
    {
        velocity.x += pushStrength * impulse.x;
        velocity.z += pushStrength * impulse.y;
    }

    void LateUpdate()
    {
        transform.localPosition += velocity * Time.deltaTime;
    }
}