using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAppPlatform.Entities
{
    public class Contact: Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public Contact Manager { get; set; }
    }
}