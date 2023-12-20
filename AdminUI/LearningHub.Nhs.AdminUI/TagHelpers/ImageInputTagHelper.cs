// <copyright file="ImageInputTagHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.TagHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// Defines the <see cref="ImageInputTagHelper" />.
    /// </summary>
    [HtmlTargetElement("image-input", Attributes = "for, label, hint, value, image-width, image-height, base-url, circle-crop, existing-url")]
    public class ImageInputTagHelper : TagHelper
    {
        /// <summary>
        /// Gets or sets the BaseUrl.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CircleCrop.
        /// </summary>
        public bool CircleCrop { get; set; }

        /// <summary>
        /// Gets or sets the ExistingUrl.
        /// </summary>
        public string ExistingUrl { get; set; }

        /// <summary>
        /// Gets or sets the For.
        /// </summary>
        public string For { get; set; }

        /// <summary>
        /// Gets or sets the Hint.
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Gets or sets the ImageHeight.
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the ImageWidth.
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The ProcessAsync.
        /// </summary>
        /// <param name="context">The context<see cref="TagHelperContext"/>.</param>
        /// <param name="output">The output<see cref="TagHelperOutput"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            var hintList = new List<string>(this.Hint.Split(';'));
            var hint = string.Empty;
            if (hintList.Any() && hintList.Count > 1)
            {
                hint = "<ul>";
                foreach (var entry in hintList)
                {
                    hint = hint + $"<li><small style='color: #768692' class='mt-1 d-block hint'>{entry}</small></li>";
                }

                hint = hint + "</ul>";
            }
            else
            {
                hint = !string.IsNullOrEmpty(this.Hint)
                ? $"<small style='color: #768692' class='mt-1 d-block hint'>{this.Hint}</small>"
                : string.Empty;
            }

            string imageStyle;
            if (this.ImageWidth == -1 && this.ImageHeight == -1)
            {
                imageStyle = $"max-width: 100%; max-height: 100%;";
            }
            else
            {
                var circleRadius = System.Math.Min(this.ImageWidth, this.ImageHeight);
                imageStyle = this.CircleCrop
                    ? $"width: {this.ImageWidth}px; height: {this.ImageHeight}px; border-radius: {circleRadius / 2}px;"
                    : $"max-width: {this.ImageWidth}px; max-height: {this.ImageHeight}px;";
            }

            output.Content.AppendHtml($@"
            <div class='image-input'>
                <input id='{this.For}' type='file' accept='image/png, image/jpeg, image/gif, image/svg+xml' style='display:none;' name='{this.For}' class='image-input-hidden'/>
                <input id='hdn{this.For}Value1' type='hidden' name='{this.ExistingUrl}' value='{this.Value}'/>
                <div class='image-input-container' style='display:none'>
                    <div class='row'>
                        <div class='col-12 form-group'>
                            <label for='{this.For}'>{this.Label}</label>
                            {hint}
                            <div class='image-input-control'>
                                <button class='btn image-input-browse' type='button'>Browse</button>
                                <p>No file selected</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class='image-input-display' style='display: none;'>
                    <div class='row'>
                        <div class='col-12'>
                            <label for='{this.For}'>{this.Label}</label>
                            {hint}
                            <div class='image-input-display-container'>
                                <div class='row'>
                                    <div class='col-12 d-flex justify-content-center'>
                                        <img src='' style='{imageStyle}' class='mt-1 image-input-display-image'></img>
                                    </div>
                                </div>
                                <div class='row'>
                                    <div class='col d-flex justify-content-between'>
                                        <button class='image-input-change btn btn-link' type='button'>Change image</button>
                                        <button class='image-input-remove btn btn-link btn-link-red' type='button' name='{this.For}'>Remove image</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <input id='hdn{this.For}Value2' type='hidden' value='{this.Value}' class='image-url-value'/>
                <input type='hidden' class='image-url-base' value='{this.BaseUrl}'/>
            </div>
            ");
        }
    }
}
