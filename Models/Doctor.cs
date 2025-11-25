using System;
using System.Collections.Generic;

namespace health.api.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }
        public string? Department { get; set; }
        public ICollection<DoctorPatient>? DoctorPatients { get; set; }
    }
}
