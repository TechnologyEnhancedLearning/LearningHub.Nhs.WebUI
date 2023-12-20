// <copyright file="ScormContentEventSource.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.EventSource
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Threading;

    /// <summary>
    /// The ScormContentEventSource.
    /// </summary>
    [EventSource(Name = SourceName)]
    public class ScormContentEventSource : EventSource
    {
        /// <summary>
        /// Event source name.
        /// </summary>
        public const string SourceName = "ScormContent.Counters";

        /// <summary>
        /// Property Instance.
        /// </summary>
        public static readonly ScormContentEventSource Instance = new ScormContentEventSource();

        private PollingCounter requestCount;
        private IncrementingPollingCounter requestCountDelta;
        private EventCounter requestDuration;
        private EventCounter requestBytesServed;
        private EventCounter mp4RequestDuration;
        private EventCounter mp4RequestBytesServed;
        private long requestCountValue = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentEventSource"/> class.
        /// </summary>
        public ScormContentEventSource()
            : base(SourceName, EventSourceSettings.EtwSelfDescribingEventFormat)
        {
            // create the counters: they'll be bound to this event source + CounterGroup
            this.CreateCounters();
        }

        /// <summary>
        /// Add request process metadata.
        /// </summary>
        /// <param name="elapsedMilliseconds">Time serving the request.</param>
        /// <param name="bytesServed">Bytes served.</param>
        /// <param name="fileType">File type.</param>
        internal void AddRequestProcessMetadata(long elapsedMilliseconds, long bytesServed, string fileType)
        {
            Interlocked.Increment(ref this.requestCountValue);

            if (fileType.ToLower() == ".mp4")
            {
                this.mp4RequestDuration?.WriteMetric(elapsedMilliseconds);

                this.mp4RequestBytesServed?.WriteMetric(bytesServed);
            }
            else
            {
                // compute min/max/mean
                this.requestDuration?.WriteMetric(elapsedMilliseconds);

                this.requestBytesServed?.WriteMetric(bytesServed);
            }
        }

        private void CreateCounters()
        {
            // the same request count can be used for two counters:
            // - raw request counter that will always increase
            // - increment counter that will automatically compute the delta between the current value and the value when the counter was previously sent
            this.requestCount ??= new PollingCounter("request-count", this, () => this.requestCountValue) { DisplayName = "Requests count" };

            this.requestCountDelta ??= new IncrementingPollingCounter("request-count-delta", this, () => this.requestCountValue) { DisplayName = "New requests", DisplayRateTimeScale = new TimeSpan(0, 0, 1) };

            this.requestDuration ??= new EventCounter("request-duration", this) { DisplayName = "Requests duration in milliseconds" };

            this.requestBytesServed ??= new EventCounter("request-byte-served", this) { DisplayName = "Bytes-served during Request" };

            this.mp4RequestDuration ??= new EventCounter("mp4-request-duration", this) { DisplayName = "Mp4 Requests duration in milliseconds" };

            this.mp4RequestBytesServed ??= new EventCounter("mp4-request-byte-served", this) { DisplayName = "Bytes-served during Mp4 Request" };
        }
    }
}