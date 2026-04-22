using Microsoft.AspNetCore.Mvc;
using RoomsReservationsApi.Data;
using RoomsReservationsApi.Models;

namespace RoomsReservationsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Reservation>> GetAll()
    {
        return Ok(AppData.Reservations);
    }
    
    [HttpGet("{id}")]
    public ActionResult<Reservation> GetById(int id)
    {
        var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }
    
    [HttpGet("filter")]
    public ActionResult<IEnumerable<Reservation>> Filter(
        [FromQuery] DateTime? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        var query = AppData.Reservations.AsQueryable();

        if (date.HasValue)
            query = query.Where(r => r.Date.Date == date.Value.Date);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status.ToLower() == status.ToLower());

        if (roomId.HasValue)
            query = query.Where(r => r.RoomId == roomId.Value);

        return Ok(query.ToList());
    }
    
    [HttpPost]
    public ActionResult<Reservation> Create(Reservation reservation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var room = AppData.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null)
            return BadRequest("Room does not exist");
        
        if (!room.IsActive)
            return BadRequest("Room is not active");
        
        if (!reservation.IsTimeValid())
            return BadRequest("EndTime must be after StartTime");
        
        var conflict = AppData.Reservations.Any(r =>
            r.RoomId == reservation.RoomId &&
            r.Date.Date == reservation.Date.Date &&
            reservation.StartTime < r.EndTime &&
            reservation.EndTime > r.StartTime
        );

        if (conflict)
            return Conflict("Time conflict with another reservation");
        
        reservation.Id = AppData.Reservations.Any()
            ? AppData.Reservations.Max(r => r.Id) + 1
            : 1;

        AppData.Reservations.Add(reservation);

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, Reservation updated)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
            return NotFound();

        if (!updated.IsTimeValid())
            return BadRequest("Invalid time");

        reservation.RoomId = updated.RoomId;
        reservation.OrganizerName = updated.OrganizerName;
        reservation.Topic = updated.Topic;
        reservation.Date = updated.Date;
        reservation.StartTime = updated.StartTime;
        reservation.EndTime = updated.EndTime;
        reservation.Status = updated.Status;

        return Ok(reservation);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
            return NotFound();

        AppData.Reservations.Remove(reservation);

        return NoContent();
    }
    
}