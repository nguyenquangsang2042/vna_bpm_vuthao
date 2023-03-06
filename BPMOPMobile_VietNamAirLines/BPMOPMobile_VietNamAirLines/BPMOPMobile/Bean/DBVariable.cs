using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class DBVariable : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string Id { get; set; }
        public string Value { get; set; }

        public DBVariable() { }
        public DBVariable(string id, string value)
        {
            this.Id = id;
            this.Value = value;
        }
    }
}
