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

    internal virtual Dictionary<int, int> GetScaledSidePlates(int[] plates, int requiredWeight)
    {
        // Coin Change      
        
        return new Dictionary<int, int>();
    }
}