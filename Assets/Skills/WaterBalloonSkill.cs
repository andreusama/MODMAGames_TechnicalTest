using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterBalloonSkill : Skill
{
    [Header("Water Balloon Settings")]
    public GameObject WaterBalloonPrefab;
    public float Damage = 25f;
    public float ExplosionDelay = 1.5f;
    public float Cooldown = 2f;
    public float ScatterForce = 10f;
    public float ExplosionRadius = 2.5f;
    public LayerMask TargetLayers;

    [Header("Aiming Settings")]
    public float MinRange = 2f;
    public float MaxRange = 10f;
    public float MaxHeight = 2f;

    private Transform m_SpawnPoint;
    private CircleDrawer m_CircleDrawer;

    // Guarda el último input válido
    private Vector2 m_LastAimInput = Vector2.zero;

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
        InitCooldown(Cooldown);
        m_SpawnPoint = m_PlayerMotor.transform;
        m_CircleDrawer = m_PlayerMotor.GetComponentInChildren<CircleDrawer>();
        if (m_CircleDrawer == null)
            Debug.LogWarning("CircleDrawer no encontrado en el jugador.");
    }

    public override void BindInput(InputMap actions)
    {
        actions.Player.ThrowBalloon.performed += OnAimingPerformed;
        actions.Player.ThrowBalloon.canceled += OnAimingCanceled;
        actions.Player.ThrowBalloon.Enable();
    }

    public override void UnbindInput(InputMap actions)
    {
        actions.Player.ThrowBalloon.performed -= OnAimingPerformed;
        actions.Player.ThrowBalloon.canceled -= OnAimingCanceled;
        actions.Player.ThrowBalloon.Disable();
    }

    // Guarda el último input válido y actualiza la visualización
    public void OnAimingPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.01f)
            m_LastAimInput = input;

        UpdateAimingVisual(input);
    }

    // Cuando se cancela, lanza el globo usando el último input válido
    public void OnAimingCanceled(InputAction.CallbackContext ctx)
    {
        if (m_LastAimInput.sqrMagnitude > 0.01f)
        {
            ThrowWaterBalloon(m_LastAimInput);
            m_LastAimInput = Vector2.zero;
        }
        m_CircleDrawer.Hide();
    }

    // Visualización de la parábola y círculo
    public void UpdateAimingVisual(Vector2 input)
    {
        if (m_CircleDrawer == null)
            return;

        if (input.sqrMagnitude < 0.01f)
        {
            m_CircleDrawer.Hide();
            return;
        }

        Vector3 direction = new Vector3(input.x, 0, input.y).normalized;
        float range = Mathf.Lerp(MinRange, MaxRange, input.magnitude);
        Vector3 target = m_SpawnPoint.position + direction * range;

        m_CircleDrawer.DrawCircle(target, ExplosionRadius);
        m_CircleDrawer.DrawParabola(
            m_SpawnPoint.position,
            ParabolicCalculator.CalculateLaunchVelocity(m_SpawnPoint.position, target, MaxHeight, Mathf.Abs(Physics.gravity.y)),
            Mathf.Abs(Physics.gravity.y),
            2f // Puedes ajustar el tiempo máximo de la parábola
        );
    }

    // Lanza el globo usando el input guardado
    public void ThrowWaterBalloon(Vector2 aimInput)
    {
        SetCooldown(Cooldown);
        if (!IsCooldownReady)
            return;

        Vector3 direction = new Vector3(aimInput.x, 0, aimInput.y).normalized;
        float range = Mathf.Lerp(MinRange, MaxRange, aimInput.magnitude);
        Vector3 target = m_SpawnPoint.position + direction * range;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float maxHeight = m_SpawnPoint.position.y + MaxHeight;
        Vector3 launchVelocity = ParabolicCalculator.CalculateLaunchVelocity(
            m_SpawnPoint.position, target, maxHeight, gravity
        );

        GameObject balloonObj = GameObject.Instantiate(WaterBalloonPrefab, m_SpawnPoint.position, Quaternion.identity);
        var balloon = balloonObj.GetComponent<WaterBalloon>();
        if (balloon != null)
        {
            balloon.Damage = Damage;
            balloon.ExplosionDelay = ExplosionDelay;
            balloon.ExplosionRadius = ExplosionRadius;
            balloon.TargetLayers = TargetLayers;

            balloon.Throw(launchVelocity);
        }
        StartCooldown();
    }
}