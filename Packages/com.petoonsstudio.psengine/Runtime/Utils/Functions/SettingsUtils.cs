using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsUtils
{
    public static bool CompareResolution(Resolution source, Resolution reference)
    {
        return source.width == reference.width && source.height == reference.height && source.refreshRateRatio.Equals(reference.refreshRateRatio);
    }
}