using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;  // para IOptions
using REMOVED.Data;
using REMOVED.Data.Entities;
using REMOVED.Helpers;

namespace REMOVED
{
    public class Program
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Customer", "Employee", "Guest" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@exemplo.com";
            string adminPassword = "Password123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = REMOVED
                };

                var createUserResult = await userManager.CreateAsync(newAdminUser, adminPassword);
                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create admin user: " +
                        string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
                }
            }
        }

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuração da base de dados
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Usar o ApplicationUser personalizado e ativar roles
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = REMOVED;
                options.Password.RequireLowercase = REMOVED;
                options.Password.RequireUppercase = REMOVED;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            // Configurações SMTP a partir do appsettings.json
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

            // Registar EmailSender usando as configurações injetadas
            builder.Services.AddTransient<IEmailSender>(serviceProvider =>
            {
                var smtpSettings = serviceProvider.GetRequiredService<IOptions<SmtpSettings>>().Value;
                return new EmailSender(smtpSettings);
            });

            var app = builder.Build();

            // Criar roles e admin ao iniciar
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await CreateRoles(services);
            }

            // Pipeline HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // <-- IMPORTANTE
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Remove se não estás a usar Razor Pages
            // app.MapRazorPages();

            await app.RunAsync();
        }
    }
}
