using Microsoft.AspNetCore.Mvc;

using System.Net.Mail;
using System.Threading.Tasks;
using FlightReservationAPI.DATA;
using FlightReservationAPI.MODELS;

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
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                return NotFound("Reserva não encontrada.");
            }

            // Marcar o check-in como completo (exemplo: adicionando um campo na classe Reservation)
            reservation.IsCheckedIn = true;
            await _context.SaveChangesAsync();

            // Emitir bilhete e enviar por e-mail.
            await SendTicketByEmail(reservation);

            return Ok("Check-in realizado com sucesso. Bilhete enviado por e-mail.");
        }

        private async Task SendTicketByEmail(Reservation reservation)
        {
            // Lógica para enviar bilhete por e-mail.
            var passenger = await _context.Passengers.FindAsync(reservation.PassengerId);
            if (passenger != null)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(passenger.Email);
                mail.Subject = "Seu Bilhete de Voo";
                mail.Body = $"Olá {passenger.Name},\n\nAqui está o seu bilhete para o voo {reservation.FlightId}.\nAssento: {reservation.SeatNumber}.\n\nBoa viagem!";
                mail.From = new MailAddress("no-reply@flightreservation.com");

                SmtpClient smtp = new SmtpClient("smtp.your-email.com"); // Configurar seu servidor SMTP
                smtp.Send(mail);
            }
        }
    }
}
