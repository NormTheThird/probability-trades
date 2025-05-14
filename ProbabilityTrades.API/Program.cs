var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();
builder.ConfigureLogging();
builder.ConfigureSwagger();
builder.ConfigureStripe();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(_ =>
{
    _.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    _.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseCors("CORSPolicy");
app.UseHttpsRedirection();
app.UseApiKey();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();