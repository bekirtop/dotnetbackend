using Microsoft.EntityFrameworkCore;
using health.api.Models;

namespace health.api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 🔹 DbSet tanımları
        public DbSet<User> Users => Set<User>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<DoctorPatient> DoctorPatients => Set<DoctorPatient>();
        public DbSet<Medication> Medications => Set<Medication>();
        public DbSet<MedicationDoseSchedule> MedicationDoseSchedules => Set<MedicationDoseSchedule>();
        public DbSet<MedicationRecord> MedicationRecords => Set<MedicationRecord>();
        public DbSet<SideEffect> SideEffects => Set<SideEffect>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔸 Doctor–Patient (n–n)
            modelBuilder.Entity<DoctorPatient>()
                .HasKey(dp => new { dp.DoctorId, dp.PatientId });

            modelBuilder.Entity<DoctorPatient>()
                .HasOne(dp => dp.Doctor)
                .WithMany(d => d.DoctorPatients)
                .HasForeignKey(dp => dp.DoctorId);

            modelBuilder.Entity<DoctorPatient>()
                .HasOne(dp => dp.Patient)
                .WithMany(p => p.DoctorPatients)
                .HasForeignKey(dp => dp.PatientId);

            // 🔸 Medication – DoseSchedule (1–n)
            modelBuilder.Entity<Medication>()
                .HasMany(m => m.DoseSchedules)
                .WithOne(ds => ds.Medication)
                .HasForeignKey(ds => ds.MedicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔸 Medication – Record (1–n)
            modelBuilder.Entity<Medication>()
                .HasMany(m => m.MedicationRecords)
                .WithOne(r => r.Medication)
                .HasForeignKey(r => r.MedicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔸 Patient – Medication (1–n)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Medications)
                .WithOne(m => m.Patient)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔸 Patient – SideEffect (1–n)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.SideEffects)
                .WithOne(s => s.Patient)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔸 SideEffect – Medication (n–1) - Optional relationship
            // İlaç silinirse Yan Etki kaydı silinmesin, sadece MedicationId null olsun
            modelBuilder.Entity<SideEffect>()
                .HasOne(s => s.Medication)
                .WithMany()
                .HasForeignKey(s => s.MedicationId)
                .OnDelete(DeleteBehavior.SetNull);

            // 🔸 Message tablo ilişkileri
            modelBuilder.Entity<Message>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
