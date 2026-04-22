using Microsoft.AspNetCore.Mvc;
using RoomsReservationsApi.Data;
using RoomsReservationsApi.Models;

namespace RoomsReservationsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetAll()
    {
        return Ok(AppData.Rooms);
    }
    
    [HttpGet("{id}")]
    public ActionResult<Room> GetById(int id)
    {
        var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
            return NotFound();

        return Ok(room);
    }
    
    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<Room>> GetByBuilding(string buildingCode)
    {
        var rooms = AppData.Rooms
            .Where(r => r.BuildingCode.ToLower() == buildingCode.ToLower())
            .ToList();

        return Ok(rooms);
    }
    
    [HttpGet("filter")]
    public ActionResult<IEnumerable<Room>> Filter(
        [FromQuery] int? minCapacity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        var query = AppData.Rooms.AsQueryable();

        if (minCapacity.HasValue)
            query = query.Where(r => r.Capacity >= minCapacity.Value);

        if (hasProjector.HasValue)
            query = query.Where(r => r.HasProjector == hasProjector.Value);

        if (activeOnly == true)
            query = query.Where(r => r.IsActive);

        return Ok(query.ToList());
    }
    
    [HttpPost]
    public ActionResult<Room> Create(Room room)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        room.Id = AppData.Rooms.Max(r => r.Id) + 1;

        AppData.Rooms.Add(room);

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, Room updatedRoom)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
            return NotFound();

        room.Name = updatedRoom.Name;
        room.BuildingCode = updatedRoom.BuildingCode;
        room.Floor = updatedRoom.Floor;
        room.Capacity = updatedRoom.Capacity;
        room.HasProjector = updatedRoom.HasProjector;
        room.IsActive = updatedRoom.IsActive;

        return Ok(room);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
            return NotFound();

        AppData.Rooms.Remove(room);

        return NoContent();
    }
}