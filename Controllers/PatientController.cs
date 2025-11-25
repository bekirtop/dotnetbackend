using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using health.api.Data;
using health.api.Models;
using System.Threading.Tasks;
using System.Linq;

namespace health.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PatientController(AppDbContext db)
        {
            _db = db;
        }

        // DTO (controller içinde)
        public class PatientListDto
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Diagnosis { get; set; }
            public DateTime? DischargeDate { get; set; }
        }

        // GET: /api/Patient
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _db.Patients
                .Include(p => p.User)
                .Select(p => new PatientListDto
                {
                    Id = p.Id,
                    FullName = p.User.FullName,
                    Diagnosis = p.Diagnosis,
                    DischargeDate = p.DischargeDate
                })
                .ToListAsync();

            return Ok(patients);
        }

        // GET: /api/Patient/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .Select(p => new PatientListDto
                {
                    Id = p.Id,
                    FullName = p.User.FullName,
                    Diagnosis = p.Diagnosis,
                    DischargeDate = p.DischargeDate
                })
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // POST: /api/Patient
        [HttpPost]
        public async Task<IActionResult> Create(Patient patient)
        {
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
            return Ok(patient);
        }

        // PUT: /api/Patient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient updated)
        {
            var existing = await _db.Patients.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Diagnosis = updated.Diagnosis ?? existing.Diagnosis;
            existing.DischargeDate = updated.DischargeDate ?? existing.DischargeDate;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: /api/Patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            _db.Patients.Remove(patient);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Silindi" });
        }

        // GET: /api/Patient/{id}/medications
        [HttpGet("{id}/medications")]
        public async Task<IActionResult> GetMedicationsByPatient(int id)
        {
            var meds = await _db.Medications
                .Where(m => m.PatientId == id)
                .Include(m => m.DoseSchedules)
                .Include(m => m.MedicationRecords)
                .ToListAsync();

            return Ok(meds);
        }
    }
}
