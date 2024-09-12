using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Threading.Tasks;
using FlightReservationAPI.DATA;
using FlightReservationAPI.MODELS;
using Microsoft.EntityFrameworkCore;

namespace FlightReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly FlightReservationContext _context;

        public CheckInController(FlightReservationContext context)
        {
            _context = context;
        }

        // Método POST para realizar o check-in online.
        [HttpPost("{reservationId}")]
        public async Task<IActionResult> OnlineCheckIn(int reservationId)
        {
            try
            {
                // Verifica se a reserva existe
                var reservation = await _context.Reservations.FindAsync(reservationId);
                if (reservation == null)
                {
                    return NotFound("Reserva não encontrada.");
                }

                // Marcar o check-in como completo
                reservation.IsCheckedIn = true;
                await _context.SaveChangesAsync();

                // Emitir bilhete e enviar por e-mail.
                await SendTicketByEmail(reservation);

                return Ok("Check-in realizado com sucesso. Bilhete enviado por e-mail.");
            }
            catch (SmtpException ex)
            {
                // Captura erros relacionados ao envio de e-mail
                return StatusCode(500, $"Erro ao enviar o bilhete por e-mail: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                // Captura erros relacionados ao banco de dados
                return StatusCode(500, $"Erro ao atualizar o banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Captura qualquer outra exceção não tratada
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        private async Task SendTicketByEmail(Reservation reservation)
        {
            try
            {
                // Lógica para enviar bilhete por e-mail.
                var passenger = await _context.Passengers.FindAsync(reservation.PassengerId);
                if (passenger == null)
                {
                    throw new Exception("Passageiro não encontrado.");
                }

                MailMessage mail = new MailMessage();
                mail.To.Add(passenger.Email);
                mail.Subject = "Seu Bilhete de Voo";
                mail.Body = $"Olá {passenger.Name},\n\nAqui está o seu bilhete para o voo {reservation.FlightId}.\nAssento: {reservation.SeatNumber}.\n\nBoa viagem!";
                mail.From = new MailAddress("no-reply@flightreservation.com");

                // Configuração do servidor SMTP
                SmtpClient smtp = new SmtpClient("smtp.your-email.com"); // Configurar seu servidor SMTP

                // Envio do e-mail
                smtp.Send(mail);
            }
            catch (SmtpException ex)
            {
                // Lida com erros no envio do e-mail
                throw new Exception($"Erro ao enviar o e-mail: {ex.Message}", ex);
            }
        }
    }
}
