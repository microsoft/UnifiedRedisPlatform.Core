using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.UnifiedRedisPlatform.Core;
using Microsoft.UnifiedRedisPlatform.Core.Logging;

namespace Microsoft.UnifiedRedisPlatform.TestConsoleCore.Latest
{
    class Program
    {
        private const int BasicMode = 0;
        private const int AdvancedMode = 1;

        private static IConfiguration _configuration;
        private static string _clusterName;
        private static string _appName;
        private static string _appSecret;
        private static string _location;
        private static UnifiedConnectionMultiplexer _mux;
        private static IUnifiedDatabase _database;
        private static int _mode = AdvancedMode;
        private static bool AsyncMode = true;

        static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("UNIFIED REDIS PLATFORM (URP) ADMIN APP");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("  .NET Version: .NET Core 3.1");
            Console.WriteLine("  Library: Direct Reference (Latest code)");
            Console.WriteLine("\n\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Do you want to use the default test configurations? (Y/N)");
            var defaultAppChoice = Console.ReadKey();

            if (defaultAppChoice.KeyChar == 'Y' || defaultAppChoice.KeyChar == 'y')
                SetupDefaultAppDetails();
            else
                SetAppDetails();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please wait while we connect to your Unified Cache...");

            _mux = null;
            if (_mode == AdvancedMode)
            {
                Console.WriteLine("Do you want to create the connection using custom configuration");
                var defaultConfigChoice = Console.ReadKey();
                Console.WriteLine();

                if (defaultConfigChoice.KeyChar == 'Y' || defaultConfigChoice.KeyChar == 'y')
                {
                    Console.WriteLine("Enter the connection max retry (-1 to choose preferred or default)");
                    int connectionMaxRetry = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter the connection timeout in ms (-1 to choose preferred or default)");
                    int connectionTimeout = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter the operational max retry (-1 to choose preferred or default)");
                    int opMaxRetry = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter the operational max timeout in ms (-1 to choose preferred or default)");
                    int opMaxTimeout = int.Parse(Console.ReadLine());

                    Console.WriteLine("Do you want to enable hard timeout (Y/N)");
                    var isHardTimeoutEnabled = Console.ReadKey().KeyChar.ToString().ToLower().Equals("y");
                    Console.WriteLine();

                    Console.WriteLine("Do you want to enable diagnostics (Y/N)");
                    var isDiagnosticsEnabled = Console.ReadKey().KeyChar.ToString().ToLower().Equals("y");
                    Console.WriteLine();
                    var diagnosticSetting = new LogConfiguration() { LoggingStrategy = "CacheAsideLogging", Enabled = false };

                    if (isDiagnosticsEnabled)
                    {
                        diagnosticSetting.Enabled = true;
                        Console.WriteLine("Press 1 to enable Immediate logging, 2 for Aggregated Logging and anything else for Cache-Aside Logging");
                        var cachingChoice = Console.ReadKey().KeyChar.ToString();
                        var isImmediateCachingEnabled = cachingChoice == "1";
                        var isAggregatedLoggingEnabled = cachingChoice == "2";
                        var isCacheAsideLogginEnabled = !isImmediateCachingEnabled && !isAggregatedLoggingEnabled;
                        if (isAggregatedLoggingEnabled)
                        {
                            diagnosticSetting.LoggingStrategy = "AggregateLogging";
                        }
                        else if (isImmediateCachingEnabled)
                        {
                            diagnosticSetting.LoggingStrategy = "ImmediateLogging";
                        }
                    }

                    var configuration = new UnifiedConfigurationServerOptions()
                    {
                        ClusterName = _clusterName,
                        AppName = _appName,
                        AppSecret = _appSecret,
                        Region = _location,
                        ConnectionRetryProtocol = connectionTimeout > 0 && connectionMaxRetry > 0 ? new RetryProtocol()
                        {
                            TimeoutInMs = connectionTimeout > 0 ? connectionTimeout : RetryProtocol.GetDefaultConnectionProtocol().TimeoutInMs,
                            MaxRetryCount = connectionMaxRetry > 0 ? connectionMaxRetry : RetryProtocol.GetDefaultConnectionProtocol().MaxRetryCount,
                            HardTimeoutEnabled = true,
                            MaxBackoffIntervalInMs = connectionTimeout > 0 ? connectionTimeout : RetryProtocol.GetDefaultConnectionProtocol().TimeoutInMs,
                            MinBackoffIntervalInMs = 5000
                        } : null,
                        OperationsRetryProtocol = opMaxTimeout > 0 && opMaxRetry > 0 ? new RetryProtocol()
                        {
                            TimeoutInMs = opMaxTimeout > 0 ? opMaxTimeout : RetryProtocol.GetDefaultOperationProtocol().TimeoutInMs,
                            MaxRetryCount = opMaxRetry > 0 ? opMaxRetry : RetryProtocol.GetDefaultOperationProtocol().MaxRetryCount,
                            HardTimeoutEnabled = isHardTimeoutEnabled,
                            MaxBackoffIntervalInMs = opMaxTimeout > 0 ? opMaxTimeout : RetryProtocol.GetDefaultOperationProtocol().TimeoutInMs,
                            MinBackoffIntervalInMs = 5000
                        } : null,
                        DiagnosticSettings = diagnosticSetting
                    };

                    _mux = UnifiedConnectionMultiplexer.Connect(configuration) as UnifiedConnectionMultiplexer;
                }

                Console.WriteLine("Do you want operations to be performed in sync method?");
                AsyncMode = !(Console.ReadKey().KeyChar.ToString().ToLower().Equals("y"));
                Console.WriteLine();
                Console.WriteLine("Please wait while we create your connection");
            }

