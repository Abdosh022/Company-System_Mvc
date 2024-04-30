﻿using Demo.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Contexts
{
    public class MVCAppG01DbContext : IdentityDbContext<ApplicationUser>
    {
        public MVCAppG01DbContext(DbContextOptions<MVCAppG01DbContext> options):base(options)
        {
            
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer("Server = .; Database = MVCAppG01Db; Trusted_Connection = true");

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }

		//public DbSet<ApplicationUser> AspNetUsers { get; set; }
		//public DbSet<IdentityRole> AspNetRoles { get; set; }
	}
}
