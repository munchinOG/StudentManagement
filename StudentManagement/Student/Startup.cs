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
             } ).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc( options =>
             {
                 var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
                 options.Filters.Add( new AuthorizeFilter( policy ) );
             } ).AddXmlSerializerFormatters();

            services.ConfigureApplicationCookie( options =>
             {
                 options.AccessDeniedPath = new PathString( "/Administration/AccessDenied" );
             } );

            services.AddAuthorization( options =>
             {
                 //Claims Policy
                 options.AddPolicy( "DeleteRolePolicy",
                     policy => policy.RequireClaim( "Delete Role" )
                         .RequireClaim( "Create Role" ) );

                 options.AddPolicy( "EditRolePolicy",
                     policy => policy.RequireClaim( "Edit Role", "true" ) );

                 //Roles Policy
                 options.AddPolicy( "AdminPolicy",
                     policy => policy.RequireRole( "Admin" ) );
             } );

            services.AddControllersWithViews();
            services.AddControllers( options => options.EnableEndpointRouting = false );
            services.AddScoped<IStudentRepository, SqlStudentRepository>();
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
            //app.UseMvcWithDefaultRoute();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();



            app.UseMvc( routes =>
            {
                routes.MapRoute( "default", "{controller}/{action}/{id}" );
            } );

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllerRoute( "default", "{controller=home}/{action=index}/{id?}" );
            } );

            //app.UseEndpoints( endpoints =>
            // {
            //     endpoints.MapGet( "/", async context =>
            //     {
            //         await context.Response.WriteAsync( "Hello World!" );
            //     } );
            // } );
        }
    }
}
