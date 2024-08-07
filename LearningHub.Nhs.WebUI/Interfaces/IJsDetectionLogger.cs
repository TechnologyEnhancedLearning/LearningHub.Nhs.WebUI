﻿namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// IJsDetectionLogger.
    /// </summary>
    public interface IJsDetectionLogger
    {
        /// <summary>
        /// FlushCounters.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task FlushCounters();

        /// <summary>
        /// IncrementEnabled.
        /// </summary>
        void IncrementEnabled();

        /// <summary>
        /// IncrementDisabled.
        /// </summary>
        void IncrementDisabled();
    }
}