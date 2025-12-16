namespace FritzWebApi;

/// <summary>
/// AVM Home Automation service class.
/// </summary>
public sealed class HomeAutomation : JsonService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HomeAutomation"/> class using the specified store key and application name.
    /// </summary>
    /// <param name="storeKey">The key used to identify the credential store.</param>
    /// <param name="appName">The name of the application using the service.</param>
    public HomeAutomation(string storeKey, string appName) 
        : base(storeKey, appName, SourceGenerationContext.Default)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeAutomation"/> class using the specified host, authenticator, and application name.
    /// </summary>
    /// <param name="host">The URI of the home automation service host.</param>
    /// <param name="authenticator">The authenticator used for authentication, or <c>null</c> if not required.</param>
    /// <param name="appName">The name of the application using the service.</param>
    public HomeAutomation(Uri host, IAuthenticator? authenticator, string appName) 
        : base(host, authenticator, appName, SourceGenerationContext.Default)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeAutomation"/> class using the specified host URI and user credentials.
    /// </summary>
    /// <param name="host">The URI of the home automation service host.</param>
    /// <param name="login">The username or login used for authentication.</param>
    /// <param name="password">The password used for authentication.</param>
    /// <param name="appName">The name of the application using the service.</param>
    /// <remarks>
    /// This constructor creates a <see cref="FritzAuthenticator"/> from the provided credentials and delegates
    /// to <see cref="HomeAutomation(Uri, IAuthenticator?, string)"/>.
    /// </remarks>
    public HomeAutomation(Uri host, string login, string password, string appName)
        : this(host, new FritzAuthenticator(login, password), appName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeAutomation"/> class using the specified host (string) and user credentials.
    /// </summary>
    /// <param name="host">The host address (as a string) of the home automation service. This will be converted to a <see cref="Uri"/>.</param>
    /// <param name="login">The username or login used for authentication.</param>
    /// <param name="password">The password used for authentication.</param>
    /// <param name="appName">The name of the application using the service.</param>
    /// <remarks>
    /// This constructor converts <paramref name="host"/> to a <see cref="Uri"/>, creates a <see cref="FritzAuthenticator"/>
    /// from the provided credentials and delegates to <see cref="HomeAutomation(Uri, IAuthenticator?, string)"/>.
    /// </remarks>
    public HomeAutomation(string host, string login, string password, string appName)
        : this(new Uri(host), new FritzAuthenticator(login, password), appName)
    { }

    /// <summary>
    /// Configures the provided <see cref="HttpClient"/> instance with specific default headers required for API requests.
    /// This includes setting the User-Agent, Accept, and API version headers.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> to configure for GitHub API usage.</param>
    /// <param name="appName">The name of the application, used as the User-Agent header value.</param>
    protected override void InitializeClient(HttpClient client, string appName)
    {
        client.DefaultRequestHeaders.Add("User-Agent", appName);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    internal string? sessionId;

    /// <summary>
    /// Gets the URL used to test authentication for the HomeAutomation service.
    /// </summary>
    protected override string? AuthenticationTestUrl => BuildUrl("getdevicelistinfos");


    /// <summary>
    /// Special value for setting a device or actuator to "On" state (e.g., for switches or thermostats).
    /// </summary>
    public const int On = 254;

    /// <summary>
    /// Special value for setting a device or actuator to "Off" state (e.g., for switches or thermostats).
    /// </summary>
    public const int Off = 253;

    private string BuildUrl(string cmd) 
        => $"webservices/homeautoswitch.lua?switchcmd={cmd}&sid={this.sessionId}";
    
    private string BuildUrl(string cmd, string ain) 
        => $"webservices/homeautoswitch.lua?ain={ain}&switchcmd={cmd}&sid={this.sessionId}";

    private string BuildUrl(string cmd, string ain, string param) 
        => $"webservices/homeautoswitch.lua?ain={ain}&switchcmd={cmd}&sid={this.sessionId}&{param}";

    #region Public Methods

    /// <summary>
    /// Returns the AIN / MAC list of all known sockets.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<string[]?> GetSwitchListAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getswitchlist"), cancellationToken);
        return res!.SplitList();
    }

    /// <summary>
    /// Turns on the specified power socket or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the socket was successfully turned on, <c>false</c> if the operation failed, or <c>null</c> if the state is unknown.
    /// </returns>
    public async Task<bool?> SetSwitchOnAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("setswitchon", ain), cancellationToken);
        return res.ToBool();
    }

    /// <summary>
    /// Turns off the specified power socket or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the socket was successfully turned off, <c>false</c> if the operation failed, or <c>null</c> if the state is unknown.
    /// </returns>
    public async Task<bool?> SetSwitchOffAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("setswitchoff", ain), cancellationToken);
        return res.ToBool();
    }

    /// <summary>
    /// Toggles the state of the specified power socket or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the socket was successfully toggled, <c>false</c> if the operation failed, or <c>null</c> if the state is unknown.
    /// </returns>
    public async Task<bool?> SetSwitchToggleAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("setswitchtoggle", ain), cancellationToken);
        return res.ToBool();
    }

    /// <summary>
    /// Determines the current switching state of the specified power socket or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the socket is on, <c>false</c> if it is off, or <c>null</c> if the state is unknown.
    /// </returns>
    /// <remarks>
    /// If the connection is lost, the state may only change to <c>false</c> after a delay of several minutes.
    /// </remarks>
    public async Task<bool?> GetSwitchStateAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getswitchstate", ain), cancellationToken);
        return res.ToBool();
    }

    /// <summary>
    /// Determines whether the specified power socket or actuator is currently connected (present).
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the device is present, <c>false</c> if not, or <c>null</c> if the state is unknown.
    /// </returns>
    public async Task<bool?> GetSwitchPresentAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getswitchpresent", ain), cancellationToken);
        return res.ToBool();
    }

    /// <summary>
    /// Determines the current power consumption (in watts) of the specified power socket or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the current power in watts, or <c>null</c> if the value is unavailable.
    /// </returns>
    public async Task<double?> GetSwitchPowerAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getswitchpower", ain), cancellationToken);
        return res.ToPower();
    }

    /// <summary>
    /// Retrieves the total energy consumption (in watt-hours) measured by the specified power socket or actuator since commissioning or the last reset of energy statistics.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the total energy consumption in watt-hours, or <c>null</c> if the value is unavailable.
    /// </returns>
    public async Task<double?> GetSwitchEnergyAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getswitchenergy", ain), cancellationToken);
        return res.ToPower();
    }

    /// <summary>
    /// Retrieves the name of the specified power socket or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the name of the device, or <c>null</c> if the name is unavailable.
    /// </returns>
    public async Task<string?> GetSwitchNameAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getswitchname", ain), cancellationToken);
        return res?.TrimEnd();
    }

    /// <summary>
    /// Provides the basic information of all SmartHome devices.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<DeviceList?> GetDeviceListInfosAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await this.client!.GetStringAsync(BuildUrl("getdevicelistinfos"), cancellationToken);
        return res.XDeserialize<DeviceList>("devicelist");
    }

    /// <summary>
    /// Provides the basic information of all SmartHome devices as XML.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<XmlDocument?> GetDeviceListInfosXmlAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getdevicelistinfos"), cancellationToken);
        return res.ToXml();
    }

    /// <summary>
    /// Retrieves the latest measured temperature (in degrees Celsius) from the specified actuator or sensor.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the current temperature in degrees Celsius, or <c>null</c> if the value is unavailable.
    /// </returns>
    public async Task<double?> GetTemperatureAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gettemperature", ain), cancellationToken);
        return res.ToTemperature();
    }

    /// <summary>
    /// Retrieves the setpoint temperature currently configured for the specified radiator regulator (HKR).
    /// </summary>
    /// <param name="ain">Identification of the actuator or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the setpoint temperature in degrees Celsius.
    /// </returns>
    public async Task<double> GetHkrtSollAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gethkrtsoll", ain), cancellationToken);
        return res.ToHkrTemperature();
    }

    /// <summary>
    /// Retrieves the comfort temperature configured for the specified radiator regulator (HKR) timer.
    /// </summary>
    /// <param name="ain">Identification of the actuator or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the comfort temperature in degrees Celsius.
    /// </returns>
    public async Task<double> GetHkrKomfortAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gethkrkomfort", ain), cancellationToken);
        return res.ToHkrTemperature();
    }

    /// <summary>
    /// Retrieves the economy (lowering) temperature configured for the specified radiator regulator (HKR) timer.
    /// </summary>
    /// <param name="ain">Identification of the actuator or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the economy temperature in degrees Celsius.
    /// </returns>
    public async Task<double> GetHkrAbsenkAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gethkrabsenk", ain), cancellationToken);
        return res.ToHkrTemperature();
    }

    /// <summary>
    /// Sets the setpoint temperature for the specified radiator regulator (HKR).
    /// </summary>
    /// <param name="ain">Identification of the actuator or template (AIN/MAC).</param>
    /// <param name="temperature">
    /// Temperature value in degrees Celsius (8°C - 28°C), or use <see cref="On"/>/<see cref="Off"/> for special states.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the setpoint temperature in degrees Celsius after the operation.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="temperature"/> is not within the valid range (8–28), and is not <see cref="On"/> or <see cref="Off"/>.
    /// </exception>
    public async Task<double> SetHkrtSollAsync(string ain, double temperature, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        //ArgumentOutOfRangeException.ThrowIfLessThan(temperature, 8, nameof(temperature));
        //ArgumentOutOfRangeException.ThrowIfGreaterThan(temperature, 28, nameof(temperature));
        //ArgumentOutOfRangeException.ThrowIf(temperature != On && temperature != Off && (temperature < 8 || temperature > 28), nameof(temperature), "Temperature must be between 8 and 28, or use On/Off.");

        if ((temperature < 8 || temperature > 28) && (temperature != On) && (temperature != Off))
        {
            throw new ArgumentOutOfRangeException(nameof(temperature));
        }
        var res = await GetStringAsync(BuildUrl("sethkrtsoll", ain, $"param={temperature.ToHkrTemperature()}"), cancellationToken);
        return res.ToHkrTemperature();
    }

    /// <summary>
    /// Retrieves the basic statistics (such as temperature, voltage, and power) for the specified actuator or device.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a <see cref="DeviceStats"/> object containing the basic statistics, or <c>null</c> if the data is unavailable.
    /// </returns>
    public async Task<DeviceStats?> GetBasicDeviceStatsAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getbasicdevicestats", ain), cancellationToken);
        return res.XDeserialize<DeviceStats>("devicestats");
    }

    /// <summary>
    /// Retrieves the basic statistics (such as temperature, voltage, and power) for the specified actuator or device as an XML document.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an <see cref="XmlDocument"/> containing the basic statistics, or <c>null</c> if the data is unavailable.
    /// </returns>
    public async Task<XmlDocument?> GetBasicDeviceStatsXmlAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getbasicdevicestats", ain), cancellationToken);
        return res.ToXml();
    }

    /// <summary>
    /// Retrieves the basic information of all routines and triggers available in the Smart Home system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a <see cref="TriggerList"/> containing the routines and triggers information, or <c>null</c> if the data is unavailable.
    /// </returns>
    /// <remarks>Requires FRITZ!OS 7.39 or higher.</remarks>
    public async Task<TriggerList?> GetTriggerListInfosAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gettriggerlistinfos"), cancellationToken);
        return res.XDeserialize<TriggerList>("triggerlist");
    }

    /// <summary>
    /// Retrieves the basic information of all routines and triggers available in the Smart Home system as an XML document.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an <see cref="XmlDocument"/> containing the routines and triggers information, or <c>null</c> if the data is unavailable.
    /// </returns>
    /// <remarks>Requires FRITZ!OS 7.39 or higher.</remarks>
    public async Task<XmlDocument?> GetTriggerListInfosXmlAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gettriggerlistinfos"), cancellationToken);
        return res.ToXml();
    }

    /// <summary>
    /// Activates or deactivates the specified trigger in the Smart Home system.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="active">Set to <c>true</c> to activate the trigger, or <c>false</c> to deactivate it.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the trigger was successfully updated, <c>false</c> if the operation failed, or <c>null</c> if the state is unknown.
    /// </returns>
    /// <remarks>Requires FRITZ!OS 7.39 or higher.</remarks>
    public async Task<bool?> SetTriggerActiveAsync(string ain, bool active, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("settriggeractive", ain, $"active={(active ? "1" : "0")}"), cancellationToken);
        return res.ToBool();
    }

    /// <summary>
    /// Returns the basic information of all templates / templates.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<TemplateList?> GetTemplateListInfosAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gettemplatelistinfos"), cancellationToken);
        return res.XDeserialize<TemplateList>("templatelist");
    }

    /// <summary>
    /// Returns the basic information of all templates / templates as XML.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<XmlDocument?> GetTemplateListInfosXmlAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("gettemplatelistinfos"), cancellationToken);
        return res.ToXml();
    }

    /// <summary>
    /// Applies the specified template to the associated device or group.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    public async Task<int?> ApplyTemplateAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("applytemplate", ain), cancellationToken);
        return res.ToInt();
    }

    /// <summary>
    /// Switches a device, actuator, or lamp on, off, or toggles its state.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="onOff">
    /// The desired state to set:
    /// <list type="bullet">
    /// <item><description><see cref="OnOff.On"/> to switch on</description></item>
    /// <item><description><see cref="OnOff.Off"/> to switch off</description></item>
    /// <item><description><see cref="OnOff.Toggle"/> to toggle the current state</description></item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the new state (<see cref="OnOff"/>) after the operation, or <c>null</c> if the state is unknown.
    /// </returns>
    public async Task<OnOff?> SetSimpleOnOffAsync(string ain, OnOff onOff, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("setsimpleonoff", ain, $"onoff={((int)onOff)}"), cancellationToken);
        return res.ToOnOff();
    }

    /// <summary>
    /// Sets the dimming, height, brightness, or level of a device or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="level">The level to set (0–255).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="level"/> is not in the range 0–255.
    /// </exception>
    public async Task<int?> SetLevelAsync(string ain, int level, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 0, nameof(level));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, 255, nameof(level));

        var res = await GetStringAsync(BuildUrl("setlevel", ain, $"level={level}"), cancellationToken);
        return res?.ToInt();
    }

    /// <summary>
    /// Sets the dimming, height, brightness, or level of a device or actuator as a percentage.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="level">The level to set, in percent (0–100).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="level"/> is not in the range 0–100.
    /// </exception>
    public async Task<int?> SetLevelPercentageAsync(string ain, int level, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 0, nameof(level));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, 100, nameof(level));

        var res = await GetStringAsync(BuildUrl("setlevelpercentage", ain, $"level={level}"), cancellationToken);
        return res?.ToInt();
    }

    /// <summary>
    /// Adjusts the hue and saturation color of a lamp or actuator using the HSV color space.
    /// The brightness (value) can be configured separately using <see cref="SetLevelAsync"/> or <see cref="SetLevelPercentageAsync"/>.
    /// This method allows you to set the hue and saturation values directly.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="hue">Hue value of the color (0–359).</param>
    /// <param name="saturation">Saturation value of the color (0–255).</param>
    /// <param name="duration">Optional. The speed of the color change as a <see cref="TimeSpan"/>. If <c>null</c>, the default speed is used.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="hue"/> is not in the range 0–359, or <paramref name="saturation"/> is not in the range 0–255.
    /// </exception>
    public async Task<int?> SetColorAsync(string ain, int hue, int saturation, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(hue, 0, nameof(hue));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hue, 359, nameof(hue));
        ArgumentOutOfRangeException.ThrowIfLessThan(saturation, 0, nameof(saturation));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(saturation, 255, nameof(saturation));

        var res = await GetStringAsync(BuildUrl("setcolor", ain, $"hue={hue}&saturation={saturation}&duration={duration.ToDeciseconds()}"), cancellationToken);
        return res?.ToInt();
    }

    /// <summary>
    /// Adjusts the hue and saturation color of a lamp or actuator using the HSV color space, without mapping to device-specific color tables.
    /// The brightness (value) can be configured separately using <see cref="SetLevelAsync"/> or <see cref="SetLevelPercentageAsync"/>.
    /// This method allows you to set the unmapped hue and saturation values directly.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="hue">Unmapped hue value of the color (0–359).</param>
    /// <param name="saturation">Unmapped saturation value of the color (0–255).</param>
    /// <param name="duration">Optional. The speed of the color change as a <see cref="TimeSpan"/>. If <c>null</c>, the default speed is used.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="hue"/> is not in the range 0–359, or <paramref name="saturation"/> is not in the range 0–255.
    /// </exception>
    public async Task<int?> SetUnmappedColorAsync(string ain, int hue, int saturation, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(hue, 0, nameof(hue));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hue, 359, nameof(hue));
        ArgumentOutOfRangeException.ThrowIfLessThan(saturation, 0, nameof(saturation));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(saturation, 255, nameof(saturation));

        var res = await GetStringAsync(BuildUrl("setunmappedcolor", ain, $"hue={hue}&saturation={saturation}&duration={duration.ToDeciseconds()}"), cancellationToken);
        return res?.ToInt();
    }

    /// <summary>
    /// Sets the color temperature of a lamp or actuator.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="temperature">Color temperature in degrees Kelvin (2700–6500).</param>
    /// <param name="duration">Optional. The speed of the color change as a <see cref="TimeSpan"/>. If <c>null</c>, the default speed is used.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="temperature"/> is not in the range 2700–6500.
    /// </exception>
    public async Task<int?> SetColorTemperatureAsync(string ain, int temperature, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(temperature, 2700, nameof(temperature));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(temperature, 6500, nameof(temperature));

        var res = await GetStringAsync(BuildUrl("setcolortemperature", ain, $"temperature={temperature}&duration={duration.ToDeciseconds()}"), cancellationToken);
        return res.ToInt();
    }

    /// <summary>
    /// Creates a color template for lamps with specified brightness and color settings.
    /// </summary>
    /// <param name="name">The name of the template to create.</param>
    /// <param name="levelPercentage">The brightness level percentage of the light (0–1000).</param>
    /// <param name="hue">The hue value of the color (0–359).</param>
    /// <param name="saturation">The saturation value of the color (0–255).</param>
    /// <param name="ains">A collection of lamp device identifiers (AIN/MAC) to which the template will be applied.</param>
    /// <param name="colorpreset">Set to <c>true</c> to mark as a user color preset; otherwise, <c>false</c>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an integer indicating the result of the operation, or <c>null</c> if the operation failed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="levelPercentage"/> is not in the range 0–1000,
    /// <paramref name="hue"/> is not in the range 0–359,
    /// or <paramref name="saturation"/> is not in the range 0–255.
    /// </exception>
    public async Task<int?> AddColorLevelTemplateAsync(string name, int levelPercentage, int hue, int saturation, IEnumerable<string> ains, bool colorpreset = false, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(levelPercentage, 0, nameof(levelPercentage));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(levelPercentage, 1000, nameof(levelPercentage));
        ArgumentOutOfRangeException.ThrowIfLessThan(hue, 0, nameof(hue));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hue, 359, nameof(hue));
        ArgumentOutOfRangeException.ThrowIfLessThan(saturation, 0, nameof(saturation));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(saturation, 255, nameof(saturation));

        string ainlist = ains.Select((v, i) => $"child_{i + 1}={v}").Aggregate("", (a, b) => $"{a}&{b}");
        string req = $"webservices/homeautoswitch.lua?switchcmd=addcolorleveltemplate&sid={this.sessionId}&name={name}&levelPercentage={levelPercentage}&hue={hue}&saturation={saturation}&{ainlist}" + (colorpreset ? "&colorpreset=true" : "");
        var res = await GetStringAsync(req, cancellationToken);
        return res.ToInt();
    }

    /// <summary>
    /// Creates a color template for lamps
    /// </summary>
    /// <param name="name">Name of the template to create.</param>
    /// <param name="levelPercentage">Level Percentage of the light.</param>
    /// <param name="temperature">Color temperature of the light.</param>
    /// <param name="ains">List of lamp devices.</param>
    /// <param name="colorpreset">>User color preset or not.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">On of the argument are out of range.</exception>
    public async Task<int?> AddColorLevelTemplateAsync(string name, int levelPercentage, int temperature, IEnumerable<string> ains, bool colorpreset = false, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfLessThan(levelPercentage, 0, nameof(levelPercentage));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(levelPercentage, 1000, nameof(levelPercentage));
        ArgumentOutOfRangeException.ThrowIfLessThan(temperature, 2700, nameof(temperature));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(temperature, 6500, nameof(temperature));

        string ainlist = ains.Select((v, i) => $"child_{i + 1}={v}").Aggregate("", (a, b) => $"{a}&{b}");
        string req = $"webservices/homeautoswitch.lua?switchcmd=addcolorleveltemplate&sid={this.sessionId}&name={name}&levelPercentage={levelPercentage}&temperature={temperature}&{ainlist}" + (colorpreset ? "&colorpreset=true" : "");
        var res = await GetStringAsync(req, cancellationToken);
        return res?.ToInt();
    }

    /// <summary>
    /// Provides a proposal for the color selection values.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<ColorDefaults?> GetColorDefaultsAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getcolordefaults"), cancellationToken);
        return res.XDeserialize<ColorDefaults>("colordefaults");
    }

    /// <summary>
    /// Provides a proposal for the color selection values as XML.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<XmlDocument?> GetColorDefaultsXmlAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getcolordefaults"), cancellationToken);
        return res?.ToXml();
    }

    /// <summary>
    /// Activate HKR Boost with end time for the disable: endtimestamp = null.
    /// The end time may not exceed to 24 hours in the future lie.
    /// </summary>
    /// <param name="ain">Identification of the actor or template.</param>
    /// <param name="endtimestamp">End time to set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<DateTime?> SetHkrBoostAsync(string ain, DateTime? endtimestamp = null, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        if (endtimestamp.HasValue && (endtimestamp < DateTime.Now || endtimestamp > DateTime.Now + new TimeSpan(24, 0, 0)))
        {
            throw new ArgumentOutOfRangeException(nameof(endtimestamp));
        }
        var res = await GetStringAsync(BuildUrl("sethkrboost", ain, $"endtimestamp={endtimestamp.ToUnixTime()}"), cancellationToken);
        return res?.ToNullableDateTime();
    }

    /// <summary>
    /// HKR window open mode activate with end time for the disable: endtimestamp = null.
    /// The end time may not exceed to 24 hours in the future lie.
    /// </summary>
    /// <param name="ain">Identification of the actor or template.</param>
    /// <param name="endtimestamp">End time to set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<DateTime?> SetHkrWindowOpenAsync(string ain, DateTime? endtimestamp = null, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        if (endtimestamp.HasValue && (endtimestamp < DateTime.Now || endtimestamp > DateTime.Now + new TimeSpan(24, 0, 0)))
        {
            throw new ArgumentOutOfRangeException(nameof(endtimestamp));
        }
        var res = await GetStringAsync(BuildUrl("sethkrwindowopen", ain, $"endtimestamp={endtimestamp.ToUnixTime()}"), cancellationToken);
        return res?.ToNullableDateTime();
    }

    /// <summary>
    /// Close, open or stop the blind.
    /// Blinds have the HANFUN unit type Blind (281).
    /// </summary>
    /// <param name="ain">Identification of the actor or template.</param>
    /// <param name="target">Target to set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<Target?> SetBlindAsync(string ain, Target target, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("setblind", ain, $"target={target.ToString().ToLower()}"), cancellationToken);
        return res?.ToTarget();
    }

    /// <summary>
    /// Change device or group name.
    /// Attention: the user session must have the smart home and app right.
    /// </summary>
    /// <param name="ain">Identification of the actor or template.</param>
    /// <param name="name">New name, maximum 40 characters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <remarks>Requires the "Restricted FRITZ!Box settings for apps" permission.</remarks>
    public async Task<string?> SetNameAsync(string ain, string name, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, 40, nameof(name));

        var res = await GetStringAsync(BuildUrl("setname", ain, $"name={name}"), cancellationToken);
        return res?.TrimEnd();
    }

    /// <summary>
    /// json metadata of the template or change/set empty string
    /// </summary>
    /// <param name="ain">Identification of the actor or template.</param>
    /// <param name="metaData">Metadata to set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <remarks>New in Fritz!OS 7.39</remarks>
    public async Task SetMetaDataAsync(string ain, MetaData metaData, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        string md = ""; // metaData.AsToJson();
        var _ = await GetStringAsync(BuildUrl("setmetadata", ain, $"metadata={md}"), cancellationToken);
        //return res;
    }

    /// <summary>
    /// Start DECT ULE device registration.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <remarks>Requires the "Restricted FRITZ!Box settings for apps" permission.</remarks>
    public async Task StartUleSubscriptionAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        await GetStringAsync(BuildUrl("startulesubscription"), cancellationToken);
    }

    /// <summary>
    /// Query DECT-ULE device registration status.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <remarks>Requires the "Restricted FRITZ!Box settings for apps" permission.</remarks>
    public async Task<SubscriptionState?> GetSubscriptionStateAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getsubscriptionstate"), cancellationToken);
        return res.XDeserialize<SubscriptionState>("state");
    }

    /// <summary>
    /// Query DECT-ULE device registration status as XML.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <remarks>Requires the "Restricted FRITZ!Box settings for apps" permission.</remarks>
    public async Task<XmlDocument?> GetSubscriptionStateXmlAsync(CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getsubscriptionstate"), cancellationToken);
        return res.ToXml();
    }

    /// <summary>
    /// Provides the basic information of a single SmartHome device.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a <see cref="Device"/> object containing the device information, or <c>null</c> if the data is unavailable.
    /// </returns>
    public async Task<Device?> GetDeviceInfosAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getdeviceinfos", ain), cancellationToken);
        return res.XDeserialize<Device>("device");
    }

    /// <summary>
    /// Provides the basic information of a single SmartHome device as an XML document.
    /// </summary>
    /// <param name="ain">Identification of the actor or template (AIN/MAC).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an <see cref="XmlDocument"/> containing the device information, or <c>null</c> if the data is unavailable.
    /// </returns>
    public async Task<XmlDocument?> GetDeviceInfosXmlAsync(string ain, CancellationToken cancellationToken = default)
    {
        WebServiceException.ThrowIfNotConnected(client);

        var res = await GetStringAsync(BuildUrl("getdeviceinfos", ain), cancellationToken);
        return res.ToXml();
    }

    /// <summary>
    /// Create a bug report file.
    /// </summary>
    public void CreateBugReportFile()
    {
        WebServiceException.ThrowIfNotConnected(client);

        string fileName = $"BugReport-{DateTime.Now:yyy-MM-dd-HH-mm-ss}.xml";
        using var file = File.CreateText(fileName);

        file.WriteLine("<Report>");
        file.WriteLine(this.client!.GetStringAsync(BuildUrl("getdevicelistinfos")).Result);
        file.WriteLine(this.client!.GetStringAsync(BuildUrl("gettemplatelistinfos")).Result);
        file.WriteLine(this.client!.GetStringAsync(BuildUrl("getcolordefaults")).Result);
        try
        {
            file.WriteLine(this.client!.GetStringAsync(BuildUrl("gettriggerlistinfos")).Result);
        }
        catch
        { }
        file.WriteLine("</Report>");
    }
            
    #endregion
}
