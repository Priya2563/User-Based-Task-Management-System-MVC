using Microsoft.AspNetCore.Mvc;
using TaskManagementSystemMVC.Data;
using TaskManagementSystemMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TaskManagementSystemMVC.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private AppDbContext _Context;
        public TaskController( AppDbContext Context) {
            _Context = Context;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TaskItems task)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                task.UserId = userId;
                _Context.Tasks.Add(task);
                _Context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(task);
            
        }
        public IActionResult List(string search,string status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = _Context.Tasks.Where(x=>x.UserId==userId);
            if (!string.IsNullOrEmpty(search))
            {
                tasks = tasks.Where(x=>x.Title.Contains(search));
            }
            if (!string.IsNullOrEmpty(status)) {
                tasks = tasks.Where(x=>x.Status==status);
            }
            return View(tasks.ToList());
        }
        public IActionResult Edit(int id) {  
          var task = _Context.Tasks.FirstOrDefault(x => x.Id == id);
            if(task == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (task.UserId != userId)   // ❌ dusre ka task
                return Unauthorized();
            return View(task); 
        }
        [HttpPost]
        public IActionResult Edit(TaskItems task)
        {
            var existingTask = _Context.Tasks.FirstOrDefault(x => x.Id == task.Id);
            if (existingTask == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (existingTask.UserId != userId)
                return Unauthorized();
            if (ModelState.IsValid)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.Status = task.Status;

                _Context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(task);
        }
        public IActionResult Delete(int id)
        {
            var task = _Context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
            {
                return NotFound();

            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (task.UserId != userId)   // ❌ dusre ka task
                return Unauthorized();
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = _Context.Tasks.FirstOrDefault(x=>x.Id==id);
            if (task == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (task.UserId != userId)
            {
                return Unauthorized();
            }
            _Context.Tasks.Remove(task);
            _Context.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalTask = _Context.Tasks.Count(x => x.UserId == userId);
            var completedTasks = _Context.Tasks.Count(x => x.UserId == userId && x.Status == "Completed");
            var pendingTasks = _Context.Tasks.Count(x => x.UserId == userId && x.Status != "Completed");
            ViewBag.Total=totalTask;
            ViewBag.Completed=completedTasks;
            ViewBag.Pending=pendingTasks;
            return View();
        }

    }
}
