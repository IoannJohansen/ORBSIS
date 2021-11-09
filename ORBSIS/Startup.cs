using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ORBSIS.Data;
using ORBSIS.Hubs;
namespace ORBSIS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.AllowedUserNameCharacters = "ÀÁÂÃÄÅ¨ÆÇÈÉÊËÌÍÎÏĞÑÒÓÔÕÖ×ØÙÚÛÜİŞßàáâãäå¸æçèéêëìíîïğñòóôõö÷øùúûüışÿabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddCors();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
                AddCookie(options =>
                {
                    options.Cookie.Name = "AuthCookie";
                    options.LoginPath = "/account/facebook-login";
                    options.LoginPath = "/account/signin-google";
                    options.LoginPath = "/account/signin-github";
                }).
                AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    facebookOptions.SaveTokens = true;
                    facebookOptions.AccessDeniedPath = "/account/AccessDeny";
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:AppSecret"];
                    googleOptions.SaveTokens = true;
                    googleOptions.AccessDeniedPath = "/account/AccessDeny";
                })
                .AddGitHub(githubOptions =>
                {
                    githubOptions.ClientId = Configuration["Authentication:GitHub:ClientId"];
                    githubOptions.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                    githubOptions.SaveTokens = true;
                    githubOptions.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    githubOptions.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    githubOptions.UserInformationEndpoint = "https://api.github.com/user";
                });

            services.AddSignalR();
            
            services.AddMvc();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute
                (
                    name: "default",
                    pattern: "{controller=Chat}/{action=Index}"
                );
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
