using System;

namespace Unity.GameCore
{
    public class XblTitleManagedStatistic
    {
        public string StatisticName { get; }
        public XblTitleManagedStatType StatisticType { get; }
        public double NumberValue { get; }
        public string StringValue { get; }

        internal XblTitleManagedStatistic(string statisticName, XblTitleManagedStatType statType, string stringValue, double numberValue)
        {
            this.StatisticName = statisticName;
            this.StatisticType = statType;
            this.StringValue = stringValue;
            this.NumberValue = numberValue;
        }

        public static Int32 Create(string statisticName, string statisticValue, out XblTitleManagedStatistic titleManagedStatistic)
        {
            titleManagedStatistic = new XblTitleManagedStatistic(statisticName, XblTitleManagedStatType.String, statisticValue, default(double));
            return HR.S_OK;
        }

        public static Int32 Create(string statisticName, double statisticValue, out XblTitleManagedStatistic titleManagedStatistic)
        {
            titleManagedStatistic = new XblTitleManagedStatistic(statisticName, XblTitleManagedStatType.Number, default(string), statisticValue);
            return HR.S_OK;
        }
    }
}
