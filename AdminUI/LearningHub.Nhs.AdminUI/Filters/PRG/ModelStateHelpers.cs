// <copyright file="ModelStateHelpers.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Filters.PRG
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ModelStateHelpers" />.
    /// </summary>
    public class ModelStateHelpers
    {
        /// <summary>
        /// The DeserialiseModelState.
        /// </summary>
        /// <param name="serialisedErrorList">The serialisedErrorList<see cref="string"/>.</param>
        /// <returns>The <see cref="ModelStateDictionary"/>.</returns>
        public static ModelStateDictionary DeserialiseModelState(string serialisedErrorList)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>(serialisedErrorList);
            var modelState = new ModelStateDictionary();

            foreach (var item in errorList)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }

            return modelState;
        }

        /// <summary>
        /// The SerialiseModelState.
        /// </summary>
        /// <param name="modelState">The modelState<see cref="ModelStateDictionary"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string SerialiseModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            return JsonConvert.SerializeObject(errorList);
        }
    }
}
