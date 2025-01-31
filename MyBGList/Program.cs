using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyBGList.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
    [ResponseCache(NoStore = true)]
    () => Results.Problem());

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
