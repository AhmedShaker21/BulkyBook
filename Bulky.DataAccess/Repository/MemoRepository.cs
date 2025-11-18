using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class MemoRepository : GenericRepository<Memo>, IMemoRepository
    {
        private readonly ApplicationDbContext _context;

        public MemoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Memo obj)
        {
            _context.Memos.Update(obj);
        }
    }
}
