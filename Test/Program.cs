using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dapper;
using HZC.Data.Dapper.Common;
using HZC.Data.Dapper.Extensions;
using HZC.Data.Dapper.SqlBuilders;
using Newtonsoft.Json;
using RMES.Entity;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new MyDbContext("Data Source=.;Database=Ruanmou.Bbs;User Id=sa;Password=790825", () => 1);



            //var topic = dbContext.Load<Topic>(14);
            //topic.Title = "测试修改的主题";
            //dbContext.Update(topic);

            //topic = dbContext.Load<Topic>(14);
            //Console.WriteLine(topic.Title);

            //var data = dbContext.ToDo(conn =>
            //{
            //    var sql = "SELECT a.*,b.* FROM Topics a LEFT JOIN Users b ON a.CreateBy=b.Id";
            //    return conn.Query<Topic, User, Topic>(sql, (t, u) =>
            //    {
            //        t.Creator = u;
            //        return t;
            //    });
            //});

            var data = dbContext.PageList<Topic, User>(
                2,
                3,
                "Topics LEFT JOIN Users ON Topics.CreateBy=Users.Id",
                "Topics.*,Users.*",
                (t, u) => {
                    t.Creator = u;
                    return t;
                },
                SoftDeleteConditionBuilder.New("Topics").AndEqual("CreateBy", 1), 
                SortBuilder.New().OrderByDesc("Id", "Topics"), 
                out var total
            );

            Console.WriteLine($"记录总数：{total}");
            Console.WriteLine(JsonConvert.SerializeObject(data));

            //foreach (var topic in data)
            //{
            //    Console.WriteLine($"{topic.Id} {topic.Title} {topic.Creator.NickName}");
            //}

            //Console.WriteLine(total);

            //var builder = SoftDeleteConditionBuilder.New();
            //builder.AndEqual("ClazzId", 1);

            //Console.WriteLine(builder.ToCondition());

            //var topics = dbContext.Fetch<Topic>(ConditionBuilder.New(), SortBuilder.New());

            //foreach (var topic in topics)
            //{
            //    Console.WriteLine(topic.Title);
            //}

            //var result = dbContext.Delete<Topic>(8);
            //if (result)
            //{
            //    Console.WriteLine("删除成功");
            //}

            //var topics = dbContext.Fetch<Topic>(ConditionBuilder.New(), SortBuilder.New());

            //foreach (var topic in topics)
            //{
            //    Console.WriteLine($"{topic.Id} {topic.Title}");
            //}

            //var result = dbContext.Set<Topic>(FieldValuePairs.New().Add("Title=Title+'abcde'"), ConditionBuilder.New().AndEqual("Id", 14));

            //var rows = dbContext.ToDo(conn =>
            //{
            //    conn.Open();
            //    var result = 0;
            //    using (var trans = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            var i1 = conn.Set<Topic, int>(
            //                FieldValuePairs.New().Add("Title=Title+'1234'"),
            //                ConditionBuilder.New().AndEqual("Id", 14), 
            //                null, 
            //                trans);

            //            var i2 = conn.Set<Topic, int>(
            //                FieldValuePairs.New().Add("Title=Title+'12345678'"),
            //                ConditionBuilder.New().AndEqual("Id", 13), 
            //                null, 
            //                trans);

            //            result = i1 + i2;
            //            trans.Commit();
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e.Message);
            //            trans.Rollback();
            //        }
            //        finally
            //        {
            //            conn.Close();
            //        }
            //    }

            //    return result;
            //});

            //var result = dbContext.TransToDo((conn, trans) =>
            //{
            //    conn.Set<Topic, int>(
            //        FieldValuePairs.New().Add("Title=Title+'好的'"),
            //        ConditionBuilder.New().AndEqual("Id", 14),
            //        null,
            //        trans);

            //    conn.Set<Topic, int>(
            //        FieldValuePairs.New().Add("Title=Title+'不好'"),
            //        ConditionBuilder.New().AndEqual("Id", 13),
            //        null,
            //        trans);
            //});

            //Console.WriteLine(result);

            //var topics = dbContext.PageList<Topic>(1, 3, ConditionBuilder.New(), SortBuilder.New(), out var total);

            //Console.WriteLine($"总计 {total} 条");
            //foreach (var topic in topics)
            //{
            //    Console.WriteLine($"{topic.Id} {topic.Title}");
            //}

            //var sw = new Stopwatch();

            //var input = new TopicSearchInput
            //{
            //    Key = "测试",
            //    ChannelId = 10,
            //    CreateAtStart = DateTime.Today
            //};

            //var count = 10000;

            //sw.Start();
            //var where = "";
            //for (int i = 0; i < count; i++)
            //{
            //    where = "1=1";
            //    var idx = 0;
            //    var parameters = new List<SqlParameter>();

            //    if (!string.IsNullOrWhiteSpace(input.Key))
            //    {
            //        where += $" AND (Title LIKE '%{input.Key}%' OR Content LIKE '%{input.Key}%')";
            //        parameters.Add(new SqlParameter($"@__p_{idx++}", input.Key));
            //    }

            //    if (input.ChannelId > 0)
            //    {
            //        where += $" AND ChannelId={input.ChannelId}";
            //        parameters.Add(new SqlParameter($"@__p_{idx++}", input.Key));
            //    }

            //    if (input.CreateAtStart != null)
            //    {
            //        where += $" AND CreateAt>'{input.CreateAtStart.ToString()}'";
            //        parameters.Add(new SqlParameter($"@__p_{idx++}", input.Key));
            //    }

            //    if (input.CreateAtEnd != null)
            //    {
            //        where += $" AND CreateAt<'{input.CreateAtStart.ToString()}'";
            //        parameters.Add(new SqlParameter($"@__p_{idx++}", input.Key));
            //    }

            //    //Console.WriteLine(where);
            //}
            //sw.Stop();
            //Console.WriteLine($"第一种：{where} \r\n {sw.ElapsedMilliseconds}");

            //var builder2 = ConditionBuilder.New();

            //sw.Reset();
            //sw.Start();
            //var where2 = "";
            //for (var j = 0; j < count; j++)
            //{
            //    var builder = ConditionBuilder.New();

            //    if (!string.IsNullOrWhiteSpace(input.Key))
            //    {
            //        builder.AndContains(new[] { "Title", "Contents" }, input.Key);
            //    }

            //    if (input.ChannelId > 0)
            //    {
            //        builder.AndGreaterThan("ChannelId", input.ChannelId);
            //    }

            //    if (input.CreateAtStart != null)
            //    {
            //        builder.AndGreaterThanEqual("CreateAt", input.CreateAtStart);
            //    }

            //    if (input.CreateAtEnd != null)
            //    {
            //        builder.AndLessThanEqual("CreateAt", input.CreateAtEnd);
            //    }

            //    where2 = builder.ToCondition();
            //}
            //sw.Stop();
            //Console.WriteLine($"第二种：{where2} \r\n {sw.ElapsedMilliseconds}");

            Console.Read();
        }
    }

    public class TopicSearchInput
    {
        public string Key { get; set; }

        public int ChannelId { get; set; } 

        public DateTime? CreateAtStart { get; set; }

        public DateTime? CreateAtEnd { get; set; }
    }
}
