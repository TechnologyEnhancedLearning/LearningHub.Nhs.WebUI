namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;

    /// <summary>
    /// Defines the <see cref="ICountryService" />.
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Country> GetByIdAsync(int id);

        /// <summary>
        /// The GetFilteredAsync.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GenericListViewModel>> GetFilteredAsync(string filter);

        /// <summary>
        /// The GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<Country>> GetAllAsync();

        /// <summary>
        /// The GetAllUKCountries.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GenericListViewModel>> GetAllUKCountries();

        /// <summary>
        /// The GetAllNonUKCountries.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GenericListViewModel>> GetAllNonUKCountries();
    }
}
