﻿using Rhyous.WebFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Rhyous.WebFramework.Interfaces
{
    public interface IServiceCommon<T, Tinterface, Tid>
        where T : class, Tinterface
        where Tinterface : IId<Tid>
    {
        IRepository<T, Tinterface, Tid> Repo { get; set; }
        List<Tinterface> Get();
        List<Tinterface> Get(List<Tid> ids);
        Tinterface Get(Tid id);
        List<Tinterface> Get(Expression<Func<Tinterface, bool>> expression);
        string GetProperty(Tid id, string property);
        Tinterface Update(Tid id, Tinterface entity, List<string> changedProperties);
        List<Tinterface> Add(IList<Tinterface> entity);
        Tinterface Replace(Tid id, Tinterface entity);
        bool Delete(Tid userId);
    }
}