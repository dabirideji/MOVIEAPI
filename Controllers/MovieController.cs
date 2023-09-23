using Microsoft.AspNetCore.Mvc;
using APICLASS.Data;
using APICLASS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using APICLASS.Response;
using System.Data;
namespace APICLASS.Controllers;

[ApiController]
[Route("api/movie")]
public class MovieController : ControllerBase
{

    private readonly MovieDbContext _context;

    private readonly ILogger _logger;
    public MovieController(MovieDbContext context, ILogger<MovieController> logger)
    {
        _context = context;
        _logger = logger;
    }



    // [HttpGet]
    // public async Task<ActionResult> GetMovies()
    // {
    //     var movies = await _context.Movies.ToListAsync();
    //     return Ok(movies);
    // }



    // [HttpGet("id")]
    // public ICollection<Movie> GetMoviesByName([FromHeader] Guid id)
    // {
    //     var movies = _context.Movies.Where(x => x.id == id).ToList();
    //     return movies;
    // }




    /*
    -----------------------------------------------------------------------------------------------------------------------
    THIS METHOD IS MEANT TO GET ALL MOVIES IN THE DATABASE
    -----------------------------------------------------------------------------------------------------------------------

    */

    [HttpGet]
    public async Task<ActionResult<DefaultResponse<List<Movie>>>> GetMovies()
    {
        DefaultResponse<Movie> response = new();

        try
        {
            var result = await _context.Movies.ToListAsync();
            if (result == null || result.Count == 0)
            {
                _logger.LogError("No Data Found");
                response.ResponseMessage = "No Data Found";
                response.ResponseCode = "00";
                response.Status = false;
                return NotFound(response);
            }
            _logger.LogInformation("Data Found Successfully");
            response.ResponseMessage = "Data found Successfully";
            response.ResponseCode = "00";
            response.Status = true;
            return Ok(result);
            // return StatusCode(200, result);
        }
        catch (System.Exception Ex)
        {
            _logger.LogError("Server Error");
            response.ResponseMessage = "Something Went Wrong";
            response.ResponseCode = "99";
            response.Status = false;
            return StatusCode(404, Ex);

            throw;
        }
        // return Ok(movies);
    }




    /*
    -----------------------------------------------------------------------------------------------------------------------
    THIS METHOD IS MEANT TO GET A MOVIE IN THE DATABASE BASED ON THE ID SPECIFIED
    -----------------------------------------------------------------------------------------------------------------------

    */

    [HttpGet("{id}")]
    public async Task<ActionResult<DefaultResponse<Movie>>> GetMovie(Guid id)
    {
        var result = await _context.Movies.FindAsync(id);
        DefaultResponse<Movie> response = new();

        if (result == null)
        {
            response.ResponseCode = "99";
            response.ResponseMessage = "Not Found";
            response.Status = false;
            _logger.LogError("Data Not Found");
            return NotFound(response);
        }
        response.ResponseCode = "00";
        response.ResponseMessage = "Movie Found";
        response.Status = true;
        _logger.LogInformation("Data Found Successfully");
        return Ok(result);
        //         response.Data = result;
        // return StatusCode(201,response);
    }



    /*
        -----------------------------------------------------------------------------------------------------------------------
        THIS METHOD IS MEANT TO ADD A MOVIES TO THE DATABASE
        -----------------------------------------------------------------------------------------------------------------------

    */


    [HttpPost]
    public async Task<ActionResult<DefaultResponse<string>>> AddMovie([FromBody] MovieDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        DefaultResponse<string> response = new();
        try
        {
            var movie = new Movie()
            {
                Title = dto.Title,
                Genere = dto.Genere,
                RebaseDate = dto.RebaseDate

            };
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
            response.Status = true;
            response.ResponseMessage = "Data Saved Successfully";
            response.ResponseCode = "00";
            response.Data = response.ResponseMessage;
            _logger.LogInformation("Data Saved Successfully");

            return Ok(response);
            // response.Data = movie;
            // return StatusCode(201,response);
        }
        catch (Exception ex)
        {
            response.Status = true;
            response.ResponseMessage = "An Error Occuured";
            response.ResponseCode = "00";
            _logger.LogError("An Error Occured", ex);
            return StatusCode(500, response);
        };



    }

    /*
     -----------------------------------------------------------------------------------------------------------------------
     THIS METHOD IS MEANT TO DELETE A MOVIE IN THE DATABASE BASED ON THE ID SPECIFIED
     -----------------------------------------------------------------------------------------------------------------------

     */
    [HttpDelete("{id}")]
    public async Task<ActionResult<DefaultResponse<Movie>>> DeleteMovie(Guid id)
    {
        var result = await _context.Movies.FindAsync(id);
        DefaultResponse<string> response = new();

        if (result == null)
        {
            response.ResponseCode = "99";
            response.ResponseMessage = "Not Found";
            response.Status = false;
            _logger.LogError("Data Not Found");
            return NotFound(response);
        }
        response.ResponseCode = "00";
        response.ResponseMessage = "Movie Found";
        response.Status = true;
        _context.Movies.Remove(result);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Data Deleted Successfully");
        return NoContent();
    }




