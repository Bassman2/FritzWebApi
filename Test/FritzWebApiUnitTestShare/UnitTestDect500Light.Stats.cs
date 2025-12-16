namespace FritzWebApiUnitTest;

public partial class UnitTestDect500Light : UnitTestBase
{
    [TestMethod]
    public async Task TestMethodGetBasicDeviceStatsAsync()
    {
        DeviceStats? stats;

        using (HomeAutomation client = new HomeAutomation(TestSettings.Login, TestSettings.Password))
        {
            stats = await client.GetBasicDeviceStatsAsync(testDevice!.Ain);
        }

        Assert.IsNotNull(stats, "stats");
        Assert.IsEmpty(stats.Temperature!, "Temperature");
        Assert.IsEmpty(stats.Voltage!, "Voltage");
        Assert.IsEmpty(stats.Power!, "Power");
        Assert.IsEmpty(stats.Energy!, "Energy");
        Assert.IsEmpty(stats.Humidity!, "Humidity");
    }
}
   
            