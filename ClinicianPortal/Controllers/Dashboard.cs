using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Globalization;
using ClinicianPortal.Models;
using ClinicianPortal.Models.EntityModel;
using Microsoft.Extensions.Logging;


namespace ClinicianPortal.Controllers
{
    public class Dashboard : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public Dashboard(IWebHostEnvironment env,AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public bool SaveNote([FromBody] System.Text.Json.JsonElement data)
        {
            var newEvent = new Notes
            {
                Patient_Id= data.GetProperty("id").GetString(),
                create_date= DateTime.Now,
                notes = data.GetProperty("notes").GetString()
            };
           _context.Notes.Add(newEvent);
           _context.SaveChanges();
            return true;
        }

        // GET: Dashboard/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dashboard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Dashboard/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dashboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Dashboard/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dashboard/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public class IdRequest
        {
            public string id { get; set; }
        }

        [HttpPost]
        public JsonResult GetPatientData([FromBody] System.Text.Json.JsonElement data)
        {   // Your logic
            string id = data.GetProperty("id").GetString();
            string folder = Path.Combine(_env.WebRootPath, "Patient-Data", "CSV-Data");
            var metrics = CalculateMetricsByDate(folder,id);
            var list = metrics.Select(m => new
            {
                Date = m.Key.ToString("dd/MM/yyyy"),
                Peak = m.Value.Peak,
                AvgPressure = m.Value.AvgPressure,
                ContactArea = m.Value.ContactArea
            }).ToList();

            return Json(list);

        }



        public Dictionary<DateOnly, (double Peak, double AvgPressure, double ContactArea)>
            CalculateMetricsByDate(string folderPath, string patientId)
        {
            var result = new Dictionary<DateOnly, (double Peak, double AvgPressure, double ContactArea)>();

            var files = Directory.GetFiles(folderPath, $"{patientId}_*.csv");

            foreach (var file in files)
            {
                using var reader = new StreamReader(file);

                string? header = reader.ReadLine(); // Skip header
                if (header == null) continue;

                // Extract date from "12345_20250101.csv"
                string fileName = Path.GetFileNameWithoutExtension(file);
                string datePart = fileName.Split('_').Last();
                if (!DateOnly.TryParseExact(datePart, "yyyyMMdd", out DateOnly fileDate))
                    continue;

                double peak = double.MinValue;
                double sumPressure = 0;
                int count = 0;
                int contactArea = 0;

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');

                    if (parts.Length < 2) continue;

                    if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double pressure))
                        continue;

                    // Peak
                    if (pressure > peak)
                        peak = pressure;

                    // Avg Pressure
                    sumPressure += pressure;
                    count++;

                    // Contact Area (pressure > 0)
                    if (pressure > 0)
                        contactArea++;
                }

                double avgPressure = count > 0 ? sumPressure / count : 0;

                result[fileDate] = (Peak: peak, ContactArea: contactArea, AvgPressure: avgPressure);
            }
             
            return result;
        }
    }
}

