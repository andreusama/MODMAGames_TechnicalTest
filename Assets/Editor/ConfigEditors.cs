using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;

static class ConfigEditorUtil
{
    public static void MarkDirty(Object obj)
    {
        if (obj == null) return;
        Undo.RecordObject(obj, "Apply Config");
        EditorUtility.SetDirty(obj);
    }

    public static void MarkSceneDirty()
    {
        EditorSceneManager.MarkAllScenesDirty();
    }

    public static bool GetLiveKey(Object config) =>
        EditorPrefs.GetBool(GetKey(config), false);

    public static void SetLiveKey(Object config, bool value) =>
        EditorPrefs.SetBool(GetKey(config), value);

    private static string GetKey(Object config)
    {
        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(config));
        return $"CONFIG_LIVE_UPDATE_{guid}";
    }
}

// PlayerMotorConfig -> PlayerMotor
[CustomEditor(typeof(PlayerMotorConfig))]
public class PlayerMotorConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (PlayerMotorConfig)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(PlayerMotorConfig cfg)
    {
        var motors = Object.FindObjectsByType<PlayerMotor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var motor in motors)
        {
            var som = new SerializedObject(motor);
            var p = som.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                motor.MoveSpeed = cfg.MoveSpeed;
                motor.SmoothTime = cfg.SmoothTime;
                motor.SlideFriction = cfg.SlideFriction;
                ConfigEditorUtil.MarkDirty(motor);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }
}

// BalloonGunConfiguration -> BalloonGunSkill
[CustomEditor(typeof(BalloonGunConfiguration))]
public class BalloonGunConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (BalloonGunConfiguration)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(BalloonGunConfiguration cfg)
    {
        var skills = Object.FindObjectsByType<BalloonGunSkill>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var skill in skills)
        {
            var sos = new SerializedObject(skill);
            var p = sos.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                skill.WaterBalloonPrefab = cfg.WaterBalloonPrefab;
                skill.ExplosionDelay = cfg.ExplosionDelay;
                skill.Cooldown = cfg.Cooldown;
                skill.ExplosionRadius = cfg.ExplosionRadius;
                skill.TargetLayers = cfg.TargetLayers;

                skill.MinRange = cfg.MinRange;
                skill.MaxRange = cfg.MaxRange;
                skill.MaxHeight = cfg.MaxHeight;
                skill.AimingSpeedPercent = cfg.AimingSpeedPercent;

                // Flight/curve
                SetPrivateFloat(skill, "m_FlightTime", cfg.FlightTime);
                skill.UseAnimationCurve = cfg.UseAnimationCurve;
                skill.VelocityCurve = cfg.VelocityCurve;

                ConfigEditorUtil.MarkDirty(skill);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }

    private static void SetPrivateFloat(object obj, string field, float value)
    {
        var f = obj.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (f != null && f.FieldType == typeof(float)) f.SetValue(obj, value);
    }
}

// DashConfig -> DashSkill
[CustomEditor(typeof(DashConfig))]
public class DashConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (DashConfig)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(DashConfig cfg)
    {
        var skills = Object.FindObjectsByType<DashSkill>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var skill in skills)
        {
            var sos = new SerializedObject(skill);
            var p = sos.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                skill.DashDistance = cfg.DashDistance;
                skill.DashCooldown = cfg.DashCooldown;
                skill.DashDuration = cfg.DashDuration;
                skill.UseDashCurve = cfg.UseDashCurve;
                skill.DashCurve = cfg.DashCurve;
                // SafeOffset/IgnoreLayerName se leen en tiempo de dash (raycast), no hace falta tocar nada más
                ConfigEditorUtil.MarkDirty(skill);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }
}

