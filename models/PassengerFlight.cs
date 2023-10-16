using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_models
{
    public class PassengerFlight
    {
        //Properties
        public int AantalKoffers { get; set; }

        public float Gewicht { get; set; }
        public int Id { get; set; }
        public int PassagierId { get; set; }
        public int VluchtId { get; set; }
    }
}