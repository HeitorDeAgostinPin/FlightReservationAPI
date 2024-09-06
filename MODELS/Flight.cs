namespace FlightReservationAPI.MODELS
{
    public class Flight//Voo
    {
        public int Id { get; set; }

        public string Origin { get; set; } //Origem do Voo

        public string Destination { get; set; } //Destino do Voo

        public DateTime DepartureTime { get; set; } //Hora de partida

        public DateTime ArrivalTime { get; set; }//Hora de chegada

        public decimal Price { get; set; }// Preço da passagem

        public int AvailableSeats { get; set; }//Assentos disponíveis
    }
}
