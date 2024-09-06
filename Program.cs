using FlightReservationAPI.DATA;
using Microsoft.EntityFrameworkCore;

namespace FlightReservationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configura��o do banco de dados com o DbContext
            builder.Services.AddDbContext<FlightReservationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Adicionando suporte para controladores e endpoints de API
            builder.Services.AddControllers();

            // Configura��o do Swagger
            builder.Services.AddSwaggerGen();

            // Adicionando suporte para autoriza��o
            builder.Services.AddAuthorization();

            // Adiciona a capacidade de explorar endpoints via Swagger
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configura��o do Swagger no ambiente de desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightReservationAPI V1");
                });
            }

            // Redirecionamento de HTTP para HTTPS
            app.UseHttpsRedirection();//

            // Habilitando a autoriza��o
            app.UseAuthorization();

            // Mapeando os controladores para os endpoints
            app.MapControllers();

            // Executa o aplicativo
            app.Run();
        }
    }
}
