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
using Microsoft.OpenApi.Models;
using PocketMonsters.PokeApi;
using PocketMonsters.TranslateApi;

namespace PocketMonsters
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PocketMonsters", Version = "v1" });
            });

            var pokeApiOptions = new PokeApiOptions();
            Configuration.GetSection("PokeApi").Bind(pokeApiOptions);
            services.AddSingleton(pokeApiOptions);

            var translateApiOptions = new TranslateApiOptions();
            Configuration.GetSection("TranslateApi").Bind(translateApiOptions);
            services.AddSingleton(translateApiOptions);
            
            //TODO: Revise this
            services.AddHttpClient<PokeApiClient>();
            services.AddSingleton<IPokeApiClient, PokeApiClient>();
            services.AddSingleton<IPokeDexService, PokeDexService>();

            services.AddHttpClient<FunTranslateApiClient>();
            services.AddSingleton<FunTranslateApiClient>();
            services.AddSingleton<IShakespeareTranslator>(x => x.GetRequiredService<FunTranslateApiClient>());
            services.AddSingleton<IYodaTranslator>(x => x.GetRequiredService<FunTranslateApiClient>());

            services.AddSingleton<IPokemonTranslationService, PokemonTranslationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PocketMonsters v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("health");
            });
        }
    }
}
