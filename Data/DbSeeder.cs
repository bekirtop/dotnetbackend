using health.api.Models;

namespace health.api.Data
{
    public static class DbSeeder
    {
        public static void SeedDatabase(AppDbContext context)
        {
            // Eğer zaten veri varsa seeding yapma
            if (context.Users.Any())
            {
                Console.WriteLine("Database already seeded. Skipping...");
                return;
            }

            Console.WriteLine("Seeding database with demo data...");

            // Hash fonksiyonu
            string Hash(string input)
            {
                using var sha = System.Security.Cryptography.SHA256.Create();
                var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }

            // 1. Admin Kullanıcısı
            var adminUser = new User
            {
                FullName = "Admin User",
                Username = "admin",
                PasswordHash = Hash("admin123"),
                Role = "Admin"
            };
            context.Users.Add(adminUser);

            // 2. Doktor Kullanıcıları
            var doctor1User = new User
            {
                FullName = "Dr. Ahmet Yılmaz",
                Username = "doctor1",
                PasswordHash = Hash("doctor123"),
                Role = "Doctor"
            };
            var doctor2User = new User
            {
                FullName = "Dr. Ayşe Demir",
                Username = "doctor2",
                PasswordHash = Hash("doctor123"),
                Role = "Doctor"
            };
            context.Users.AddRange(doctor1User, doctor2User);

            // 3. Hasta Kullanıcıları
            var patient1User = new User
            {
                FullName = "Mehmet Kaya",
                Username = "patient1",
                PasswordHash = Hash("patient123"),
                Role = "Patient"
            };
            var patient2User = new User
            {
                FullName = "Fatma Şahin",
                Username = "patient2",
                PasswordHash = Hash("patient123"),
                Role = "Patient"
            };
            var patient3User = new User
            {
                FullName = "Ali Çelik",
                Username = "patient3",
                PasswordHash = Hash("patient123"),
                Role = "Patient"
            };
            context.Users.AddRange(patient1User, patient2User, patient3User);

            context.SaveChanges();

            // 4. Doktor Kayıtları
            var doctor1 = new Doctor
            {
                UserId = doctor1User.Id,
                Department = "Kardiyoloji"
            };
            var doctor2 = new Doctor
            {
                UserId = doctor2User.Id,
                Department = "Nöroloji"
            };
            context.Doctors.AddRange(doctor1, doctor2);

            // 5. Hasta Kayıtları
            var patient1 = new Patient
            {
                UserId = patient1User.Id,
                Diagnosis = "Hipertansiyon",
                DischargeDate = null
            };
            var patient2 = new Patient
            {
                UserId = patient2User.Id,
                Diagnosis = "Diyabet Tip 2",
                DischargeDate = null
            };
            var patient3 = new Patient
            {
                UserId = patient3User.Id,
                Diagnosis = "Migren",
                DischargeDate = DateTime.UtcNow.AddDays(-7) // Taburcu olmuş
            };
            context.Patients.AddRange(patient1, patient2, patient3);

            context.SaveChanges();

            // 6. Doktor-Hasta Atamaları
            var dp1 = new DoctorPatient { DoctorId = doctor1.Id, PatientId = patient1.Id };
            var dp2 = new DoctorPatient { DoctorId = doctor1.Id, PatientId = patient2.Id };
            var dp3 = new DoctorPatient { DoctorId = doctor2.Id, PatientId = patient3.Id };
            context.DoctorPatients.AddRange(dp1, dp2, dp3);

            context.SaveChanges();

            // 7. Örnek İlaçlar
            var med1 = new Medication
            {
                Name = "Ramipril",
                Dose = "5mg",
                FrequencyPerDay = 1,
                DurationDays = 90,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(80),
                Notes = "Sabahları aç karnına alın",
                PatientId = patient1.Id
            };
            var med2 = new Medication
            {
                Name = "Metformin",
                Dose = "850mg",
                FrequencyPerDay = 2,
                DurationDays = 90,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(85),
                Notes = "Yemeklerle birlikte alın",
                PatientId = patient2.Id
            };
            var med3 = new Medication
            {
                Name = "Aspirin",
                Dose = "100mg",
                FrequencyPerDay = 1,
                DurationDays = 30,
                StartDate = DateTime.UtcNow.AddDays(-15),
                EndDate = DateTime.UtcNow.AddDays(15),
                Notes = "Akşam yemeğinden sonra alın",
                PatientId = patient1.Id
            };
            context.Medications.AddRange(med1, med2, med3);

            context.SaveChanges();

            // 8. Örnek Mesajlar
            var msg1 = new Message
            {
                SenderId = doctor1User.Id,
                ReceiverId = patient1User.Id,
                Content = "Merhaba Mehmet Bey, tansiyon değerleriniz nasıl? Düzenli ölçüm yapıyor musunuz?",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                IsRead = false
            };
            var msg2 = new Message
            {
                SenderId = patient1User.Id,
                ReceiverId = doctor1User.Id,
                Content = "Doktorum, sabah 130/85 ölçtüm. Düzenli ölçüyorum.",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                IsRead = false
            };
            var msg3 = new Message
            {
                SenderId = doctor1User.Id,
                ReceiverId = patient2User.Id,
                Content = "Kan şekeri takibinizi düzenli yapıyor musunuz?",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsRead = true
            };
            context.Messages.AddRange(msg1, msg2, msg3);

            context.SaveChanges();

            // 9. Örnek Yan Etkiler
            var side1 = new SideEffect
            {
                PatientId = patient1.Id,
                MedicationId = med1.Id,
                Description = "Hafif baş dönmesi",
                Severity = "Mild",
                Date = DateTime.UtcNow.AddDays(-3)
            };
            var side2 = new SideEffect
            {
                PatientId = patient2.Id,
                MedicationId = med2.Id,
                Description = "Mide bulantısı",
                Severity = "Moderate",
                Date = DateTime.UtcNow.AddDays(-2)
            };
            context.SideEffects.AddRange(side1, side2);

            context.SaveChanges();

            Console.WriteLine("✓ Database seeded successfully!");
            Console.WriteLine("\nDemo Credentials:");
            Console.WriteLine("================");
            Console.WriteLine("Admin: admin / admin123");
            Console.WriteLine("Doctor 1: doctor1 / doctor123 (Kardiyoloji)");
            Console.WriteLine("Doctor 2: doctor2 / doctor123 (Nöroloji)");
            Console.WriteLine("Patient 1: patient1 / patient123 (Mehmet Kaya)");
            Console.WriteLine("Patient 2: patient2 / patient123 (Fatma Şahin)");
            Console.WriteLine("Patient 3: patient3 / patient123 (Ali Çelik - Taburcu)");
        }
    }
}
