using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductGrpc.Data;
using ProductGrpc.Services;

namespace ProductGrpc
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(opt =>
            {
                opt.EnableDetailedErrors=true;
            });
            services.AddGrpcReflection();
            services.AddDbContext<ProductsContext>(options=>
            options.UseInMemoryDatabase("Products"));
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ProductService>();
                if (env.IsDevelopment())
                {
                    //https://github.com/fullstorydev/grpcurl/releases/tag/v1.9.2
                    //command: grpcurl localhost:5001 describe
                    //grpcurl -d "{ \"name\": \"World\" }" localhost:5001 greet.Greeter/SayHello


                    endpoints.MapGrpcReflectionService();
                }

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
