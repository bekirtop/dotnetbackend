using System;
using System.Collections.Generic;

namespace health.api.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime? DischargeDate { get; set; }

        public ICollection<DoctorPatient>? DoctorPatients { get; set; }
        public ICollection<Medication>? Medications { get; set; }
        public ICollection<SideEffect>? SideEffects { get; set; }
    }
}
