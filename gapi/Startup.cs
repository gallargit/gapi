using gapi.Queries;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Execution.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace gapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }

            app
                .UseHttpsRedirection()
                .UseAuthorization()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapGraphQL("/graphql")
                    .AllowAnonymous()
                    .WithOptions(new GraphQLServerOptions()
                    {
                        Tool = { Enable = true }
                    }
                    ));
        }

        //This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(Configuration)
                .AddControllers();

            int GraphQLSecondsTimeout = 4;

            //GraphQL objects
            services.AddValidation();
            services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .SetRequestOptions(_ => new RequestExecutorOptions { ExecutionTimeout = TimeSpan.FromSeconds(GraphQLSecondsTimeout) })
                //disable introspection queries
                .AllowIntrospection(false)
                ;
        }
    }
}
