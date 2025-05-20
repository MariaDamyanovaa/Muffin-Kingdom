using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Muffin_Kingdon.Data;

namespace MVC_Muffin_Kingdon.Controllers
{

    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private const string LastSeenOrderIdKey = "LastSeenOrderId";

        public OrdersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            IQueryable<Order> ordersQuery;

            if (User.IsInRole("Admin"))
            {
                ordersQuery = _context.Orsers
                    .Include(o => o.Users)
                    .Include(o => o.Products);
            }
            else
            {
                var currentUserId = _userManager.GetUserId(User);
                ordersQuery = _context.Orsers
                    .Include(o => o.Users)
                    .Include(o => o.Products)
                    .Where(x => x.UserId == currentUserId);
            }

            var lastSeenIdStr = HttpContext.Session.GetString(LastSeenOrderIdKey);

            // Ако не е зададено нищо в сесията, го задаваме с най-високото Id – така скриваме всички стари
            if (string.IsNullOrEmpty(lastSeenIdStr))
            {
                int currentMaxId = (await _context.Orsers.Select(o => o.Id).ToListAsync()).DefaultIfEmpty(0).Max();

                HttpContext.Session.SetString(LastSeenOrderIdKey, currentMaxId.ToString());
                return View(new List<Order>()); // Показваме празен списък
            }

            if (int.TryParse(lastSeenIdStr, out int lastSeenOrderId))
            {
                ordersQuery = ordersQuery.Where(o => o.Id > lastSeenOrderId);
            }

            var orders = await ordersQuery.OrderByDescending(o => o.DateReg).ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public IActionResult HideAll()
        {
            //int maxId = _context.Orsers.Select(o => o.Id).DefaultIfEmpty(0).Max();
            int maxId = _context.Orsers.Any() ? _context.Orsers.Max(o => o.Id) : 0;
            HttpContext.Session.SetString(LastSeenOrderIdKey, maxId.ToString());

            TempData["Message"] = "Текущата поръчка е завършена успешно!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ResetView()
        {
            HttpContext.Session.Remove(LastSeenOrderIdKey);
            TempData["Message"] = "Изгледът е нулиран.";
            return RedirectToAction("Index");
        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orsers
                .Include(o => o.Products)
                .Include(o => o.Users)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Quantity,DateReg,Description")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.DateReg = DateTime.Now;
                order.UserId = _userManager.GetUserId(User);
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", order.ProductId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orsers.FindAsync(id);
            if (order == null)
                return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", order.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,Quantity,DateReg,Description")] Order order)
        {
            if (id != order.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", order.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orsers
                .Include(o => o.Products)
                .Include(o => o.Users)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orsers.FindAsync(id);
            if (order != null)
            {
                _context.Orsers.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orsers.Any(e => e.Id == id);
        }
    }
}
