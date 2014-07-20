using FluentNHibernate.Mapping;
using GW2.Server.Worker.Data.Entities;

namespace GW2.Server.Worker.Data.Mappings
{
    public abstract class GwEntityMapper<TEntity> : ClassMap<TEntity> where TEntity:GwEntity
    {
        protected GwEntityMapper()
        {
            Id(I => I.Id).GeneratedBy.Native();
        }
    }
}