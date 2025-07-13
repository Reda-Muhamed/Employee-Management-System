using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Base_Library.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string ?Name { get; set; }
        [JsonIgnore]
        public List<Employee>? Employees { get; set; }

    }
}
