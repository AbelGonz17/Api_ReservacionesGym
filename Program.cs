using ApiReservacionesGym;
using ApiReservacionesGym.Middleawares;
using ApiReservacionesGym.Respositorio;
using ApiReservacionesGym.Servicios.Asistencias;
using ApiReservacionesGym.Servicios.Clase;
using ApiReservacionesGym.Servicios.Clientes;
using ApiReservacionesGym.Servicios.Membresias;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.Servicios.Pagos;
using ApiReservacionesGym.Servicios.Reserva;
using ApiReservacionesGym.Servicios.Suscripciones;
using ApiReservacionesGym.Servicios.Users;
using ApiReservacionesGym.UnitOfWork;
using ApiReservacionesGym.Utilidades;
using ApiReservacionesGym.Utilidades.MapeosPersonalizado;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Resources;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson(); 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); //  Mantenerlo si vamos a usar tokens

builder.Services.AddScoped<SignInManager<IdentityUser>>(); //  Necesario si usamos SignInManager

builder.Services.AddApplicationServices();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Instructor", politica => politica.RequireClaim(ClaimTypes.Role, "Instructor"));
    options.AddPolicy("Cliente", politica => politica.RequireClaim(ClaimTypes.Role, "Cliente"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
  

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["LlaveJWT"]!)),
        ClockSkew = TimeSpan.Zero
    };

});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(10);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 401)
    {
        await response.WriteAsync("🔒 No autorizado.");
    }
    else if (response.StatusCode == 403)
    {
        await response.WriteAsync("🚫 Prohibido.");
    }
});

app.MapControllers();

app.Run();