﻿using Rhyous.WebFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhyous.WebFramework.Services
{
    public class ServiceCommon<T, Tinterface> : IServiceCommon<T, Tinterface>
        where T: class
    {
        public virtual IRepository<T, Tinterface> Repo
        {
            get { return _Repo ?? (_Repo = RepositoryLoader.Load<T, Tinterface>()); }
            set { _Repo = value; }
        } private IRepository<T, Tinterface> _Repo;

        public virtual List<Tinterface> Get(List<int> ids)
        {
            return Repo.Get(ids).ToList();
        }

        public virtual List<Tinterface> Get()
        {
            return Repo.Get().ToList();
        }

        public virtual Tinterface Get(int Id)
        {
            return Repo.Get(Id);
        }

        public virtual string GetProperty(int Id, string property)
        {
            return Repo.Get(Id)?.GetPropertyValue(property)?.ToString();
        }

        public virtual Tinterface Add(Tinterface entity)
        {            
            return Repo.Create(entity);
        }

        public virtual Tinterface Update(int Id, Tinterface entity, List<string> changedProperties)
        {
            return Repo.Update(entity, changedProperties);
        }

        public virtual Tinterface Replace(int Id, Tinterface entity)
        {
            var allProperties = from prop in typeof(Tinterface).GetProperties()
                                where prop.CanRead && prop.CanWrite && prop.Name != "Id"
                                select prop.Name;
            return Repo.Update(entity, allProperties);
        }

        public virtual bool Delete(int id)
        {
            return Repo.Delete(id);
        }        
    }
}