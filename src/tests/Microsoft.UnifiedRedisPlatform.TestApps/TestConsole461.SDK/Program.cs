using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using StackExchange.Redis;
using System.Threading.Tasks;
using Microsoft.UnifiedRedisPlatform.Core;
using Microsoft.UnifiedRedisPlatform.Core.Logging;

namespace TestConsole461.SDK
{
    class Program
    {
        private const int BasicMode = 0;
        private const int AdvancedMode = 1;

        
        private static string _clusterName;
        private static string _appName;
        private static string _appSecret;
        private static string _location;
        private static UnifiedConnectionMultiplexer _mux;
        private static IDatabase _database;
        private static int _mode = AdvancedMode;
        private static bool AsyncMode = true;

        static void Main(string[] args)
        {
            

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("UNIFIED REDIS PLATFORM (URP) TEST SDK (.NET FRAMEWORK 461) APP");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("  .NET Version: .NET Framework 4.6.1");
            Console.WriteLine("  Library: 0.0.5");
            Console.WriteLine("\n\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Do you want to use the default test configurations? (Y/N)");
            var defaultAppChoice = Console.ReadKey();

            if (defaultAppChoice.KeyChar == 'Y' || defaultAppChoice.KeyChar == 'y')
            { 
                //SetupDefaultAppDetails();
            }
            else
            {
                SetAppDetails();
            }   

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
                        DiagnosticSettings = new LogConfiguration()
                        {
                            Enabled = isDiagnosticsEnabled
                        }
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

            _database = _mux.GetDatabase();
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
                        DeleteKey();
                        break;
                    case 6:
                        PingContinously();
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

        //private static void SetupDefaultAppDetails()
        //{
        //    Console.WriteLine();
        //    Console.ForegroundColor = ConsoleColor.Blue;
        //    Console.WriteLine("Please wait while we get the default configuration...");

        //    _clusterName = _configuration["ClusterName"];
        //    _appName = _configuration["ClientName"];

        //    var keyVaultName = _configuration["KeyVaultName"];
        //    var secretsProvider = new SecretProvider(keyVaultName);

        //    var appSecretLocation = _configuration["AppSecretName"];
        //    _appSecret = secretsProvider.GetSecret(appSecretLocation).Result;

        //    Console.WriteLine("Default configurations received.");
        //}

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

            Console.Write("Location: ");
            _location = Console.ReadLine();
        }

        private static void ShowOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Here are your options:");
            Console.WriteLine("   0. Show options");
            Console.WriteLine("   1. Create a key");
            Console.WriteLine("   2. Get value by key name");
            Console.WriteLine("   3. Get all keys");
            Console.WriteLine("   4. Flush cache");
            Console.WriteLine("   5. Delete Key");
            Console.WriteLine("   6. Ping Continous");
            Console.WriteLine("   7. Exit App");
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
                Console.WriteLine("Warning: Please be aware of the performace impact of your operation. Press 'e' to exit");
                var choice = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                if (choice.KeyChar.ToString().ToLower() == "e")
                    return;

                var keys = AsyncMode ? _mux.GetKeysAsync().Result : _mux.GetKeys();
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
    }
}
