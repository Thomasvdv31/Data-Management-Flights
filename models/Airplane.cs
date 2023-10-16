using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace Data_Management_Flights_models
{
    public class Airplane
    {
        //Properties
        public int AantalPlaatsen { get; set; }

        public int BrandstofCapaciteit { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public int TypeId { get; set; }

        //ToString-methode van het object en retourneert een stringrepresentatie (model) van een vliegtuig.
        public override string ToString()
        {
            return Model;
        }
    }
}