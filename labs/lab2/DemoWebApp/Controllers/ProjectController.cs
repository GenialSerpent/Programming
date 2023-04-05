using DemoWebApp.Models;
using DemoWebApp.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DemoWebApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly UsersRepositoryService service;

        public UsersController(IRepository<User> service)
        {
            this.service = (UsersRepositoryService)service;
        }

        [HttpGet]
        public IActionResult Read(
            [FromQuery] string projectName,
            [FromQuery] int price = 0,
            [FromQuery] int rating = 1,
            [FromQuery] int perRating = 5)
            
        {
            var filterBy = new User()
            {
                Name = projectName,
            };

            return Ok(new Response()
            {
                Status = 200,
                Data = new
                {
                    count = service.Count(filterBy),
                    items = service.Read(filterBy, orderBy, order, page, perPage)
                }
            });
        }

        [HttpGet("{id}")]
        public IActionResult ReadById(int id)
        {
            return Ok(new Response()
            {
                Status = 200,
                Data = service.Read(id)
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            var errors = new Dictionary<string, List<string>>();
            if (service.UserNameExist(user))
            {
                errors.Add("Name", new List<string>() { "A user with this name already exists." });
            }

            if (errors.Count > 0)
            {
                return BadRequest(new Response()
                {
                    Status = 400,
                    Errors = errors
                });
            }

            return Created(nameof(User), new Response()
            {
                Status = 201,
                Data = service.Create(user)
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            service.Delete(id);

            return Ok(new Response()
            {
                Status = 200
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            user.Id = id;

            var errors = new Dictionary<string, List<string>>();
            if (service.UserNameExist(user))
            {
                errors.Add("Name", new List<string>() { "A user with this name already exists." });
            }

            if (errors.Count > 0)
            {
                return BadRequest(new Response()
                {
                    Status = 400,
                    Errors = errors
                });
            }

            service.Update(user);

            return Ok(new Response()
            {
                Status = 200
            });
        }
    }
}