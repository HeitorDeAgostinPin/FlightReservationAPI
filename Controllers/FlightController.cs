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
            // Retorna a lista de voos armazenados no banco de dados de forma assíncrona.
            return await _context.Flights.ToListAsync();
        }

        // Método POST para adicionar um novo voo ao sistema. Recebe os dados do voo no corpo da requisição.
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            // Adiciona o novo voo ao contexto do banco de dados.
            _context.Flights.Add(flight);

            // Salva as alterações no banco de dados de forma assíncrona.
            await _context.SaveChangesAsync();

            // Retorna o status 201 Created, com a URL do novo recurso e os dados do voo criado.
            return CreatedAtAction(nameof(GetFlights), new { id = flight.Id }, flight);
        }
    }
}
