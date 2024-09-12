// Importando os namespaces necessários para o funcionamento do controlador.
using Microsoft.AspNetCore.Mvc;
using FlightReservationAPI.DATA;
using FlightReservationAPI.MODELS;
using Microsoft.EntityFrameworkCore;

namespace FlightReservationAPI.Controllers
{
    // Definindo a rota base para o controlador ("/api/Flight") e indicando que ele será um controlador de API.
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        // Injetando o contexto do banco de dados (FlightReservationContext) através do construtor.
        private readonly FlightReservationContext _context;

        // Construtor do controlador que recebe o contexto do banco de dados e o atribui a uma variável local.
        public FlightController(FlightReservationContext context)
        {
            _context = context;
        }

        // Método GET para obter uma lista de todos os voos disponíveis.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            try
            {
                // Retorna a lista de voos armazenados no banco de dados de forma assíncrona.
                var flights = await _context.Flights.ToListAsync();
                return Ok(flights);
            }
            catch (Exception ex)
            {
                // Captura qualquer erro durante a consulta ao banco de dados e retorna um erro 500
                return StatusCode(500, $"Erro ao acessar o banco de dados: {ex.Message}");
            }
        }

        // Método POST para adicionar um novo voo ao sistema. Recebe os dados do voo no corpo da requisição.
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            try
            {
                // Verifica se os dados do voo são válidos
                if (flight == null)
                {
                    return BadRequest("Dados do voo estão inválidos.");
                }

                // Adiciona o novo voo ao contexto do banco de dados.
                _context.Flights.Add(flight);

                // Salva as alterações no banco de dados de forma assíncrona.
                await _context.SaveChangesAsync();

                // Retorna o status 201 Created, com a URL do novo recurso e os dados do voo criado.
                return CreatedAtAction(nameof(GetFlights), new { id = flight.Id }, flight);
            }
            catch (DbUpdateException ex)
            {
                // Captura erros relacionados ao banco de dados
                return StatusCode(500, $"Erro ao salvar os dados no banco: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Captura qualquer outro erro
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
