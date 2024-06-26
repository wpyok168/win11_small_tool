using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win11_设置
{
    public class CBListItem
    {
        private string name;
        private string value;

        public string Name { get => name; set => name = value; }
        public string Value { get => value; set => this.value = value; }
    }
}
