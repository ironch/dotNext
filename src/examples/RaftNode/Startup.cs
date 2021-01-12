﻿using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Net.Cluster.Consensus.Raft.Http.Embedding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace RaftNode
{
    internal sealed class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public void Configure(IApplicationBuilder app)
        {
            app.UseConsensusProtocolHandler();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IClusterMemberLifetime, ClusterConfigurator>()
                .AddSingleton<IHttpMessageHandlerFactory, RaftClientHandlerFactory>()
                .AddOptions();
            var path = configuration[SimplePersistentState.LogLocation];
            if (!string.IsNullOrWhiteSpace(path))
            {
                Func<IServiceProvider, SimplePersistentState> serviceCast = ServiceProviderServiceExtensions.GetRequiredService<SimplePersistentState>;
                services
                    .AddSingleton<SimplePersistentState>()
                    .AddSingleton<IPersistentState>(serviceCast)
                    .AddSingleton<IValueProvider>(serviceCast)
                    .AddSingleton<IHostedService, DataModifier>();
            }
        }
    }
}