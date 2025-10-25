using Microsoft.EntityFrameworkCore;
using PharmaGo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PharmaGo.DataAccess.Repositories
{
    public class ReservaRepository : BaseRepository<Reserva>
    {
        private readonly PharmacyGoDbContext _context;

        public ReservaRepository(PharmacyGoDbContext context) : base(context) 
        {
            _context = context;
        }

        public override Reserva GetOneByExpression(Expression<Func<Reserva, bool>> expression)
        {
            return _context.Set<Reserva>().FirstOrDefault(expression);
        }

        public override void InsertOne(Reserva reserva)
        {
            _context.Entry(reserva).State = EntityState.Added;
            _context.Set<Reserva>().Add(reserva);
        }
    }
}
