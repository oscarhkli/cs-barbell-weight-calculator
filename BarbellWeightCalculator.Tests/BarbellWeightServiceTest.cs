using Moq;

namespace BarbellWeightCalculator.Tests;

[TestFixture]
[TestOf(typeof(BarbellWeightService))]
public class BarbellWeightServiceTest
{
    [Test(Description =
        "Given requiredWeight is smaller than barWeight, When GetWeight, Then can throw exception")]
    public void ShouldGetCorrectWeight()
    {
        var sBarbellWeightService = new Mock<BarbellWeightService>() { CallBase = true };
        var requiredWeight = 10;
        var barWeight = 20;

        Assert.That(() => sBarbellWeightService.Object.GetSidePlates(requiredWeight, barWeight),
            Throws.TypeOf<ArgumentException>()
                .With.Message
                .Contain("requiredWeight must be greater than or equal to barWeight"));
        sBarbellWeightService.Verify(
            x => x.GetScaledSidePlates(It.IsAny<int[]>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test(Description =
        "Given requiredWeight equals to barWeight, When GetWeight, Then can get 0")]
    public void ShouldGetZeroForRequiredWeightEqualsToBarWeight()
    {
        var sBarbellWeightService = new Mock<BarbellWeightService>() { CallBase = true };
        var requiredWeight = 20;
        var barWeight = 20;

        var actual = sBarbellWeightService.Object.GetSidePlates(requiredWeight, barWeight);

        Assert.That(actual, Is.Empty);
        sBarbellWeightService.Verify(
            x => x.GetScaledSidePlates(It.IsAny<int[]>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test(Description =
        "Given requiredWeight and barWeight, When GetWeight, Then can get correct side plates")]
    public void ShouldGetSidePlatesByScaledPlates()
    {
        var sBarbellWeightService = new Mock<BarbellWeightService>() { CallBase = true };

        var requiredWeight = 25;
        var barWeight = 20;

        var scaledPlates = new[] { 1, 2, 4, 8, 16, 20 };
        var fakeScaledPlates = new Dictionary<int, int>
        {
            [2] = 1
        };
        sBarbellWeightService.Setup(x =>
                x.GetScaledSidePlates(It.Is<int[]>(arr => arr.SequenceEqual(scaledPlates)), 2))
            .Returns(fakeScaledPlates);

        var actual = sBarbellWeightService.Object.GetSidePlates(requiredWeight, barWeight);

        Assert.That(actual[2.5], Is.EqualTo(1));
    }

    private static IEnumerable<TestCaseData> InvalidWeightCases()
    {
        yield return new TestCaseData(
                10,
                20,
                typeof(ArgumentException),
                "requiredWeight must be greater than or equal to barWeight")
            .SetName("ShouldThrow_WhenRequiredSideWeightIsLessThanMinPlate");

        yield return new TestCaseData(
                21.5,
                20,
                typeof(ArgumentException),
                "requiredSideWeight must be greater than or equal to minPlate")
            .SetName("ShouldThrow_WhenRequiredSideWeightIsLessThanMinPlate");
    }

    [TestCaseSource(nameof(InvalidWeightCases))]
    public void ShouldThrowForInvalidWeight(double totalWeight, int barWeight,
        Type expectedException, string expectedMessage)
    {
        var barbellWeightService = new BarbellWeightService();

        Assert.That(() => barbellWeightService.GetSidePlates(totalWeight, barWeight),
            Throws.TypeOf(expectedException)
                .With.Message
                .Contain(expectedMessage));
    }

    private static IEnumerable<TestCaseData> SidePlatesCases()
    {
        yield return new TestCaseData(
            20,
            20,
            new Dictionary<double, int>()).SetName("ShouldGetSidePlates_WithWeight20_BarWeight20");

        yield return new TestCaseData(
            50,
            20,
            new Dictionary<double, int>
            {
                { 5, 1 },
                { 10, 1 }
            }
        ).SetName("ShouldGetSidePlates_WithWeight50_BarWeight50");

        yield return new TestCaseData(
            127.5,
            20,
            new Dictionary<double, int>
            {
                { 1.25, 1 },
                { 2.5, 1 },
                { 25, 2 }
            }
        ).SetName("ShouldGetSidePlates_WithWeight107.5_BarWeight10");
    }

    [TestCaseSource(nameof(SidePlatesCases))]
    public void ShouldGetSidePlates(double totalWeight, int barWeight,
        Dictionary<double, int> expected)
    {
        var barbellWeightService = new BarbellWeightService();

        var actual = barbellWeightService.GetSidePlates(totalWeight, barWeight);

        Assert.That(actual, Is.EqualTo(expected),
            $"Expected {FormatDictDouble(expected)}\nActual: {FormatDictDouble(actual)}");
    }

    private static string FormatDictDouble(Dictionary<double, int> dict)
    {
        return string.Join(", ", dict.Select(kv => $"{kv.Key}:{kv.Value}"));
    }

    private static IEnumerable<TestCaseData> ScaledSidePlatesCases()
    {
        yield return new TestCaseData(
            new[] { 1, 2, 4, 8, 16, 20 },
            2,
            new Dictionary<int, int> { { 2, 1 } }
        ).SetName("ShouldGetScaledSidePlates_WithWeight2");

        yield return new TestCaseData(
            new[] { 1, 2, 4, 8, 16, 20 },
            50,
            new Dictionary<int, int>
            {
                { 20, 2 },
                { 8, 1 },
                { 2, 1 }
            }
        ).SetName("ShouldGetScaledSidePlates_WithWeight50");

        yield return new TestCaseData(
            new[] { 3, 4, 5, 15, 20 },
            37,
            new Dictionary<int, int> { { 3, 1 }, { 4, 1 }, { 15, 2 } }
        ).SetName("ShouldGetScaledSidePlates_WithWeight37");

        yield return new TestCaseData(
            new[] { 3, 4, 5, 15, 20 },
            2,
            new Dictionary<int, int>()
        ).SetName("ShouldGetScaledSidePlates_WithWeightImpossible");
    }

    [TestCaseSource(nameof(ScaledSidePlatesCases))]
    public void ShouldGetScaledSidePlates(int[] plates, int sideWeight,
        Dictionary<int, int> expected)
    {
        var barbellWeightService = new BarbellWeightService();

        var actual = barbellWeightService.GetScaledSidePlates(plates, sideWeight);

        Assert.That(actual, Is.EqualTo(expected),
            $"Expected {FormatDictInt(expected)}\nActual: {FormatDictInt(actual)}");
    }

    private static string FormatDictInt(Dictionary<int, int> dict)
    {
        return string.Join(", ", dict.Select(kv => $"{kv.Key}:{kv.Value}"));
    }
}