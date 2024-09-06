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
            var report = _context.Flights
                .Select(f => new
                {
                    FlightId = f.Id,
                    OccupiedSeats = _context.Reservations.Count(r => r.FlightId == f.Id), // Número de reservas
                    TotalSeats = f.AvailableSeats + _context.Reservations.Count(r => r.FlightId == f.Id),
                    OccupancyRate = (double)_context.Reservations.Count(r => r.FlightId == f.Id) / (f.AvailableSeats + _context.Reservations.Count(r => r.FlightId == f.Id)) * 100
                })
                .ToList();

            return Ok(report);
        }

        // Relatório mensal de vendas.
        [HttpGet("sales")]
        public IActionResult GetSalesReport()
        {
            var report = _context.Reservations
                .GroupBy(r => r.FlightId) // FlightId é um int, então essa comparação deve ser com outro int
                .Select(grp => new
                {
                    FlightId = grp.Key,
                    TotalSales = grp.Sum(r => _context.Flights.First(f => f.Id == grp.Key).Price * grp.Count()), // Total arrecadado por voo
                    ReservationCount = grp.Count() // Número de reservas
                })
                .ToList();

            return Ok(report);
        }
    }
}
