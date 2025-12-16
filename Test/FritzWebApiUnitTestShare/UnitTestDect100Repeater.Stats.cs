namespace FritzWebApiUnitTest;

public partial class UnitTestDect100Repeater : UnitTestBase
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
        Assert.IsNotEmpty(stats!.Temperature!, "Temperature");
        Assert.IsEmpty(stats.Voltage!, "Voltage");
        Assert.IsNotEmpty(stats!.Power!, "Power");
        Assert.IsNotEmpty(stats!.Energy!, "Energy");
        Assert.IsNotEmpty(stats!.Humidity!, "Humidity");
    }
}
   
            