var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✔️ Swagger (zostaje)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// możesz zostawić albo zakomentować
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();