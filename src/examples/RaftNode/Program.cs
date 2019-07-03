﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RaftNode
{
    public static class Program
    {
        private static void StartNode(int port, string messageFile = null)
        {
            var configuration = new Dictionary<string, string>
            {
                {"absoluteMajority", "false"},
                {"lowerElectionTimeout", "150" },
                {"upperElectionTimeout", "400" },
                {"members:0", "http://localhost:3262"},
                {"members:1", "http://localhost:3263"},
                {"members:2", "http://localhost:3264"}
            };
            if (!string.IsNullOrEmpty(messageFile))
                configuration[FileListener.MessageFile] = messageFile;
            new WebHostBuilder()
                .UseKestrel(options => options.ListenLocalhost(port))
                .UseShutdownTimeout(TimeSpan.FromHours(1))
                //.ConfigureLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning))
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configuration))
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        private static void Main(string[] args)
        {
            switch (args.LongLength)
            {
                case 0:
                    Console.WriteLine("Specify port number");
                    break;
                case 1:
                    StartNode(int.Parse(args[0]));
                    break;
                case 2:
                    StartNode(int.Parse(args[0]), args[1]);
                    break;
            }
        }
    }
}