    /*
       -----------------------------------------------------------------------------------------------------------------------
       THIS METHOD IS MEANT TO UPDATE A MOVIE IN THE DATABASE BASED ON THE ID SPECIFIED
       -----------------------------------------------------------------------------------------------------------------------

       */

    [HttpPut("{id}")]
    public async Task<ActionResult<DefaultResponse<Movie>>> UpdateMovie(Guid id, [FromBody] MovieDto mov)
    {
        var movie = await _context.Movies.FindAsync(id);
        DefaultResponse<string> response = new();
        if (movie == null)
        {
            response.ResponseCode = "99";
            response.ResponseMessage = "Not Found";
            response.Status = false;
            _logger.LogError("Movie Not Found");
            return NotFound(response);
        }
        movie.Title = mov.Title;
        movie.Genere = mov.Genere;
        movie.RebaseDate = mov.RebaseDate;
        try
        {
            _context.Entry(movie).State = EntityState.Modified;
            response.ResponseCode = "00";
            response.ResponseMessage = "Data Updated Successfully";
            response.Status = true;
            _logger.LogInformation("Movie Updated Successfully");
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovieExists(id))
            {
                response.ResponseCode = "99";
                response.ResponseMessage = "Not Found";
                response.Status = false;
                _logger.LogError("Movie Not Found");
                return NotFound(response);
            }
            else
            {
                // throw;
                return Conflict();
            }

        }
    }


    /*
        -----------------------------------------------------------------------------------------------------------------------
        THIS PRIVATE METHOD IS MEANT TO HELP CHECK IF A DATA IS PRESENT IN THE DATABASE BASED ON THE ID SPECIFIED
        -----------------------------------------------------------------------------------------------------------------------

        */
    private bool MovieExists(Guid id)
    {
        return _context.Movies.Any(e => e.id == id);
    }






























































































































































    /*
        -----------------------------------------------------------------------------------------------------------------------
        THE CODE BELOW THIS LINE REPRESENTS MY PREVIOUS TRIALS OF THE API . I THINK IT IS ALSO SYNCHRONOUS
        -----------------------------------------------------------------------------------------------------------------------
    */




















    // [HttpDelete("id")]
    // public async Task<ActionResult> DeleteMovie([FromHeader] Guid id)
    // {
    //     var movies = await _context.Movies.FindAsync(id);
    //     if (movies == null)
    //     {
    //         return NotFound();
    //     }
    //     _context.Movies.Remove(movies);
    //     await _context.SaveChangesAsync();
    //     // return NoContent();
    //     return StatusCode(500, "sata ");
    // }





    // [HttpGet]
    // // []
    //     public async Task<ActionResult<DefaultResponse<Movie>>> GetMoviesByName([FromHeader]Guid id){
    //         DefaultResponse<Movie> response=new();
    //          var movies=await _context.Movies.FindAsync(id);

    //         if(movies==null){
    //             response.Status=false;
    //             response.ResponseMessage="Movie not found";
    //             response.ResponseCode="99";
    //             return StatusCode(404,response);
    //         }
    //         response.Status=true;
    //             response.ResponseMessage="Found Movie";
    //             response.ResponseCode="00";
    //             return StatusCode(404);
    //         // var movies =  _context.Movies.Where(x=>x.id==id).ToList();
    //         // return StatusCode()
    //     }

    // public ICollection<Movie> GetMoviesByName([FromHeader]Guid id){
    //     var movies =  _context.Movies.Where(x=>x.id==id).ToList();
    //     return movies;
    // }

    // [HttpPut("id")]
    // public async Task<ActionResult> UpdateMovie([FromHeader] Guid id, [FromBody] MovieDto dto)
    // {

    //     var movies = await _context.Movies.FindAsync(id);
    //     //   var movies = await _context.Movies.Where(x=>x.id==id).ToListAsync();
    //     if (movies == null)
    //     {
    //         return NotFound();
    //     }
    //     var movie = new Movie()
    //     {
    //         id = id,
    //         Title = dto.Title,
    //         Genere = dto.Genere,
    //         RebaseDate = dto.RebaseDate
    //     };
    //     // _context.Entry("Movies").State=EntityState.Modified;
    //     // _context.Movies.Where(x=>x.Title==dto.Title).;
    //     // await _context.SaveChangesAsync();
    //     // return NoContent();
    //     // var movies=_context.Movies.Where(x=>x.id==id).ToList();
    //     _context.Entry("Movies").State = EntityState.Modified;
    //     // _context.Movies.Update(movie);
    //     try
    //     {
    //         await _context.SaveChangesAsync();
    //     }
    //     catch (DbUpdateConcurrencyException)
    //     {
    //         if (!_context.Movies.Any(e => e.id == id))
    //         {
    //             return NotFound();
    //         }
    //         else
    //         {
    //             throw;
    //         }

    //     }
    //     return NoContent();

    //     // // var movie=new Movie(){
    //     // //     Title=dto.Title,
    //     // //     Genere=dto.Genere,
    //     // //     RebaseDate=dto.RebaseDate
    //     // // };
    //     // _context.Entry("Movies").State=EntityState.Modified;
    //     // _context.Movies.Where(x=>x.Title==dto.Title).;
    //     // await _context.SaveChangesAsync();
    //     // return NoContent();
    // }





};





















