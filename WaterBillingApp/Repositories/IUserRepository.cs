using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Repositories
{
    /// <summary>
    /// Interface defining user-related data operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<ApplicationUser>> GetAllAsync();

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<ApplicationUser> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<ApplicationUser> GetByEmailAsync(string email);

        /// <summary>
        /// Creates a new user with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>The result of the creation operation.</returns>
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        Task UpdateAsync(ApplicationUser user);

        /// <summary>
        /// Deletes a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Checks if the user's email is confirmed asynchronously.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the email is confirmed; otherwise, false.</returns>
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);

        /// <summary>
        /// Checks if the user belongs to a specific role asynchronously.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="role">The role name.</param>
        /// <returns>True if the user is in the role; otherwise, false.</returns>
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);

        /// <summary>
        /// Adds the user to a specific role asynchronously.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="role">The role name.</param>
        /// <returns>A task representing the asynchronous add operation.</returns>
        Task AddToRoleAsync(ApplicationUser user, string role);

        /// <summary>
        /// Generates an email confirmation token for the user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The generated email confirmation token.</returns>
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        /// <summary>
        /// Generates a password reset token for the user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The generated password reset token.</returns>
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

        /// <summary>
        /// Confirms a user's email using the provided token asynchronously.
        /// </summary>
        /// <param name="user">The user to confirm.</param>
        /// <param name="token">The confirmation token.</param>
        /// <returns>The result of the confirmation operation.</returns>
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);

        /// <summary>
        /// Resets the user's password using the provided token asynchronously.
        /// </summary>
        /// <param name="user">The user whose password will be reset.</param>
        /// <param name="token">The password reset token.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>The result of the password reset operation.</returns>
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    }
}
