// <copyright file="TimedHostedService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.JsDetection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// TimedHostedService.
    /// </summary>
    public sealed class TimedHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">The IServiceScopeFactory.</param>
        public TimedHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        /// <inheritdoc/>
        public override Task StartAsync(CancellationToken stoppingToken)
        {
            this.timer = new Timer(async o => await this.DoWork(o), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private async Task DoWork(object state)
        {
            using var scope = this.serviceScopeFactory.CreateScope();

            var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IJsDetectionLogger>();

            await scopedProcessingService.FlushCounters();
        }
    }
}