using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest_Console
{
    public class ImportHelper
    {
        public static List<dynamic> BuildDynamicObject()
        {
            var dict = new Dictionary<string, object>()
            {
                { "CorpId",91504},
                 { "EmployeeId",1},
                { "TemplateId","T12306"},
                { "Year",2017},
                { "Month",1}
            };

            var list = new List<dynamic>();
            var obj = new DynamicDictionaryFactory(dict);

            list.Add(obj);
            return list;
        }
    }

    public sealed class DynamicDictionaryFactory : DynamicObject
    {
        private readonly Dictionary<string, object> _properties;

        public DynamicDictionaryFactory(Dictionary<string, object> properties)
        {
            _properties = properties;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.ContainsKey(binder.Name))
            {

                result = _properties[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_properties.ContainsKey(binder.Name))
            {

                _properties[binder.Name] = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