            if (_mux == null)
            {
                _mux = UnifiedConnectionMultiplexer.Connect(_clusterName, _appName, _appSecret, preferredLocation: _location) as UnifiedConnectionMultiplexer;
            }

            _database = _mux.GetDatabase() as IUnifiedDatabase;
            _mux.ConnectionRestored += (s, e) => Console.WriteLine($"     [System][Log] Connection has been restored.");
            _mux.ConnectionFailed += (s, e) => Console.WriteLine($"     [System][Log] Connection failed with exception {e.Exception.Message}");
            _mux.ConfigurationChanged += (s, e) => Console.WriteLine("     [System][Log] Configuration changed.");
            _mux.ConfigurationChangedBroadcast += (s, e) => Console.WriteLine("     [System][Log] Configuration changed broadcast");
            _mux.ErrorMessage += (s, e) => Console.WriteLine($"     [System][Log] Server {e.EndPoint.ToString()} replied with error message - {e.Message}");
            _mux.HashSlotMoved += (s, e) => Console.WriteLine($"     [System][Log] Hash slot moved to new endpoint - {e.NewEndPoint.ToString()}");


            Console.WriteLine("Your connection has been created");

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n\n");

            var continueOperation = true;
            ShowOperations();
            do
            {
                Console.WriteLine("Enter your choice: ");
                if (!int.TryParse(Console.ReadLine(), out int operationChoice))
                    break;
                switch (operationChoice)
                {
                    case 0:
                        ShowOperations();
                        break;
                    case 1:
                        AddKey();
                        break;
                    case 2:
                        ShowKey();
                        break;
                    case 3:
                        ShowAllKeys();
                        break;
                    case 4:
                        Flush();
                        break;
                    case 5:
                        FlushSecondary();
                        break;
                    case 6:
                        DeleteKey();
                        break;
                    case 7:
                        DeletePattern();
                        break;
                    case 8:
                        PingContinously();
                        break;
                    case 9:
                        ShowLogs();
                        break;
                    case 10:
                        GetGeneicKey();
                        break;
                    case 11:
                        SetKeyWithExpiration();
                        break;
                    case 12:
                        ResetKeySlidingWindow();
                        break;

                    default:
                        continueOperation = false;
                        break;
                }
            } while (continueOperation);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Thank you for using the URP Test App. Press any key to exit...");
            Console.ReadKey();
        }

