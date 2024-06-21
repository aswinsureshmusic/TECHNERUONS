using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TECHNERUONS.Data;
using TECHNERUONS.Models;
using X.PagedList;

namespace TECHNERUONS.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly DBContext dbContext;
        public EmployeesController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";
            ViewData["CurrentFilter"] = searchString;

            var employees = from e in dbContext.Employees
                            select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Name.Contains(searchString) || e.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.Name);
                    break;
                case "Description":
                    employees = employees.OrderBy(e => e.Description);
                    break;
                case "description_desc":
                    employees = employees.OrderByDescending(e => e.Description);
                    break;
                default:
                    employees = employees.OrderBy(e => e.Name);
                    break;
            }
            int pageNumber = page ?? 1;
            int pageSize = 5; // Number of items to display per page
            var pagedEmployees = await employees.ToPagedListAsync(pageNumber, pageSize);
            return View(pagedEmployees);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployee)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployee.Name,
                Description = addEmployee.Description,
            };

            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null)
            {
                var viewModel = new UpdateEmployeeView()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Description = employee.Description,
                };
                return await Task.Run(() => View("View",viewModel));
            }
           
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeView model)
        {
            var employee = await dbContext.Employees.FindAsync(model.Id);
                if (employee != null)
            {
                employee.Name = model.Name;
                employee.Description = model.Description;

                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeView model)
        {
            var employee = await dbContext.Employees.FindAsync(model.Id);
            if (employee != null)
            {
                dbContext.Employees.Remove(employee);
                await dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }
    }
}
