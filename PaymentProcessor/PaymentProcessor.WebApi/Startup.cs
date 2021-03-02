using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PaymentProcessor.Models.Entity;
using PaymentProcessor.Services;
using PaymentProcessor.Gateways;
using PaymentProcessor.Models.Entity.Repository;
using AutoMapper;

namespace PaymentProcessor.WebApi
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
            services.AddControllers();

            var configurationSection = Configuration.GetSection("ConnectionStrings:PaymentProcessorDb");
            services.AddDbContext<PaymentProcessorContext>(options => options.UseSqlServer(configurationSection.Value));

            services.AddScoped<ICheapPaymentGateway, CheapPaymentGateway>();
            services.AddScoped<IExpensivePaymentGateway, ExpensivePaymentGateway>();
            services.AddScoped<IPaymentRequestService, PaymentRequestService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentStateRepository, PaymentStateRepository>();
            services.AddAutoMapper(c => { c.AddProfile<Services.Profile.PaymentProfile>(); c.AddProfile<Services.Profile.PaymentStateProfile>(); }, typeof(Startup));

        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
