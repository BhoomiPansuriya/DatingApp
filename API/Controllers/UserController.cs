namespace  API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserController(DataContext db) : ControllerBase {
    private readonly DataContext _context = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() {
            var user = await _context.Users.ToListAsync();
            return user;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id) {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
    }
}