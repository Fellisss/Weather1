using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather1.Data;
using Weather1.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Weather1.Controllers
{
    public class ObservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Observations/TodayByCity?city=Москва
        [HttpGet]
        public async Task<IActionResult> TodayByCity(string city)
        {
            // Если город не выбран, возвращаем пустую частичную view (частичка сама покажет подсказку)
            if (string.IsNullOrEmpty(city))
            {
                return PartialView("_TodayForecast", null);
            }

            // Определяем начало и конец сегодняшнего дня (включительно)
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Ищем последние наблюдения за сегодня для выбранного города
            var obs = await _context.Observations
                .Where(o => o.City == city && o.Timestamp >= today && o.Timestamp < tomorrow)
                .OrderByDescending(o => o.Timestamp)
                .FirstOrDefaultAsync();

            // Возвращаем частичное представление с моделью (может быть null)
            return PartialView("_TodayForecast", obs);
        }

        // ✅ Index — "Данные на сегодня" + список городов
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todays = await _context.Observations
                .Where(o => o.Timestamp >= today && o.Timestamp < tomorrow)
                .OrderByDescending(o => o.Timestamp)
                .ToListAsync();

            var latest = todays.FirstOrDefault();

            ViewBag.Latest = latest;
            ViewBag.TodayObservations = todays;

            ViewBag.AllCities = new List<string>
    {
        "Москва",
        "Санкт-Петербург",
        "Новосибирск",
        "Екатеринбург",
        "Казань",
        "Нижний Новгород",
        "Челябинск",
        "Самара",
        "Уфа",
        "Ростов-на-Дону"
    };

            return View();
        }

        // ✅ Archive — список всех записей с фильтрацией
        public async Task<IActionResult> Archive(string city, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Observations.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(o => o.City == city);

            if (startDate.HasValue)
                query = query.Where(o => o.Timestamp >= startDate.Value);

            if (endDate.HasValue)
            {
                var end = endDate.Value.AddDays(1);
                query = query.Where(o => o.Timestamp < end);
            }

            var result = await query
                .OrderByDescending(o => o.Timestamp)
                .ToListAsync();

            return View(result);
        }

        // ✅ Create GET
        public IActionResult Create()
        {
            var model = new Observation
            {
                Timestamp = DateTime.Now
            };
            return View(model);
        }

        // ✅ Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Observation observation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(observation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Наблюдение успешно сохранено!";
                return RedirectToAction(nameof(Index));
            }
            return View(observation);
        }

        // ✅ Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var obs = await _context.Observations.FindAsync(id);
            if (obs == null) return NotFound();
            return View(obs);
        }

        // ✅ Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Observation observation)
        {
            if (id != observation.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(observation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Наблюдение успешно изменено!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Observations.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(observation);
        }

        // ✅ Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var obs = await _context.Observations.FindAsync(id);
            if (obs == null) return NotFound();
            return View(obs);
        }

        // ✅ Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var observation = await _context.Observations.FindAsync(id);
            if (observation != null)
            {
                _context.Observations.Remove(observation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Наблюдение успешно удалено";
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ API: Температура + фильтр по городу
        [HttpGet]
        public async Task<IActionResult> TemperatureData(DateTime? from, DateTime? to, string city)
        {
            var q = _context.Observations.AsQueryable();
            if (from.HasValue) q = q.Where(o => o.Timestamp >= from.Value);
            if (to.HasValue) q = q.Where(o => o.Timestamp <= to.Value);
            if (!string.IsNullOrEmpty(city)) q = q.Where(o => o.City == city);

            var data = await q.OrderBy(o => o.Timestamp)
                .Select(o => new { timestamp = o.Timestamp.ToString("yyyy-MM-dd HH:mm"), temperature = o.Temperature })
                .ToListAsync();
            return Json(data);
        }


        // ✅ API: Влажность + фильтр по городу
        [HttpGet]
        public async Task<IActionResult> HumidityData(DateTime? from, DateTime? to, string city)
        {
            var q = _context.Observations.AsQueryable();
            if (from.HasValue) q = q.Where(o => o.Timestamp >= from.Value);
            if (to.HasValue) q = q.Where(o => o.Timestamp <= to.Value);
            if (!string.IsNullOrEmpty(city)) q = q.Where(o => o.City == city);

            var data = await q.OrderBy(o => o.Timestamp)
                .Select(o => new { timestamp = o.Timestamp.ToString("yyyy-MM-dd HH:mm"), humidity = o.Humidity })
                .ToListAsync();
            return Json(data);
        }
    }
}