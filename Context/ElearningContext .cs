using Microsoft.EntityFrameworkCore;
using QtechBackend.Models;
using QtechBackendv2.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QtechBackend.Context
{
    public class ElearningContext : DbContext
    {


        public ElearningContext(DbContextOptions<ElearningContext> options) : base(options) { }

        public DbSet<Users> Employees { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Documentation> Documentations { get; set; }
        public DbSet<Enrolled> Enrolleds { get; set; }


    }
}
