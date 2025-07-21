using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sakanat.Models;

namespace sakanat.Controllers
{
    public class ApartmentsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ApplicationDbContext _context;

        public ApartmentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Apartments
        public async Task<IActionResult> Index(string searchAddress, GenderType? genderType, decimal? minPrice, decimal? maxPrice)
        {
            var apartments = _context.Apartments.Include(a => a.Images).AsQueryable();

            if (!string.IsNullOrEmpty(searchAddress))
                apartments = apartments.Where(a => a.Address.Contains(searchAddress));

            if (genderType != null)
                apartments = apartments.Where(a => a.GenderType == genderType);

            if (minPrice != null)
                apartments = apartments.Where(a => a.Price >= minPrice);

            if (maxPrice != null)
                apartments = apartments.Where(a => a.Price <= maxPrice);

            return View(await apartments.ToListAsync());
        }

        // GET: Apartments/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Apartment apartment, List<IFormFile> NewImages)
        {
            if (ModelState.IsValid)
            {
                if (NewImages != null && NewImages.Count > 0)
                {
                    foreach (var image in NewImages)
                    {
                        if (image.Length > 0)
                        {
                            // مسار حفظ الصورة
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/apartments", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            apartment.Images.Add(new ApartmentImage
                            {
                                ImageUrl = "/images/apartments/" + fileName
                            });
                        }
                    }
                }

                _context.Add(apartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(apartment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var apartment = await _context.Apartments
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
                return NotFound();

            return View(apartment);
        }

        // POST: Apartments/Edit/5
        [HttpPost]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Apartment apartment, List<IFormFile> NewImages, List<int> DeletedImageIds)
        {
            if (id != apartment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // الصور الحالية من قاعدة البيانات
                    var existingApartment = await _context.Apartments
                        .Include(a => a.Images)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (existingApartment == null)
                        return NotFound();

                    // تحديث البيانات العامة
                    existingApartment.Title = apartment.Title;
                    existingApartment.Description = apartment.Description;
                    existingApartment.Price = apartment.Price;
                    existingApartment.Address = apartment.Address;
                    existingApartment.RoomCount = apartment.RoomCount;
                    existingApartment.GenderType = apartment.GenderType;

                    // حذف الصور المطلوبة
                    if (DeletedImageIds != null)
                    {
                        foreach (var imageId in DeletedImageIds)
                        {
                            var image = existingApartment.Images.FirstOrDefault(i => i.Id == imageId);
                            if (image != null)
                            {
                                // حذف من الملفات
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                                if (System.IO.File.Exists(filePath))
                                    System.IO.File.Delete(filePath);

                                _context.ApartmentImages.Remove(image);
                            }
                        }
                    }

                    // إضافة الصور الجديدة
                    if (NewImages != null && NewImages.Count > 0)
                    {
                        foreach (var image in NewImages)
                        {
                            if (image.Length > 0)
                            {
                                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/apartments", fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                existingApartment.Images.Add(new ApartmentImage
                                {
                                    ImageUrl = "/images/apartments/" + fileName
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Apartments.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(apartment);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment != null)
            {
                // حذف الصور من السيرفر (اختياري)
                foreach (var image in apartment.Images)
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
