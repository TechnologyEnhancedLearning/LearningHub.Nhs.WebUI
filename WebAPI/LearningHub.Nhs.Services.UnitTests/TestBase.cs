// <copyright file="TestBase.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using AutoMapper;
    using LearningHub.Nhs.Models.Automapper;

    /// <summary>
    /// TestBase.
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// MapperProfile.
        /// </summary>
        /// <returns>IMapper.</returns>
        public IMapper MapperProfile()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            return mapper;
        }
    }
}
