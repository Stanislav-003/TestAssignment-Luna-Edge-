using ManagementSystem.Contracts;
using ManagmentSystem.Core.Abstractions;
using ManagmentSystem.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UsersResponse>>> GetUsers()
        { 
            var users = await _usersService.GetUsers();

            var response = users.Select(u => new UsersResponse(u.Id, u.UserName, u.Email));

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UsersRequest request)
        {
            var (user, error) = ManagmentSystem.Core.Models.User.Create(
                Guid.NewGuid(),
                request.userName,
                request.email,
                request.password);

            if (!string.IsNullOrEmpty(error))
            { 
                return BadRequest(error);
            }

            var userId = await _usersService.CreateUser(user);

            return Ok(userId);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateBook(Guid id, [FromBody] UsersRequest request)
        {
            var userId = await _usersService.UpdateUser(id, request.userName, request.email, request.password);

            return Ok(userId);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            return Ok(await _usersService.DeleteUser(id));
        }
    }
}
