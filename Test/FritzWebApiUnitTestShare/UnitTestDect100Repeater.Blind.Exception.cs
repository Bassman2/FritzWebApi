namespace FritzWebApiUnitTest;

public partial class UnitTestDect100Repeater : UnitTestBase
{
    #region Blind

    [TestMethod]
    public async Task TestMethodSetBlindAsyncError()
    {
        using HomeAutomation client = new HomeAutomation(TestSettings.Login, TestSettings.Password);

        await Assert.ThrowsAsync<HttpRequestException>(async () => 
            await client.SetBlindAsync(testDevice!.Ain, Target.Stop)
        );
        
    }

    #endregion
}
   
            