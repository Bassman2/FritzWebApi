namespace FritzWebApiUnitTest
{
    public partial class UnitTestDect100Repeater : UnitTestBase
    {
        #region Energy Meter

        [TestMethod]
        //// [ExpectedHttpRequestException(HttpStatusCode.InternalServerError)]
        public async Task TestMethodEnergyAsyncError()
        {
            double? energy;

            using HomeAutomation client = new HomeAutomation(TestSettings.Login, TestSettings.Password);

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
                energy = await client.GetSwitchEnergyAsync(testDevice!.Ain)
            );
        }

        [TestMethod]
        //// [ExpectedHttpRequestException(HttpStatusCode.InternalServerError)]
        public async Task TestMethodPowerAsyncError()
        {
            double? power;

            using HomeAutomation client = new HomeAutomation(TestSettings.Login, TestSettings.Password);

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
                power = await client.GetSwitchPowerAsync(testDevice!.Ain)
            );
        }

        #endregion
    }
}