        private static void SetupDefaultAppDetails()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Please wait while we get the default configuration...");

            _clusterName = _configuration["ClusterName"];
            _appName = _configuration["AppName"];
            _appSecret = _configuration["AppSecret"];
            _location = _configuration["AppLocation"];

            Console.WriteLine("Default configurations received.");
        }

        private static void SetAppDetails()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Cluster Name: ");
            _clusterName = Console.ReadLine();

            Console.Write("App Name: ");
            _appName = Console.ReadLine();

            Console.Write("App Secret: ");
            _appSecret = Console.ReadLine();

            Console.Write("Region: ");
            _location = Console.ReadLine();
        }

        private static void ShowOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Here are your options:");
            Console.WriteLine("   0. Show options");
            Console.WriteLine("   1. Create a key");
            Console.WriteLine("   2. Get value by key name");
            Console.WriteLine("   3. Search keys");
            Console.WriteLine("   4. Flush cache");
            Console.WriteLine("   5. Flush secondary cache");
            Console.WriteLine("   6. Delete Key");
            Console.WriteLine("   7. Delete Key by pattern");
            Console.WriteLine("   8. Ping Continous");
            Console.WriteLine("   9. Show Uncomiited Logs");
            Console.WriteLine("   10. Advanced - Get generic key (with expiration)");
            Console.WriteLine("   11. Advanced - Set generic key with expiration");
            Console.WriteLine("   12. Advanced - Reset sliding window expiration");
            Console.WriteLine("   Anything else. Exit App");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void AddKey()
        {
            try
            {
                Console.WriteLine("Enter key name: ");
                var keyName = Console.ReadLine();
                Console.WriteLine("Enter value: ");
                var value = Console.ReadLine();

                if (AsyncMode)
                    _database.StringSetAsync(keyName, value).Wait();
                else
                    _database.StringSet(keyName, value);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Key has been succesfully added");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void ShowKey()
        {
            try
            {
                Console.WriteLine("Enter key name: ");
                var keyName = Console.ReadLine();

                var value =
                    AsyncMode ? _database.StringGetAsync(keyName).Result : _database.StringGet(keyName);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Your value is: {value}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void ShowAllKeys()
        {
            try
            {
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Warning: Please be aware of the performace impact of your operation. Press 'e' to exit or any other key to continue");
                var choice = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                if (choice.KeyChar.ToString().ToLower() == "e")
                    return;

                Console.WriteLine("Enter search pattern (use * for generic search). Press enter to search for all keys.");
                string searchPattern = Console.ReadLine();

                var keys = AsyncMode ? _mux.GetKeysAsync(pattern: searchPattern).Result : _mux.GetKeys(pattern: searchPattern);
                if (!keys.Any())
                    Console.WriteLine("No keys found in cache");
                else
                {
                    var counter = 1;
                    foreach (var key in keys)
                    {
                        Console.WriteLine($"{counter++}-> {key}");
                    }
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void ShowLogs()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Logs are only shown for Cache Aside Model (with default key)");
                Console.ForegroundColor = ConsoleColor.White;

                var logKey = "URP:Logs";
                var logs = AsyncMode ? _database.ListRangeAsync(logKey, 0, -1).Result : _database.ListRange(logKey, 0, -1);
                if (logs != null || logs.Any())
                {
                    var counter = 1;
                    foreach (var log in logs)
                    {
                        Console.WriteLine($"{counter++}. {log}");
                    }
                }
                else
                {
                    Console.WriteLine("No log found");
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void Flush()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Warning: Please be aware of the performace impact of your operation. Press 'e' to exit");
                var choice = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                if (choice.KeyChar.ToString().ToLower() == "e")
                    return;

                if (AsyncMode)
                    _mux.FlushAsync().Wait();
                else
                    _mux.FlushAsync();

                Console.WriteLine("Cache flushed");
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void DeletePattern()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Warning: Please be aware of the performace impact of your operation. Press 'e' to exit or any other key to continue.");
                var choice = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                if (choice.KeyChar.ToString().ToLower() == "e")
                    return;

                Console.WriteLine("Enter search pattern (use * for generic search). Press enter to search for all keys.");
                string searchPattern = Console.ReadLine();

                if (AsyncMode)
                    _mux.FlushAsync(pattern: searchPattern).Wait();
                else
                    _mux.FlushAsync(pattern: searchPattern);

                Console.WriteLine("Keys deleted");
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void FlushSecondary()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Warning: Please be aware of the performace impact of your operation. Press 'e' to exit");
                var choice = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                if (choice.KeyChar.ToString().ToLower() == "e")
                    return;

                if (AsyncMode)
                    _mux.FlushSecondaryAsync().Wait();
                else
                    _mux.FlushSecondary();

                Console.WriteLine("Cache flushed");
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void PingContinously()
        {
            try
            {
                Console.WriteLine("Press escape to stop");
                var counter = 1;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        if (AsyncMode)
                            _database.PingAsync();
                        else
                            _database.Ping();

                        Console.WriteLine($"Pinged {counter++} times. Time elapsed: {stopwatch.Elapsed.TotalSeconds}s");
                        Task.Delay(1500).Wait();
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void DeleteKey()
        {
            try
            {
                Console.WriteLine("Enter key name: ");
                var keyName = Console.ReadLine();

                if (AsyncMode)
                    _database.KeyDeleteAsync(keyName).Wait();
                else
                    _database.KeyDelete(keyName);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Key has been deleted");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void GetGeneicKey()
        {
            try
            {
                Console.WriteLine("Enter key name: ");
                var keyName = Console.ReadLine();

                var value =
                    AsyncMode ? _database.GetAsync(keyName).Result : _database.Get(keyName);

                Console.ForegroundColor = ConsoleColor.Green;
                if (value != null)
                    Console.WriteLine($"Your value is: {Encoding.ASCII.GetString(value)}");
                else
                    Console.WriteLine($"Value is not found");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void SetKeyWithExpiration()
        {
            try
            {
                Console.WriteLine("Enter key name: ");
                var key = Console.ReadLine();

                Console.WriteLine("Enter value: ");
                var value = Console.ReadLine();

                Console.WriteLine("Enter Relative Expiration (in sec): ");
                var expireOnSec = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter Sliding Window (in sec): ");
                var slidingWindowSec = int.Parse(Console.ReadLine());


                var cacheOptions = new CachingOptions();
                if (expireOnSec > 0)
                    cacheOptions.RelativeAbsoluteExpiration = TimeSpan.FromSeconds(expireOnSec);
                if (slidingWindowSec > 0)
                    cacheOptions.SlidingWindow = TimeSpan.FromSeconds(slidingWindowSec);

                if (AsyncMode)
                    _database.SetAsync(key, Encoding.ASCII.GetBytes(value), cacheOptions).Wait();
                else
                    _database.Set(key, Encoding.ASCII.GetBytes(value), cacheOptions);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Key has been added");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void ResetKeySlidingWindow()
        {
            try
            {
                Console.WriteLine("Enter key name: ");
                var key = Console.ReadLine();

                Console.WriteLine("Enter Sliding Window (in sec): ");
                var slidingWindowSec = int.Parse(Console.ReadLine());
                TimeSpan? slidingWindow = null;
                if (slidingWindowSec > 0)
                    slidingWindow = TimeSpan.FromSeconds(slidingWindowSec);

                if (AsyncMode)
                    _database.ResetWindowAsync(key, slidingWindow).Wait();
                else
                    _database.ResetWindow(key, slidingWindow);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Key's sliding has been reset");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("There was an error in executing the operation");
                Console.WriteLine(exception.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
