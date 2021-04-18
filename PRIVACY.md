# Data Collection. 

The software may collect information about you and your use of the software and send it to Microsoft. Microsoft may use this information to provide services and improve our products and services. You may turn off the telemetry as described in the repository. There are also some features in the software that may enable you and Microsoft to collect data from users of your applications. If you use these features, you must comply with applicable law, including providing appropriate notices to users of your applications together with a copy of Microsoft's privacy statement. Our privacy statement is located at https://go.microsoft.com/fwlink/?LinkID=824704. You can learn more about data collection and use in the help documentation and our privacy statement. Your use of the software operates as your consent to these practices.

# Turning Off Telemetry

Telemetry is collected by the SDK in 2 different places
1. URP Server
    URP Web server is connected to Application Insights, and API usage (including Request-Response latency, dependencies and exceptions) are collected. You can turn of data collection by keeping `ApplicationInsights:Instrumentation` setting blank. Do not delete this key, as deleting the key might cause issues while bootstrapping the API and Function App.
    The change needs to be done in two places:
    a. [App Settings of the API](https://github.com/microsoft/UnifiedRedisPlatform.Core/blob/main/src/service/Microsoft.UnifiedRedisPlatform.Service/Web/API/appsettings.json)
    b. [App Settings of the Function App](https://github.com/microsoft/UnifiedRedisPlatform.Core/blob/main/src/service/Microsoft.UnifiedRedisPlatform.Service/Web/Function/appsettings.json)
2. URP SDK
    Data related to the usage of Redis Cache is collected. Information collection includes Configuration change events, Type of operations performed (StringGet, StringSet, etc.), latency of these operations and the success status of the operations. Only the Key name is collected, the values present in the keys are not logged. Each registered application can turn off data collection by disabling the diagnostic settings:
    ```
    "DiagnosticSettings": {
        "Enabled": "false"
    }
    ```
    `DiagnosticSettings:Enabled` should be `false` when registering the application. If the setting is enabled, then the data is sent to Application Insights.
    