var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
    opts.ResolveConflictingActions(apiDesc => apiDesc.First())
);

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
app.UseAuthorization();

app.MapGet("/error/test", () => { throw new Exception("test"); });
app.MapGet("/error", () => Results.Problem());
app.MapControllers();

app.Run();
