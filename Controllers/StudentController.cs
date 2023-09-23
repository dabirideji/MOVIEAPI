using System.Security.Claims;
using APICLASS.Models;
using APICLASS.Response;
using BCrypt.Net;
// using System.IdentityModel.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOVIEAPI.Data;
using MOVIEAPI.Models;
using SQLitePCL;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;

namespace MOVIEAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly MovieDbContext _context;

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public StudentController(MovieDbContext context, ILogger<StudentController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }


        [HttpPost]
        [Route("login")]
        public ActionResult<DefaultResponse<Student>> StudentLogin([FromBody] StudentLoginDto dto)
        {
            DefaultResponse<Student> response = new();
            // var result=await _context.Students.Where(x=>x.Username==dto.Username&&BCrypt.Net.BCrypt.Verify(dto.Password,x.Password)).ToListAsync();
            var result = _context.Students.Where(x => x.Username == dto.Username).FirstOrDefault();
            if (result == null)
            {
                return BadRequest("USER NOT FOUND");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, result.Password))
            {
                return BadRequest("WRONG PASSWORD");

            }
            string token = GenerateToken(result);
            response.Status = true;
            response.ResponseMessage = "Student Found Successfully";
            response.ResponseCode = "00";
            response.Data = result;
            _logger.LogInformation("Student Login Successfully: " + token);
            return Ok(response);
        }



        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<DefaultResponse<Movie>>> AddStudent([FromBody] StudentDto student)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(student.Password);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DefaultResponse<string> response = new();
            try
            {
                var stud = new Student()
                {
                    Name = student.Name,
                    Username = student.Username,
                    Password = passwordHash,
                };
                await _context.Students.AddAsync(stud);
                await _context.SaveChangesAsync();
                response.Status = true;
                response.ResponseMessage = "Data Saved Successfully";
                response.ResponseCode = "00";
                // response.Data = response.ResponseMessage;
                _logger.LogInformation("Data Saved Successfully");

                return Ok(stud);
                // response.Data = movie;
                // return StatusCode(201,response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ResponseMessage = "An Error Occuured";
                response.ResponseCode = "99";
                _logger.LogError("An Error Occured", ex);
                return StatusCode(500, response);
            };



        }

        private string GenerateToken(Student stu)
        {
            List<Claim> claims = new List<Claim>{
        new Claim(ClaimTypes.Name,stu.Username)
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:TOKEN").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }

}