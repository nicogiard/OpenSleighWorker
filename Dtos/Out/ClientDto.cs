using System;

namespace OpenSleighWorker.Dtos.Out
{
    public class ClientDto
    {
        public Guid ClientId { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}