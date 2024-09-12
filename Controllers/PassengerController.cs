// Importando os namespaces necessários para o funcionamento do controlador.
using Microsoft.AspNetCore.Mvc;
using FlightReservationAPI.DATA;
using FlightReservationAPI.MODELS;
using Microsoft.EntityFrameworkCore;

namespace FlightReservationAPI.Controllers
{
    // Definindo a rota base para o controlador ("/api/Passenger") e indicando que ele será um controlador de API.
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        // Injetando o contexto do banco de dados (FlightReservationContext) através do construtor.
        private readonly FlightReservationContext _context;

        // Construtor do controlador que recebe o contexto do banco de dados e o atribui a uma variável local.
        public PassengerController(FlightReservationContext context)
        {
            _context = context;
        }

        // Método POST para criar um novo passageiro. Recebe os dados do passageiro no corpo da requisição.
        [HttpPost]
        public async Task<ActionResult<Passenger>> PostPassenger(Passenger passenger)
        {
            try
            {
                // Adiciona o novo passageiro ao contexto do banco de dados.
                _context.Passengers.Add(passenger);

                // Salva as alterações no banco de dados de forma assíncrona.
                await _context.SaveChangesAsync();

                // Retorna o status 201 Created, com a URL do novo recurso e os dados do passageiro criado.
                return CreatedAtAction(nameof(GetPassenger), new { id = passenger.Id }, passenger);
            }
            catch (DbUpdateException ex)
            {
                // Captura erros relacionados a atualizações do banco de dados, como chaves duplicadas.
                return BadRequest(new { message = "Erro ao salvar os dados do passageiro no banco de dados.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Captura outras exceções genéricas e retorna erro 500.
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", error = ex.Message });
            }
        }

        // Método GET para obter os dados de um passageiro específico com base no ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(int id)
        {
            try
            {
                // Busca o passageiro pelo ID no banco de dados de forma assíncrona.
                var passenger = await _context.Passengers.FindAsync(id);

                // Se o passageiro não for encontrado, retorna 404 Not Found.
                if (passenger == null)
                {
                    return NotFound(new { message = "Passageiro não encontrado." });
                }

                // Se o passageiro for encontrado, retorna os dados dele.
                return passenger;
            }
            catch (Exception ex)
            {
                // Captura exceções genéricas e retorna erro 500.
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar o passageiro.", error = ex.Message });
            }
        }
    }
}
