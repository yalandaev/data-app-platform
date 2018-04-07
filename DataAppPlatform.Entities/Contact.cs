using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAppPlatform.Entities.Common.Attributes;

namespace DataAppPlatform.Entities
{
    [DisplayValue(nameof(FullName))]
    public class Contact: Entity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public long? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public Contact Manager { get; set; }
    }
}