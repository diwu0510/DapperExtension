using System;

namespace HZC.Data.Dapper.Reflections
{
    public class CustomPropertyInfo
    {
        /// <summary>
        /// 属性的数据类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性对应的数据表的列明
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 是不是主键
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// 是不是导航属性。不是值类型和string类型的属性默认为导航属性
        /// </summary>
        public bool IsNavProperty { get; set; }

        /// <summary>
        /// 导航属性对应的外键名称，如果不指定，默认就是导航属性名称+Id如：
        /// LEFT JOIN Student.ClazzId=Clazz.Id
        /// 对应的是上面表达式中的 ClazzId
        /// </summary>
        public string ForeignKey { get; set; }

        /// <summary>
        /// 导航属性对应表的主键名称。如：
        /// LEFT JOIN Student.ClazzId=Clazz.Id
        /// 对应的是上面表达式中的 Id
        /// </summary>
        public string MasterKey { get; set; }

        /// <summary>
        /// 创建时忽略
        /// </summary>
        public bool InsertIgnore { get; set; }

        /// <summary>
        /// 更新时忽略
        /// </summary>
        public bool UpdateIgnore { get; set; }

        /// <summary>
        /// 不映射，比如说计算属性之类的，不建议使用
        /// 注意IsMap=false的属性，增删改查是都会被忽略
        /// </summary>
        public bool IsMap { get; set; } = true;
    }
}
