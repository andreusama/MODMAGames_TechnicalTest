using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    // typedef struct XblTitleManagedStatistic
    // {
    //     _Field_z_ const char* statisticName;
    //     XblTitleManagedStatType statisticType;
    //     double numberValue;
    //     _Field_z_ const char* stringValue;
    // } XblTitleManagedStatistic;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XblTitleManagedStatistic
    {
        internal readonly UTF8StringPtr statisticName;
        internal readonly XblTitleManagedStatType statisticType;
        internal readonly double numberValue;
        internal readonly UTF8StringPtr stringValue;

        internal XblTitleManagedStatistic(Unity.GameCore.XblTitleManagedStatistic statistic, DisposableCollection disposableCollection)
        {
            this.statisticName = new UTF8StringPtr(statistic.StatisticName, disposableCollection);
            this.statisticType = statistic.StatisticType;
            this.numberValue = statistic.NumberValue;
            this.stringValue = new UTF8StringPtr(statistic.StringValue, disposableCollection);
        }
    }
}
