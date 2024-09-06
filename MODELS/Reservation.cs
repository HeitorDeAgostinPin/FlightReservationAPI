namespace FlightReservationAPI.MODELS
{
    public class Reservation//Reserva
    {
        public int Id { get; set; }
        public int PassengerId { get; set; } // Id do Passageiro
        public Passenger Passenger { get; set; }
        public int FlightId { get; set; } //Id do Voo
        public Flight Flight { get; set; }
        public DateTime ReservationDate { get; set; }//Data da Reserva
        public string SeatNumber { get; set; }//Número do assento


        public bool IsCheckedIn { get; set; } // Nova propriedade para indicar se o check-in foi realizado
    }
}
