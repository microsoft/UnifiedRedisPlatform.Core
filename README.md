# Unified Refis Platform (URP)

This platform allows multiple application to re-use a single or a cluster of Redis Caches. The platform provides proper isolation required by multiple apps to operate (get/edit/delete/update cached objects) on a single cluster without interfering (overriding other application's keys) with each other.

Capabilities:

1.	Consolidate multiple Azure Cache for Redis instances to a single cluster
2.	Automatic key name-spacing for providing isolation between multiple applications
3.	Disaster Recovery by keeping Azure Redis Cache in multiple regions in a single cluster
4.	Multiple read-regions
5.	Geo-replication of data (add/delete/update) across multiple regions
6.	Support for all operations available in StackExchange.Redis library. All methods and data types from StackExchange.Redis can be re-used without any code change.
7.	Support for auto-retires and timeouts in case of errors
8.	Automatic lazy and singleton connection for preventing connection exhaustion issues
9.	Secure connection using application specific private Keys
10.	Telemetry support for key usage, performance reports and failures
11.	Management console for managing keys from backend

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
