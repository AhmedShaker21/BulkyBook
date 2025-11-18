using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISectorRepository Sector { get; }
        IDepartmentRepository Department { get; }
        IMemoRepository Memo { get; }

        void Save();
    }
}
