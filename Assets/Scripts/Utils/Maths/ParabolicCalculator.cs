using UnityEngine;

public static class ParabolicCalculator
{
    /// <summary>
    /// Calculates the initial velocity to launch a projectile from 'origin' to 'target' with a desired maximum height.
    /// </summary>
    public static Vector3 CalculateLaunchVelocity(Vector3 origin, Vector3 target, float maxHeight, float gravity)
    {
        Vector3 displacement = target - origin;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float timeUp = Mathf.Sqrt(2 * (maxHeight - origin.y) / gravity);
        float timeDown = Mathf.Sqrt(2 * (maxHeight - target.y) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * (maxHeight - origin.y));
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }

    // Additional method to calculate the target and launch velocity from joystick input
    public static Vector3 CalculateTargetAndLaunchVelocity(Transform spawnPoint, Vector2 joystickInput, float minRange, float maxRange, Rigidbody balloonRigidbody)
    {
        Vector3 direction = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
        float range = Mathf.Lerp(minRange, maxRange, joystickInput.magnitude);
        Vector3 target = spawnPoint.position + direction * range;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float maxHeight = spawnPoint.position.y + 2f; // You can adjust max height

        Vector3 launchVelocity = CalculateLaunchVelocity(spawnPoint.position, target, maxHeight, gravity);

        balloonRigidbody.linearVelocity = launchVelocity;

        return target; // Return calculated target
    }

    public static Vector3 CalculateLaunchVelocityByTime(Vector3 start, Vector3 target, float flightTime, float gravity)
    {
        Vector3 toTarget = target - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float yOffset = toTarget.y;
        float xzDistance = toTargetXZ.magnitude;

        float t = flightTime;
        float v0y = (yOffset + 0.5f * Mathf.Abs(Physics.gravity.y) * t * t) / t;
        float v0xz = xzDistance / t;

        Vector3 result = toTargetXZ.normalized * v0xz + Vector3.up * v0y;

        return result;
    }

    /// <summary>
    /// Parabolic interpolation between two points (you can adjust extra height if needed)
    /// </summary>
    public static Vector3 ParabolicLerp(Vector3 start, Vector3 end, float t, float extraHeight = 3f)
    {
        float height = Mathf.Max(start.y, end.y) + extraHeight;
        Vector3 mid = Vector3.Lerp(start, end, t);
        float parabola = 4 * height * t * (1 - t);
        mid.y += parabola;
        return mid;
    }
}