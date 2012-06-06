using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace sample_mongodb_net
{
    public class Contact
    {
        public BsonObjectId Id { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
    }
}