// EnemyBaseConfig -> EnemyAI
[CustomEditor(typeof(EnemyBaseConfig))]
public class EnemyBaseConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (EnemyBaseConfig)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(EnemyBaseConfig cfg)
    {
        var ais = Object.FindObjectsByType<EnemyAI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var ai in ais)
        {
            var soAI = new SerializedObject(ai);
            var p = soAI.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                ai.Damage = cfg.Damage;
                ai.AttackRange = cfg.AttackRange;
                ai.AttackCooldown = cfg.AttackCooldown;
                ConfigEditorUtil.MarkDirty(ai);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }
}

// EnemyConfig -> Enemy
[CustomEditor(typeof(EnemyConfig))]
public class EnemyConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (EnemyConfig)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(EnemyConfig cfg)
    {
        var enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            var soe = new SerializedObject(e);
            var p = soe.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                e.CleanableLayers = cfg.CleanableLayers;
                e.CleanRadius = cfg.CleanRadius;
                // Al morir usa m_DestroyDelay y MaxHealth actuales:
                SetPrivateFloat(e, "m_DestroyDelay", cfg.DestroyDelay);
                SetPrivateFloat(e, "m_CurrentHealth", Mathf.Min(GetPrivateFloat(e, "m_CurrentHealth"), cfg.MaxHealth));
                SetPrivateFloat(e, "m_IsAlive", 1f); // noop: solo por coherencia
                e.GetType().GetField("MaxHealth").SetValue(e, cfg.MaxHealth);
                ConfigEditorUtil.MarkDirty(e);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }

    private static float GetPrivateFloat(object obj, string field)
    {
        var f = obj.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (f != null && f.FieldType == typeof(float)) return (float)f.GetValue(obj);
        return 0f;
    }
    private static void SetPrivateFloat(object obj, string field, float value)
    {
        var f = obj.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (f != null && f.FieldType == typeof(float)) f.SetValue(obj, value);
    }
}

// WettableEnemyConfig -> WettableEnemy
[CustomEditor(typeof(WettableEnemyConfig))]
public class WettableEnemyConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (WettableEnemyConfig)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(WettableEnemyConfig cfg)
    {
        var objs = Object.FindObjectsByType<WettableEnemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var obj in objs)
        {
            var soo = new SerializedObject(obj);
            var p = soo.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                obj.MinSpeedPercent = cfg.MinSpeedPercent;
                obj.DotsToSpawn = cfg.DotsToSpawn;
                obj.DestroyDelay = cfg.DestroyDelay;
                obj.MinDotDistance = cfg.MinDotDistance;
                obj.ExplosionRadius = cfg.ExplosionRadius;
                ConfigEditorUtil.MarkDirty(obj);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }
}

// AutoShotConfiguration -> AutoShootSkill
[CustomEditor(typeof(AutoShotConfiguration))]
public class AutoShotConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool changed = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        var so = (AutoShotConfiguration)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Aplicar a la escena", GUILayout.Height(24)))
            {
                ApplyToScene(so);
            }

            bool live = ConfigEditorUtil.GetLiveKey(so);
            bool newLive = GUILayout.Toggle(live, "Live Update", "Button", GUILayout.Height(24));
            if (newLive != live) ConfigEditorUtil.SetLiveKey(so, newLive);
        }

        if (changed && ConfigEditorUtil.GetLiveKey(so))
        {
            ApplyToScene(so);
        }
    }

    private static void ApplyToScene(AutoShotConfiguration cfg)
    {
        var skills = Object.FindObjectsByType<AutoShootSkill>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var s in skills)
        {
            var sos = new SerializedObject(s);
            var p = sos.FindProperty("m_Config");
            if (p != null && p.objectReferenceValue == cfg)
            {
                s.MinInterval = cfg.MinInterval;
                s.MaxInterval = cfg.MaxInterval;
                s.AimTime = cfg.AimTime;
                s.ValidGroundLayers = cfg.ValidGroundLayers;
                ConfigEditorUtil.MarkDirty(s);
            }
        }
        ConfigEditorUtil.MarkSceneDirty();
    }
}