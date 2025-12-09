using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Globalization;
using ClinicianPortal.Models;
using ClinicianPortal.Models.EntityModel;
using Microsoft.Extensions.Logging;
using ScottPlot;
using CsvHelper;
using System.IO;
using ScottPlot.Plottables;


namespace ClinicianPortal.Controllers
{
    public class Dashboard : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public Dashboard(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }
        //Geenrate heatmap code
        public void deleteHeatMap(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            var exts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };
            Directory.GetFiles(folderPath)
                     .Where(f => exts.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                     .ToList()
                     .ForEach(f => System.IO.File.Delete(f));
        }
        public async Task GenerateHeatmapFromCsvFiles()
        {
            string folder = Path.Combine(_env.WebRootPath, "Patient-Data", "CSV-Data");
            string heatmapPath = Path.Combine(_env.WebRootPath, "Patient-Data", "Heatmap");
            deleteHeatMap(heatmapPath);


            foreach (var csvFile in Directory.GetFiles(folder, "*.csv"))
            {
                var data = ReadCsvTo2DArray(csvFile);
                await CreateHeatmap(data, Path.GetFileNameWithoutExtension(csvFile), folder);
            }
        }
        public double[,] ReadCsvTo2DArray(string filePath)
        {
            var rows = new List<List<double>>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                while (csv.Read())
                {
                    var row = new List<double>();
                    for (int i = 0; csv.TryGetField<double>(i, out double val); i++)
                        row.Add(val);

                    rows.Add(row);
                }
            }

            int height = rows.Count;
            int width = rows[0].Count;
            double[,] result = new double[height, width];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    result[y, x] = rows[y][x];

            return result;
        }
        public async Task CreateHeatmap(double[,] data, string fileName, string wwwrootPath)
        {
            var plt = new ScottPlot.Plot();
            plt.Add.Heatmap(data);

            string savePath = Path.Combine(wwwrootPath, $"{fileName}.png");
            plt.SavePng(savePath, 600, 400);
        }

        public ActionResult Index()
        {
            string heatmapPath = Path.Combine(_env.WebRootPath, "Patient-Data", "Heatmap");
            var files = Directory.GetFiles(heatmapPath);
            if (files.Length == 0) { GenerateHeatmapFromCsvFiles(); }
            return View();
        }
        [HttpPost]
        public bool SaveNote([FromBody] System.Text.Json.JsonElement data)
        {
            var newEvent = new Notes
            {
                Patient_Id ="0", 
                create_date = DateTime.Now,
                notes = data.GetProperty("notes").GetString()
            };
            _context.Notes.Add(newEvent);
            _context.SaveChanges();
            return true;
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
        [HttpPost]
        public JsonResult GetPatientData([FromBody] System.Text.Json.JsonElement data)
        {   // Your logic
            string id = data.GetProperty("id").GetString();
            string folder = Path.Combine(_env.WebRootPath, "Patient-Data", "CSV-Data");
            var metrics = CalculateMetricsByDate(folder, id);
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

