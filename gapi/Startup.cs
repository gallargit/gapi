using gapi.Queries;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Execution.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                .AddHttpRequestInterceptor<CustomHttpRequestInterceptor>()
                .AddIntrospectionAllowedRule() //as seen at https://github.com/ChilliCream/hotchocolate/issues/3417
                ;
        }
    }

    public class CustomHttpRequestInterceptor : DefaultHttpRequestInterceptor
    {
        public override ValueTask OnCreateAsync(
            HttpContext context,
            IRequestExecutor requestExecutor,
            IQueryRequestBuilder requestBuilder,
            CancellationToken cancellationToken)
        {
            //never allow introspection
            if (false)
            {
                requestBuilder.AllowIntrospection();
            }
            else
            {
                requestBuilder.SetIntrospectionNotAllowedMessage("Introspection is disabled");
            }
            return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
        }
    }
}
