using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ISectorRepository Sector { get; private set; }
        public IDepartmentRepository Department { get; private set; }
        public IMemoRepository Memo { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Sector = new SectorRepository(context);
            Department = new DepartmentRepository(context);
            Memo = new MemoRepository(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
