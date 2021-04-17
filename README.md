# Unified Redis Platform (URP)

## Motivation
With the increase of Cloud Adoption, cost optimization has become a concern for organizations moving to the Cloud. Upon conducting a cost analysis of our Azure Subscriptions, we realized that Azure Caches for Redis were among the highest sources of expenditure. In our ecosystem of Micro-services, each service was using its own isolated Redis Cache for each environment. This is the same of many services and ecosystem across the industry. Furthermore, if you need to satisfy your customer demands of BCDR (Business Continuity Disaster Recovery), you might create replica Redis Caches in alternate regions.
In most cases, we analyzed that these Caches were not used up to their full potential (both in terms of load and storage). Our analysis concluded that we were using about 1GB of the P1 Redis Caches. A single P1 cache can store 6GB of data. A single P1 Redis Cache costs around $412 per month. With three micro-services and each service is utilizing two Redis Caches were spending about $2,412 per month.

Such an analysis motivated us to look for opportunities to merge the existing Redis Caches into a single cluster shared by the existing applications. Still, it needed to ensure that each application had proper isolation.

This platform allows multiple application to re-use a single or a cluster of Redis Caches. The platform provides proper isolation required by various apps to operate (get/edit/delete/update cached objects) on a single cluster without interfering (key overrides) with each other.

## Capabilities

1.  Consolidate multiple Azure Cache for Redis instances to a single cluster
2.  Automatic key name-spacing for providing isolation between multiple applications
3.  Disaster Recovery by keeping Azure Redis Cache in multiple regions in a single cluster
4.  Multiple read-regions
5.  Geo-replication of data (add/delete/update) across multiple regions
6.  Support for all operations available in StackExchange.Redis library. You can use all methods and data types from StackExchange.Redis without any code change.
7.  Support for auto-retires and timeouts in case of errors
8.  Automatic lazy and singleton connection for preventing connection exhaustion issues
9.  Secure connection using application-specific private Keys
10. Telemetry support for Redis Key usage, performance reports and failures
11. Management console for managing keys from backend

## Status
[![CI](https://github.com/microsoft/UnifiedRedisPlatform.Core/actions/workflows/ci.yml/badge.svg)](https://github.com/microsoft/UnifiedRedisPlatform.Core/actions/workflows/ci.yml)

[![Release](https://github.com/microsoft/UnifiedRedisPlatform.Core/actions/workflows/release-sdk.yml/badge.svg)](https://github.com/microsoft/UnifiedRedisPlatform.Core/actions/workflows/release-sdk.yml)

[![Release-Extension-DistributedCache](https://github.com/microsoft/UnifiedRedisPlatform.Core/actions/workflows/release-extensions-distributedcache.yml/badge.svg)](https://github.com/microsoft/UnifiedRedisPlatform.Core/actions/workflows/release-extensions-distributedcache.yml)

[![Build Status](https://dev.azure.com/MicrosoftIT/OneITVSO/_apis/build/status/Compliant/Core%20Services%20Engineering%20and%20Operations/Corporate%20Functions%20Engineering/Professional%20Services/Foundational%20PS%20Services/Field%20Experience%20Platform/PS-FPSS-FExP-GitHub-UnifiedRedisPlatform?branchName=main)](https://dev.azure.com/MicrosoftIT/OneITVSO/_build/latest?definitionId=33143&branchName=main)

![Nuget](https://img.shields.io/nuget/dt/UnifiedRedisPlatform?label=downloads%20core)

![Nuget](https://img.shields.io/nuget/dt/DistributedCache.Extensions.UnifiedRedisPlatform?label=downloads%20extension)

## Installation
URP comes in 2 flavours:
1. Core Package <br/>
    All types of .NET application can use the core library (desktop, web, form)
    ```
    Install-Package UnifiedRedisPlatform`
2. Extension of ASP.NET Core's `IDistributedCache` interface <br/>
    If your application is built on ASP.NET Core and you want to utilize the existing `IDistributedCache`, then this library is recommended for you <br/>
    ```
    Install-Package DistributedCache.Extensions.UnifiedRedisPlatform

See more details in the [WiKi](https://github.com/microsoft/UnifiedRedisPlatform.Core/wiki)

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
