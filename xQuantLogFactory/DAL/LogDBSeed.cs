using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SQLite.CodeFirst;

namespace xQuantLogFactory.DAL
{
    /// <summary>
    /// 数据库种子
    /// </summary>
    public class LogDBSeed : SqliteDropCreateDatabaseWhenModelChanges<LogDBContext>
    {
        public LogDBSeed(DbModelBuilder modelBuilder)
            : base(modelBuilder) { }

        protected override void Seed(LogDBContext context)
        {
            LogDBContext.UnityContext.SaveChanges();
            base.Seed(context);
        }
    }
}
