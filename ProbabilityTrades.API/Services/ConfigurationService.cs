namespace ProbabilityTrades.API.Services;

public static class ConfigurationService
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
            options.AddPolicy("ApiKeyPolicy", policy => policy.RequireClaim("ApiKey")));

        builder.Services.AddAuthentication("ApiKey")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // TODO: TREY: Figure out how to set this to true
                    ValidateAudience = false, // TODO: TREY: Figure out how to set this to true
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "CORSPolicy",
                policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });

        });

        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Add Database Connections
        builder.AddDatabaseContext<ApplicationDbContext>("ApplicationDatabaseSqlServer");
        builder.AddDatabaseContext<CurrencyHistoryDbContext>("CurrencyHistoryDatabaseSqlServer");

        // Add Azure Connections
        builder.Services.AddSingleton(_ => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));

        // Add Service Dependencies
        builder.Services.AddScoped<IAzureApiService, AzureApiService>();
        builder.Services.AddScoped<IBlogImageService, BlogImageService>();
        builder.Services.AddScoped<IBlogService, BlogService>();
        builder.Services.AddScoped<ICalculatePumpService, CalculatePumpService>();
        builder.Services.AddScoped<ICurrencyHistoryService, KucoinService>();
        builder.Services.AddScoped<ICurrencyHistoryProcessService, CurrencyHistoryProcessService>();
        builder.Services.AddScoped<IDiscordNotificationService, DiscordNotificationService>();
        builder.Services.AddScoped<IIndicatorAnalysisService, IndicatorAnalysisService>();
        builder.Services.AddScoped<IMarketService, MarketService>();
        builder.Services.AddScoped<IMailService, MailService>();
        builder.Services.AddScoped<IMovingAverageService, MovingAverageService>();
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddScoped<IStripeApiService, StripeApiService>();
        builder.Services.AddScoped<IStripeService, StripeService>();
        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
        builder.Services.AddScoped<IUserExchangeService, UserExchangeService>();
        builder.Services.AddScoped<IUserPasswordResetService, UserPasswordResetService>();
    }

    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        //Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
        builder.Host.UseSerilog((context, LoggerConfiguration) => LoggerConfiguration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            //.WriteTo.MSSqlServer(
            //    connectionString: builder.Configuration.GetConnectionString("ApplicationDatabaseSqlServer"),
            //    sinkOptions: new MSSqlServerSinkOptions
            //    {
            //        TableName = "ApplicationLog",
            //        AutoCreateSqlTable = true,
            //    })
            );
    }

    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo { Title = "Probability Trades API", Version = "1.0" });
            setup.IncludeXmlComments(AppDomain.CurrentDomain.BaseDirectory + @"ProbabilityTrades.API.xml");
            setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Format: \"Bearer [Token]\""

            });
            setup.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
            {
                Name = "x-api-key",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "apikey",
                In = ParameterLocation.Header,
                Description = "Input apikey to access this API",
            });
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                    },
                    new string[] {}
                }
            });
        });
    }

    public static void ConfigureStripe(this WebApplicationBuilder builder)
    {
        var apiKey = builder.Configuration.GetValue<string>("Stripe:SecretKey");
        Stripe.StripeConfiguration.ApiKey = apiKey;
        //builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("Stripe"));
        var appinfo = new Stripe.AppInfo
        {
            Name = "Probability Trades",
            Url = "https://www.probabilitytrades.com",
            PartnerId = "pp_partner_FJQYXJ4X2Y7X2X",
            Version = "0.0.1"
        };
        Stripe.StripeConfiguration.AppInfo = appinfo;

        builder.Services.AddHttpClient("Stripe");
        builder.Services.AddTransient<Stripe.IStripeClient, Stripe.StripeClient>(_ =>
        {
            var httpClientFactory = _.GetRequiredService<IHttpClientFactory>();
            var httpClient = new Stripe.SystemNetHttpClient(
                httpClient: httpClientFactory.CreateClient("Stripe"),
                maxNetworkRetries: 3,
                appInfo: appinfo,
                enableTelemetry: true);

            return new Stripe.StripeClient(apiKey: apiKey, httpClient: httpClient);
        });
    }


    private static void AddDatabaseContext<T>(this WebApplicationBuilder builder, string connectionStringName) where T : DbContext
    {
        builder.Services.AddDbContext<T>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString(connectionStringName));
        });
    }
}