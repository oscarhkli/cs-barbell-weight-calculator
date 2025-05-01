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
        if (requiredSideWeight < minPlate)
        {
            throw new ArgumentException("requiredSideWeight must be greater than or equal to minPlate");
        }
        
        // To avoid handling double, scale all the number with the minPlate
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
        
        for (var weight = 1; weight <= sideWeight; weight++)
        {
            foreach (var plate in plates)
            {
                if (plate > weight || dp[weight - plate] == Int32.MaxValue)
                {
                    continue;
                }

                if (dp[weight - plate] + 1 < dp[weight])
                {
                    dp[weight] = dp[weight - plate] + 1;
                    prev[weight] = plate;
                }
                
            }
        }
        
        if (dp[sideWeight] == Int32.MaxValue)
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