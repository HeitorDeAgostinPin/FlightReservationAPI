// Importando os namespaces necessários para o funcionamento do controlador.
using Microsoft.AspNetCore.Mvc;
using FlightReservationAPI.DATA;
using FlightReservationAPI.MODELS;
using Microsoft.EntityFrameworkCore;

namespace FlightReservationAPI.Controllers
{
    // Definindo a rota base para o controlador ("/api/Reservation") e indicando que ele será um controlador de API.
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly FlightReservationContext _context;

        public ReservationController(FlightReservationContext context)
        {
            _context = context;
        }

        // Método POST para criar uma nova reserva. Recebe os dados da reserva no corpo da requisição.
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            try
            {
                // Busca o voo correspondente à reserva no banco de dados.
                var flight = await _context.Flights.FindAsync(reservation.FlightId);

                if (flight == null)
                {
                    return NotFound(new { message = "Voo não encontrado." });
                }

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
                return BadRequest(new { message = "Não há assentos disponíveis." });
            }
            catch (DbUpdateException ex)
            {
                // Tratamento de erros específicos de banco de dados
                return BadRequest(new { message = "Erro ao salvar a reserva no banco de dados.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Tratamento de exceções gerais
                return StatusCode(500, new { message = "Ocorreu um erro ao processar a solicitação.", error = ex.Message });
            }
        }

        // Método GET para obter os dados de uma reserva específica com base no ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            try
            {
                // Busca a reserva pelo ID no banco de dados de forma assíncrona.
                var reservation = await _context.Reservations.FindAsync(id);

                // Se a reserva não for encontrada, retorna 404 Not Found.
                if (reservation == null)
                {
                    return NotFound(new { message = "Reserva não encontrada." });
                }

                // Se a reserva for encontrada, retorna os dados dela.
                return reservation;
            }
            catch (Exception ex)
            {
                // Tratamento de exceções gerais
                return StatusCode(500, new { message = "Erro ao buscar a reserva.", error = ex.Message });
            }
        }

        // Método DELETE para cancelar uma reserva existente
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound(new { message = "Reserva não encontrada." });
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
            catch (DbUpdateException ex)
            {
                // Tratamento de erros específicos de banco de dados
                return BadRequest(new { message = "Erro ao cancelar a reserva no banco de dados.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Tratamento de exceções gerais
                return StatusCode(500, new { message = "Ocorreu um erro ao processar o cancelamento da reserva.", error = ex.Message });
            }
        }
    }
}
