using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Repositories
{
    /// <summary>
    /// Repository for managing application users using ASP.NET Core Identity.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="userManager">The UserManager for <see cref="ApplicationUser"/>.</param>
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Gets all users asynchronously.
        /// </summary>
        /// <returns>A list of all users.</returns>
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        /// <summary>
        /// Finds a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        /// <summary>
        /// Finds a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Creates a new user with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>The result of the creation operation.</returns>
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Deletes a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        /// <summary>
        /// Checks if the user's email is confirmed asynchronously.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if confirmed; otherwise, false.</returns>
        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        /// <summary>
        /// Checks if the user is in a specific role asynchronously.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="role">The role name.</param>
        /// <returns>True if the user is in the role; otherwise, false.</returns>
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        /// <summary>
        /// Adds the user to a specific role asynchronously.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="role">The role name.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddToRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        /// <summary>
        /// Generates an email confirmation token for the user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The generated token.</returns>
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        /// <summary>
        /// Generates a password reset token for the user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The generated token.</returns>
        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        /// <summary>
        /// Confirms a user's email asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="token">The confirmation token.</param>
        /// <returns>The result of the confirmation operation.</returns>
        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        /// <summary>
        /// Resets the user's password asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="token">The reset token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The result of the password reset operation.</returns>
        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}
