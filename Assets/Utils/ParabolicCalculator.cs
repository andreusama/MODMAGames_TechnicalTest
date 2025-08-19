using UnityEngine;

public static class ParabolicCalculator
{
    /// <summary>
    /// Calcula la velocidad inicial para lanzar un proyectil desde 'origin' hacia 'target' con una altura máxima deseada.
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

    // Método adicional para calcular el objetivo y la velocidad de lanzamiento basada en la entrada del joystick
    public static Vector3 CalculateTargetAndLaunchVelocity(Transform spawnPoint, Vector2 joystickInput, float minRange, float maxRange, Rigidbody balloonRigidbody)
    {
        Vector3 direction = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
        float range = Mathf.Lerp(minRange, maxRange, joystickInput.magnitude);
        Vector3 target = spawnPoint.position + direction * range;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float maxHeight = spawnPoint.position.y + 2f; // Puedes ajustar la altura máxima

        Vector3 launchVelocity = CalculateLaunchVelocity(spawnPoint.position, target, maxHeight, gravity);

        balloonRigidbody.linearVelocity = launchVelocity;

        return target; // Retorna el objetivo calculado
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
}