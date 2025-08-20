var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar los servicios para inyección de dependencias
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.ConsultasDatos>();
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.PacienteDatos>();
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.MedicoDatos>();
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.MedicamentosDatos>();
builder.Services.AddSingleton<Proyecto_Clinica_Universitaria.Servicios.AzureBlobService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


