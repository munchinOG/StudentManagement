﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Student.Models;
using Student.Security;
using System;

namespace Student
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup( IConfiguration configuration )
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddDbContextPool<ApplicationDbContext>(
                options => options.UseSqlServer( _configuration.GetConnectionString( "StudentDbConnection" ) ) );

            services.AddIdentity<ApplicationUser, IdentityRole>( options =>
             {
                 options.Password.RequiredLength = 4;
                 options.Password.RequiredUniqueChars = 2;
                 options.Password.RequireUppercase = false;
                 options.Password.RequireNonAlphanumeric = false;

                 options.SignIn.RequireConfirmedEmail = true;
                 options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                 options.Lockout.MaxFailedAccessAttempts = 5;
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes( 15 );

             } )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider
                <ApplicationUser>>( "CustomEmailConfirmation" );

            //Changes Token Lifespan of all Token Types
            services.Configure<DataProtectionTokenProviderOptions>( options =>
             options.TokenLifespan = TimeSpan.FromHours( 2 ) );

            //Changes token lifespan of just the Email Confirmation Token
            services.Configure<CustomEmailConfirmationTokenProviderOptions>( options =>
             options.TokenLifespan = TimeSpan.FromDays( 3 ) );

            services.AddMvc( options =>
             {
                 var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
                 options.Filters.Add( new AuthorizeFilter( policy ) );
             } ).AddXmlSerializerFormatters();

            services.AddAuthentication()
                .AddGoogle( options =>
                 {
                     options.ClientId = "542063870337-g0niar7k6p9d312grejre49t51futcoh.apps.googleusercontent.com";
                     options.ClientSecret = "6fVA5QxEY8ZZblJRjUOI20nZ";
                 } )
                .AddFacebook( options =>
                 {
                     options.AppId = "325386538817621";
                     options.AppSecret = "3e908225550907fd658ffa1b0a85723d";
                 } );

            services.ConfigureApplicationCookie( options =>
             {
                 options.AccessDeniedPath = new PathString( "/Administration/AccessDenied" );
             } );

            services.AddAuthorization( options =>
             {
                 //Claims Policy
                 options.AddPolicy( "DeleteRolePolicy",
                     policy => policy.RequireClaim( "Delete Role" ) );

                 options.AddPolicy( "EditRolePolicy",
                     policy => policy.AddRequirements( new ManageAdminRolesAndClaimsRequirement() ) );

                 //Roles Policy
                 options.AddPolicy( "AdminPolicy",
                     policy => policy.RequireRole( "Admin" ) );
             } );

            services.AddControllersWithViews();
            services.AddControllers( options => options.EnableEndpointRouting = false );

            services.AddScoped<IStudentRepository, SqlStudentRepository>();

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( "/Error" );
                app.UseStatusCodePagesWithReExecute( "/Error/{0}" );
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseCors();
            //app.UseAuthorization();



            app.UseMvc( routes =>
            {
                routes.MapRoute( "default", "{controller}/{action}/{id}" );
            } );

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllerRoute( "default", "{controller=home}/{action=index}/{id?}" );
            } );
        }
    }
}
