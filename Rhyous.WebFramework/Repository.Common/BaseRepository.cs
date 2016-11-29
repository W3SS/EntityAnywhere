﻿using LinqKit;
using Rhyous.WebFramework.Interfaces;
using Rhyous.WebFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rhyous.WebFramework.Repositories
{
    public class BaseRepository<T, Tinterface> : IRepository<T, Tinterface>
        where Tinterface : IId
        where T : class, Tinterface, IId, new()
    {
        protected BaseDbContext<T> DbContext
        {
            get { return _DbContext ?? (DbContext = new BaseDbContext<T>()); }
            set { _DbContext = value; }
        } private BaseDbContext<T> _DbContext;

        public virtual Tinterface Create(Tinterface item)
        {
            Tinterface result = default(T);
            if (item != null)
            {
                var concrete = ConcreteConverter.ToConcrete<T, Tinterface>(item);
                result = DbContext.Entities.Add(concrete);
                DbContext.SaveChanges();
            }
            return result;
        }

        public virtual bool Delete(int id)
        {
            var item = DbContext.Entities.FirstOrDefault(o => o.Id == id);
            if (item == null)
                return true;
            DbContext.Entities.Remove(item);
            DbContext.SaveChanges();
            return true;
        }

        public virtual List<Tinterface> Get()
        {
            return DbContext.Entities.ToList<Tinterface>();
        }

        public virtual List<Tinterface> Get(List<int> ids)
        {
            return DbContext.Entities.Where(o => ids.Contains(o.Id)).ToList<Tinterface>();
        }

        public virtual Tinterface Get(int id)
        {
            return DbContext.Entities.FirstOrDefault(o => o.Id == id);
        }

        public virtual Tinterface Get(string name, Expression<Func<T, string>> propertyExpression)
        {
            return DbContext.Entities.AsExpandable().FirstOrDefault(e => propertyExpression.Invoke(e) == name);
        }

        public virtual List<Tinterface> Search(string searchString, params Expression<Func<T, string>>[] propertyExpressions)
        {
            var predicate = PredicateBuilder.New<T>();
            foreach (var expression in propertyExpressions)
            {
                predicate.Or(e => expression.Invoke(e).Contains(searchString));
            }
            return DbContext.Entities.AsExpandable().Where(predicate).ToList<Tinterface>();
        }

        public virtual Tinterface Update(Tinterface item, IEnumerable<string> changedProperties)
        {
            var existingItem = DbContext.Entities.FirstOrDefault(o => o.Id == item.Id);
            foreach (var prop in changedProperties)
            {
                var value = item.GetPropertyValue(prop);
                existingItem.GetPropertyInfo(prop).SetValue(existingItem, value);
            }
            DbContext.SaveChanges();
            return existingItem;
        }
    }
}