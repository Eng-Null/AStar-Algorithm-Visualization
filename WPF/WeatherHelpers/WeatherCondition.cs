namespace WPF;

public static class WeatherCondition
{
    #region WeatherConditions Code Section

    public static async Task<Node[,]> AddWeatherConditionAsync(Node[,] NodeMap)
    {
        return await Task.Run(async () =>
        {
            foreach (var node in NodeMap)
            {
                if (!node.IsObstacle && !node.IsRoad)
                {
                    node.Condition = await RandomEnumValue();
                }
            }
            return NodeMap;
        });
    }

    public static async Task<Node[,]> RemoveWeatherConditionAsync(Node[,] NodeMap)
    {
        return await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                if (!node.IsObstacle && !node.IsRoad)
                {
                    node.Condition = ExtraCondition.Clear;
                }
            }
            return NodeMap;
        });
    }

    private static async Task<ExtraCondition> RandomEnumValue()
    {
        Dictionary<ExtraCondition, float> condition = new();
        condition.Add(ExtraCondition.Sunny, 0.8f);
        condition.Add(ExtraCondition.Cloudy, 0.3f);
        condition.Add(ExtraCondition.Hail, 0.2f);
        condition.Add(ExtraCondition.Rainy, 0.3f);
        condition.Add(ExtraCondition.HeavyRain, 0.2f);
        condition.Add(ExtraCondition.Lightning, 0.1f);
        condition.Add(ExtraCondition.LightningRainy, 0.1f);
        await Task.Delay(0);
        return condition.RandomElementByWeight(e => e.Value).Key;
    }

    #endregion WeatherConditions Code Section
}