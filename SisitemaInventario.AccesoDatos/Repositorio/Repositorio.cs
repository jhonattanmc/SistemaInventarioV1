using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio (ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        async Task IRepositorio<T>.Add(T entidad)
        {
            await dbSet.AddAsync(entidad); // insert into table
        }

        async Task<T> IRepositorio<T>.Get(int id)
        {
            return await dbSet.FindAsync(id);
        }

        async Task<IEnumerable<T>> IRepositorio<T>.GetAll(Expression<Func<T, bool>> filter, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby, string incluirPropiedades, bool isTracking)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter); // select * from where... 
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);
                }
            }
            if (orderby != null)
            {
                query = orderby(query);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        async Task<T> IRepositorio<T>.GetFirst(Expression<Func<T, bool>> filter, string incluirPropiedades, bool isTracking)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter); // select * from where... 
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // ejemplo "Categoria,Marca"
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        void IRepositorio<T>.Remove(T entidad)
        {
            dbSet.Remove(entidad);
        }

        void IRepositorio<T>.RemoveRange(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }


    }
}
