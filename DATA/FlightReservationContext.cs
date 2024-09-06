
using Microsoft.EntityFrameworkCore;
using FlightReservationAPI.MODELS;
namespace FlightReservationAPI.DATA
{
    public class FlightReservationContext:DbContext
    {
        public FlightReservationContext(DbContextOptions<FlightReservationContext> options): base(options) { }
    
        public DbSet<Passenger> Passengers { get; set; }//Passageiro
        public DbSet<Flight> Flights { get; set; }//Voo

        public DbSet <Reservation> Reservations { get; set; } //Reserva

    }
}
