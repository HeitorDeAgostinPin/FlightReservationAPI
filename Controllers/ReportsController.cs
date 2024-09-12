using Microsoft.AspNetCore.Mvc;
using FlightReservationAPI.MODELS;
using System.Linq;
using FlightReservationAPI.DATA;

namespace FlightReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly FlightReservationContext _context;

        public ReportsController(FlightReservationContext context)
        {
            _context = context;
        }

        // Relatório semanal de ocupação de voos.
        [HttpGet("occupancy")]
        public IActionResult GetOccupancyReport()
        {
            try
            {
                // Lógica para calcular o relatório de ocupação de voos
                var report = _context.Flights
                    .Select(f => new
                    {
                        FlightId = f.Id,
                        OccupiedSeats = _context.Reservations.Count(r => r.FlightId == f.Id), // Número de reservas
                        TotalSeats = f.AvailableSeats + _context.Reservations.Count(r => r.FlightId == f.Id),
                        OccupancyRate = (f.AvailableSeats + _context.Reservations.Count(r => r.FlightId == f.Id)) > 0
                                        ? (double)_context.Reservations.Count(r => r.FlightId == f.Id) / (f.AvailableSeats + _context.Reservations.Count(r => r.FlightId == f.Id)) * 100
                                        : 0 // Se não houver assentos disponíveis, define a taxa de ocupação como 0
                    })
                    .ToList();

                return Ok(report); // Retorna o relatório se tudo correr bem
            }
            catch (DivideByZeroException ex)
            {
                // Tratamento específico para divisão por zero
                return BadRequest(new { message = "Erro: Tentativa de divisão por zero ao calcular a taxa de ocupação.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Tratamento geral para outros erros
                return StatusCode(500, new { message = "Erro interno do servidor.", error = ex.Message });
            }
        }

        // Relatório mensal de vendas.
        [HttpGet("sales")]
        public IActionResult GetSalesReport()
        {
            try
            {
                var report = _context.Reservations
                    .GroupBy(r => r.FlightId) // Agrupando por FlightId, que é um int
                    .Select(grp => new
                    {
                        FlightId = grp.Key,
                        TotalSales = _context.Flights.Any(f => f.Id == grp.Key)
                            ? grp.Sum(r => _context.Flights.First(f => f.Id == grp.Key).Price * grp.Count())  // Total arrecadado por voo
                            : 0, // Caso o voo não exista
                        ReservationCount = grp.Count() // Número de reservas
                    })
                    .ToList();

                return Ok(report);
            }
            catch (InvalidOperationException ex)
            {
                // Tratamento específico para erros de consulta, como consultas incorretas
                return BadRequest(new { message = "Erro na consulta.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Tratamento geral para outros erros
                return StatusCode(500, new { message = "Erro interno do servidor.", error = ex.Message });
            }
        }
    }
}
