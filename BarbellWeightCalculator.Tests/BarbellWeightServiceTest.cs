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
                .With.Message.Contain("requiredWeight must be greater than or equal to barWeight"));
        sBarbellWeightService.Verify(x => x.GetScaledSidePlates(It.IsAny<int[]>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test(Description = "Given requiredWeight equals to barWeight, When GetWeight, Then can get 0")]
    public void ShouldGetZeroForRequiredWeightEqualsToBarWeight()
    {
        var sBarbellWeightService = new Mock<BarbellWeightService>() { CallBase = true };
        var requiredWeight = 20;
        var barWeight = 20;

        var actual = sBarbellWeightService.Object.GetSidePlates(requiredWeight, barWeight);

        Assert.That(actual, Is.Empty);
        sBarbellWeightService.Verify(x => x.GetScaledSidePlates(It.IsAny<int[]>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test(Description =
        "Given requiredWeight and barWeight, When GetWeight, Then can get correct side plates")]
    public void ShouldGetSidePlates()
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
}