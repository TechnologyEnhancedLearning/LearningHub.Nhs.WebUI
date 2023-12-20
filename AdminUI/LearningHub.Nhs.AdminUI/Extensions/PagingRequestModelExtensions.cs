// <copyright file="PagingRequestModelExtensions.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Extensions
{
    using System;
    using LearningHub.Nhs.Models.Common.Enums;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// StringExtensions.
    /// </summary>
    public static class PagingRequestModelExtensions
    {
        /// <summary>
        /// Sanitize.
        /// </summary>
        /// <param name="model"> The model.</param>
        /// <returns>Sanitized model.</returns>
        public static PagingRequestModel Sanitize(this PagingRequestModel model)
        {
            if (model.Filter == null)
            {
                return model;
            }

            foreach (var filter in model.Filter)
            {
                switch (filter.Type)
                {
                    case PagingColumnType.Number:
                        if (!int.TryParse(filter.Value, out var number) || number < 0)
                        {
                            filter.Value = string.Empty;
                        }

                        break;
                    case PagingColumnType.Date:
                        if (!DateTime.TryParse(filter.Value, out _))
                        {
                            filter.Value = string.Empty;
                        }

                        break;
                    case PagingColumnType.Text:
                        filter.Value = filter.Value.Trim();

                        break;
                }
            }

            model.Filter.RemoveAll(f => f.Value == string.Empty);
            return model;
        }
    }
}
