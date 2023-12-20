// <copyright file="TextBlockBuilder.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests.Builders
{
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A Helper class to easily construct TextBlockObjects.
    /// </summary>
    public class TextBlockBuilder
    {
        /// <summary>
        /// The content of the textBlock.
        /// </summary>
        private string content;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockBuilder"/> class.
        /// </summary>
        public TextBlockBuilder()
        {
            this.content = "This is a text block";
        }

        /// <summary>
        /// Builds the text block.
        /// </summary>
        /// <returns>The created text block.</returns>
        public TextBlockViewModel Build()
        {
            return new TextBlockViewModel
            {
                Content = this.content,
            };
        }
    }
}