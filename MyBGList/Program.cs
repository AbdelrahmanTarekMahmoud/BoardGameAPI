

var builder = WebApplication.CreateBuilder(args);


//Logging
builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug();

builder.Host.UseSerilog((ctx, lc) => { 
    lc.ReadFrom.Configuration(ctx.Configuration);
    lc.WriteTo.MSSqlServer(
    connectionString:
    ctx.Configuration.GetConnectionString("DefaultConnection"),
    sinkOptions: new MSSqlServerSinkOptions
    {
        TableName = "LogEvents",
        AutoCreateSqlTable = true
    },
    columnOptions: new ColumnOptions()
    {
        AdditionalColumns = new SqlColumn[]
        {
            new SqlColumn()
            {
                ColumnName = "SourceContext",
                PropertyName = "SourceContext",
                DataType = System.Data.SqlDbType.NVarChar
            }
        }
    }
    );
},
writeToProviders: true);


// Add services to the container.
builder.Services.AddControllers(options =>
options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
(x, y) => $"The value '{x}' is not the same type as '{y}' try '{y.GetType()}'."));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
    opts.ResolveConflictingActions(apiDesc => apiDesc.First())
);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"))

);

//cors
builder.Services.AddCors(options =>
{
    //default cors policy for specific orgins
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    //customized cors policy for any origin
    options.AddPolicy(name: "AnyOrigin", cfg =>
    {
        cfg.AllowAnyOrigin();
        cfg.AllowAnyMethod();
        cfg.AllowAnyHeader();
    });
    options.AddPolicy(name: "AnyOrigin_GetOnly", cfg =>
    {
        cfg.AllowAnyOrigin();
        cfg.WithMethods("GET");
        cfg.AllowAnyHeader();
    });
});


//replaced by ManualValidationFilterAttribute
//Configuring the modelstate
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use developer exception page or exception handler based on configuration
if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();
//apply cors "must be before Authorization
app.UseCors();
app.UseAuthorization();

app.MapGet("/error/test", 
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)]
    () => { throw new Exception("test"); });

app.MapGet("/error",
[EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] (HttpContext context) =>
{
    var exceptionHandler =
    context.Features.Get<IExceptionHandlerPathFeature>();
    // TODO: logging, sending notifications, and more 
    var details = new ProblemDetails();

    details.Detail = exceptionHandler?.Error.Message;
    details.Extensions["traceId"] =
    System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
    details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
    details.Status = StatusCodes.Status500InternalServerError;

    app.Logger.LogError(CustomLogEvents.Error_Get,
    exceptionHandler?.Error,
    "An unhandled exception occurred.");

    return Results.Problem(details);
});


//Testing the code on demand constraint of rest 
app.MapGet("/cod/test",
    [EnableCors("AnyOrigin_GetOnly")]
    [ResponseCache(NoStore = true)] () =>
    Results.Text("<script>" +
    "window.alert('Your client supports JavaScript!" +
    "\\r\\n\\r\\n" +
    $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
    "\\r\\n" +
    "Client time (UTC): ' + new Date().toISOString());" +
    "</script>" +
    "<noscript>Your client does not support JavaScript</noscript>",
    "text/html"));

app.MapControllers().RequireCors("AnyOrigin");

app.Run();
