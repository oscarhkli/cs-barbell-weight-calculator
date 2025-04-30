namespace BarbellWeightCalculator;

public class BarbellWeightService
{

    private static readonly double[] Plates = [1.25, 2.5, 5, 10, 20, 25];
    
    public Dictionary<double, int> GetSidePlates(double totalWeight, int barWeight)
    {
        if (totalWeight < barWeight)
        {
            throw new ArgumentException("requiredWeight must be greater than or equal to barWeight");
        }
        
        var requiredSideWeight = (totalWeight - barWeight) / 2;
        if (requiredSideWeight.Equals(0))
        {
            return new Dictionary<double, int>();
        }
        
        var minPlate = Plates.Min();
        var scale = 1 / minPlate;
        var scaledPlates = new int[Plates.Length];
        for (var i = 0; i < Plates.Length; i++)
        {
            scaledPlates[i] = Convert.ToInt32(Plates[i] * scale);
        }
        var scaledRequiredSideWeight = Convert.ToInt32(requiredSideWeight * scale);
        
        var scaledSidePlates = GetScaledSidePlates(scaledPlates, scaledRequiredSideWeight);
        
        var results = new Dictionary<double, int>();
        foreach (var scaledSidePlate in scaledSidePlates)
        {
            var normalPlate = scaledSidePlate.Key / scale;
            results[normalPlate] = scaledSidePlate.Value;
        }
        return results;
    }

    internal virtual Dictionary<int, int> GetScaledSidePlates(int[] plates, int sideWeight)
    {
        // Coin Change      
        var dp = new int[sideWeight + 1];
        var prev = new int[sideWeight + 1];
        Array.Fill(dp, Int32.MaxValue);
        dp[0] = 0;
        Array.Fill(prev, -1);
        
        for (var i = 1; i <= sideWeight; i++)
        {
            foreach (var plate in plates)
            {
                if (plate > i || dp[i - plate] == Int32.MaxValue)
                {
                    continue;
                }

                if (dp[i - plate] + 1 < dp[i])
                {
                    dp[i] = dp[i - plate] + 1;
                    prev[i] = plate;
                }
                
            }
        }

        if (dp[sideWeight] == -1)
        {
            return new Dictionary<int, int>();
        }
        
        var curr = sideWeight;
        var usage = new Dictionary<int, int>();
        while (curr > 0)
        {
            var plate = prev[curr];
            if (usage.TryGetValue(plate, out var val))
            {
                usage[plate] = val + 1;
            }
            else
            {
                usage[plate] = 1;
            }
            curr -= plate;
        }
        return usage;
    }
}