using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICLASS.Models;
using MOVIEAPI.Models;

public class MovieDbContext : DbContext{
        public MovieDbContext(DbContextOptions options): base(options)
        {
            
        }

        public DbSet<Movie> Movies{get; set;}
        public DbSet<Student>Students{get;set;}

    }

