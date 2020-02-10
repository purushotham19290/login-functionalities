using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace mvcproject.Models
{
    public class Logindbcontext:DbContext
    {
        public Logindbcontext() : base("con") { }
        //public virtual DbSet<Login> logins { get; set; }
        public virtual DbSet<Student> students { get; set; }
        public virtual DbSet<NewUser> newusers { get; set; }

        public DbSet<ResetPassword> ResetPasswords { get; set; }
        
        // public virtual DbSet<ResetPassword> resetpasswords { get; set; }

    }
}