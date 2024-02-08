namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The card controller.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService cardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardController"/> class.
        /// </summary>
        /// <param name="cardService">The card service.</param>
        public CardController(ICardService cardService)
        {
            this.cardService = cardService;
        }

        /// <summary>
        /// Get my contributions totals.
        /// </summary>
        /// <param name="catalogueId">The catalogue Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("GetMyContributionTotals/{catalogueId}")]
        public async Task<ActionResult<MyContributionsTotalsViewModel>> GetMyContributionTotalsAsync(int catalogueId)
        {
            return this.Ok(await this.cardService.GetMyContributionsTotalsAsync(catalogueId));
        }

        /// <summary>
        /// Get my contributions cards.
        /// </summary>
        /// <param name="catalogueId">The catalogue Id.</param>
        /// <param name="statusId">The status Id.</param>
        /// <param name="restrictToCurrentUser">The restrictToCurrentUser flag.</param>
        /// <param name="offset">The skip number.</param>
        /// <param name="take">The number of records to return.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("GetContributionCards/{catalogueId}/{statusId}/{restrictToCurrentUser}/{offset}/{take}")]
        public async Task<ActionResult<List<ResourceCardViewModel>>> GetContributionCardsAsync(int catalogueId, int statusId, bool restrictToCurrentUser, int offset, int take)
        {
            ResourceContributionsRequestViewModel resourceContributionsRequestViewModel = new ResourceContributionsRequestViewModel()
            {
                CatalogueNodeId = catalogueId,
                StatusId = statusId,
                RestrictToCurrentUser = restrictToCurrentUser,
                Offset = offset,
                Take = take,
            };
            return this.Ok(await this.cardService.GetContributionsAsync(resourceContributionsRequestViewModel));
        }

        /// <summary>
        /// Get my resources cards.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("GetMyResourceCards")]
        public async Task<ActionResult<MyContributionsViewModel>> GetMyResourceCardsAsync()
        {
            return this.Ok(await Task.Run(() => this.cardService.GetMyResourceViewModelAsync()));
        }

        /// <summary>
        /// Get my extended content cards.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("GetExtendedCardContent")]
        public async Task<ActionResult> GetExtendedCardContentAsync(int id)
        {
            var card = await Task.Run(() => this.cardService.GetResourceCardExtendedViewModelAsync(id));
            return this.Ok(card);
        }
    }
}