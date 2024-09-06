namespace FlightReservationAPI.MODELS
{
    public class Passenger//Passageiro
    {
        public string Id { get; set; }
        public string CPF { get; set; }
        public string Name { get; set; }//Nome

        public string Address { get; set; }//Endereço

        public string PhoneNumber { get; set; }  //NumeroTelefone
        
        public string Email { get; set; }
            
    }
}
