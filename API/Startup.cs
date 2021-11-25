using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Persistence;
using API.Extentions;
using FluentValidation.AspNetCore;
using API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

namespace API
{
    public class Startup
    {
        private  IConfiguration _config { get; }
        public Startup(IConfiguration config)
        {
            this._config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt => {
                // we ensure that every endpoint require authorization
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation(config =>{
                config.RegisterValidatorsFromAssemblyContaining<Create>();
            });
            services.AddApplicationServices(_config);
            services.AddIndentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt => opt.Deny());
            // most important, I cas start with report only to check
            app.UseCsp(opt => opt
                .BlockAllMixedContent()// no http and https. Just https
                .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com/"))// from where css came from, we are ok just from our domain
                .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com/", "data:"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ScriptSources(s => s.Self().CustomSources("sha256-kXwZFeDqzQYQxMANlJcsdedkJvek1q5ncjzFrCq4x+I="))// for js 
                .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com/"))
                
            );
            if (env.IsDevelopment())
            {
                // orders metter and first is going to call be called first
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }else{
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            // here we have to add support to static file

            app.UseDefaultFiles();// going to check in wwwroot folder and index file
            app.UseStaticFiles();



            app.UseCors("CorsPolicy");// here the order is important

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
                 endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
