using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {

        public EmployeeRepository(MVCAppG01DbContext dbContext):base(dbContext)
        {
        }
        public IQueryable<Employee> GetEmployeesByAddress(string address)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Employee> SearchEmployeeByName(string name)
            => _dbContext.Employees.Where(E => E.Name.ToLower().Contains(name.ToLower())).Include(E => E.Department);
    }
}
