// Importando os namespaces necessários para o funcionamento do controlador.
using Microsoft.AspNetCore.Mvc;

using FlightReservationAPI.DATA;
using FlightReservationAPI.MODELS;

namespace FlightReservationAPI.Controllers
{
    // Definindo a rota base para o controlador ("/api/Reservation") e indicando que ele será um controlador de API.
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        // Injetando o contexto do banco de dados (FlightReservationContext) através do construtor.
        private readonly FlightReservationContext _context;

        // Construtor do controlador que recebe o contexto do banco de dados e o atribui a uma variável local.
        public ReservationController(FlightReservationContext context)
        {
            _context = context;
        }

        // Método POST para criar uma nova reserva. Recebe os dados da reserva no corpo da requisição.
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            // Busca o voo correspondente à reserva no banco de dados.
            var flight = await _context.Flights.FindAsync(reservation.FlightId);

            // Verifica se há assentos disponíveis no voo.
            if (flight.AvailableSeats > 0)
            {
                // Se houver, decrementa o número de assentos disponíveis.
                flight.AvailableSeats--;

                // Adiciona a nova reserva ao contexto do banco de dados.
                _context.Reservations.Add(reservation);

                // Salva as alterações no banco de dados de forma assíncrona.
                await _context.SaveChangesAsync();

                // Retorna o status 201 Created, com a URL do novo recurso e os dados da reserva criada.
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
            }

            // Se não houver assentos disponíveis, retorna um erro 400 Bad Request.
            return BadRequest("No available seats.");
        }

        // Método GET para obter os dados de uma reserva específica com base no ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            // Busca a reserva pelo ID no banco de dados de forma assíncrona.
            var reservation = await _context.Reservations.FindAsync(id);

            // Se a reserva não for encontrada, retorna 404 Not Found.
            if (reservation == null)
            {
                return NotFound();
            }

            // Se a reserva for encontrada, retorna os dados dela.
            return reservation;
        }

        // Método DELETE para cancelar uma reserva existente
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound("Reserva não encontrada.");
            }

            var flight = await _context.Flights.FindAsync(reservation.FlightId);
            if (flight != null)
            {
                flight.AvailableSeats++; // Repor o assento cancelado
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 No Content após cancelar a reserva
        }
    }
}
