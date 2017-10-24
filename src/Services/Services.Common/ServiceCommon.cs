﻿using LinqKit;
using Rhyous.Odata;
using Rhyous.WebFramework.Clients;
using Rhyous.WebFramework.Interfaces;
using Rhyous.WebFramework.RelatedEntities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Rhyous.WebFramework.Services
{
    /// <summary>
    /// A common service layer for all Entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TInterface">The entity interface type.</typeparam>
    /// <typeparam name="TId">The type of the Id property. Usually int, long, guid, string, etc...</typeparam>
    public class ServiceCommon<TEntity, TInterface, TId> : IServiceCommon<TEntity, TInterface, TId>
        where TEntity: class, TInterface, new()
        where TInterface : IId<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        public virtual IRepository<TEntity, TInterface, TId> Repo
        {
            get { return _Repo ?? (_Repo = RepositoryLoader.Load<TEntity, TInterface, TId>()); }
            set { _Repo = value; }
        } private IRepository<TEntity, TInterface, TId> _Repo;

        public IRelatedEntityManager<TEntity, TInterface, TId> RelatedEntityManager
        {
            get { return _RelatedEntityManager ?? (_RelatedEntityManager = new RelatedEntityManager<TEntity, TInterface, TId>()); }
            set { _RelatedEntityManager = value; }
        } private IRelatedEntityManager<TEntity, TInterface, TId> _RelatedEntityManager;

        /// <inheritdoc />
        public virtual int GetCount()
        {
            return Repo.Get().Count();
        }

        /// <inheritdoc />
        public virtual List<TInterface> Get(List<TId> ids)
        {
            return Repo.Get(ids).ToList();
        }

        /// <inheritdoc />
        public virtual List<TInterface> Get()
        {
            return Repo.Get().ToList();
        }

        /// <inheritdoc />
        public virtual TInterface Get(TId Id)
        {
            return Repo.Get(Id);
        }

        /// <inheritdoc />
        public virtual List<TInterface> Get(NameValueCollection parameters)
        {
            var filterString = parameters.Get("$filter", string.Empty);
            if (string.IsNullOrWhiteSpace(filterString))
                return Get(null,  parameters.Get("$top", -1), parameters.Get("$skip", -1));
            var builder = new FilterExpressionBuilder<TEntity>(filterString, new FilterExpressionParser<TEntity>());
            return Get(builder.Expression, parameters.Get("$top", -1), parameters.Get("$skip", -1));
        }

        /// <inheritdoc />
        public virtual List<TInterface> Get(Expression<Func<TEntity, bool>> expression, int take = -1, int skip = -1)
        {
            return Repo.GetByExpression(expression ?? PredicateBuilder.New<TEntity>(true), e => e.Id).IfSkip(skip).IfTake(take).ToList();
        }

        /// <inheritdoc />
        public virtual List<TInterface> Get(Func<IQueryable<TInterface>, List<TInterface>> queryableModifier )
        {
            return queryableModifier(Repo.Get());
        }

        /// <inheritdoc />
        public virtual List<TInterface> Get(Func<IQueryable<TInterface>, List<TInterface>> queryableModifier, Expression<Func<TEntity, bool>> expression)
        {
            var queryable = Repo.GetByExpression(expression ?? PredicateBuilder.New<TEntity>(true));
            return queryableModifier(queryable);
        }

        /// <inheritdoc />
        public virtual string GetProperty(TId Id, string property)
        {
            return Repo.Get(Id)?.GetPropertyValue(property)?.ToString();
        }

        /// <inheritdoc />
        public virtual string UpdateProperty(TId id, string property, string value)
        {
            var entity = new TEntity() { Id = id};
            var changedProperties = new List<string>() { property };
            var type = entity.GetPropertyInfo(property).PropertyType;
            var typedValue = Convert.ChangeType(value, type);
            entity.GetPropertyInfo(property).SetValue(entity, typedValue);
            return Update(id, entity, changedProperties).GetPropertyValue(property).ToString();
        }

        /// <inheritdoc />
        public virtual List<TInterface> Add(IList<TInterface> entities)
        {
            return Repo.Create(entities);
        }

        /// <inheritdoc />
        public virtual TInterface Add(TInterface entity)
        {
            return Repo.Create(new[] { entity }).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TInterface Update(TId id, TInterface entity, List<string> changedProperties)
        {
            entity.Id = id;
            return Repo.Update(entity, changedProperties);
        }

        /// <inheritdoc />
        public virtual TInterface Replace(TId Id, TInterface entity)
        {
            var allProperties = from prop in typeof(TInterface).GetProperties()
                                where prop.CanRead && prop.CanWrite && prop.Name != "Id"
                                select prop.Name;
            return Repo.Update(entity, allProperties);
        }

        /// <inheritdoc />
        public virtual bool Delete(TId id)
        {
            return Repo.Delete(id);
        }

        /// <inheritdoc />
        public virtual RelatedEntityCollection GetRelatedEntities(TInterface entity)
        {
            return TaskRunner.RunSynchonously(RelatedEntityManager.GetRelatedEntitiesAsync, entity, new List<string>());
        }

        /// <inheritdoc />
        public virtual List<RelatedEntityCollection> GetRelatedEntities(IEnumerable<TInterface> entities)
        {
            return TaskRunner.RunSynchonously(RelatedEntityManager.GetRelatedEntitiesAsync, entities, new List<string>());
        }
    }
}