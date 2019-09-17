namespace HZC.Data.Dapper.Common
{
    public interface IBaseEntity<TPrimary>
    {
        TPrimary Id { get; set; }
    }

    public class BaseEntity<TPrimaryKey> : IBaseEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}
