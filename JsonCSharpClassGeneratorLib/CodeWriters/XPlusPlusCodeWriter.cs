using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class XPlusPlusCodeWriter : ICodeWriter
    {
        private const string indentSize = "    ";
        public string DisplayName
        {
            get
            {
                return "X++";
            }
        }

        public string FileExtension
        {
            get
            {
                return ".xml";
            }
        }

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "object";
                case JsonTypeEnum.Array: return "List";
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "boolean";
                case JsonTypeEnum.Float: return "real";
                case JsonTypeEnum.Integer: return "int";
                case JsonTypeEnum.Long: return "RecId";
                case JsonTypeEnum.Date: return "utcDateTime";
                case JsonTypeEnum.NonConstrained: return "object";
                case JsonTypeEnum.NullableBoolean: return "bool?";
                case JsonTypeEnum.NullableFloat: return "double?";
                case JsonTypeEnum.NullableInteger: return "int?";
                case JsonTypeEnum.NullableLong: return "long?";
                case JsonTypeEnum.NullableDate: return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.String: return "str";
                default: throw new System.NotSupportedException("Unsupported json type");
            }
        }

        public void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type)
        {
            var prefix = config.UseNestedClasses && !type.IsRoot ? "            " : "        ";
            sw.WriteLine("    [DataContractAttribute]");
                        
            sw.WriteLine("    public class {0}", type.AssignedName);
            sw.WriteLine("    {");

            WriteClassMembers(config, sw, type, prefix);

            sw.WriteLine("    }");
            sw.WriteLine();
        }


        private void WriteClassMembers(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
            foreach (var field in type.Fields)
            {
                sw.WriteLine(prefix + "{0} {1};", field.Type.GetTypeName(), char.ToLowerInvariant(field.MemberName[0]) + field.MemberName.Substring(1));
            }
            
            foreach (var field in type.Fields)
            {
                var camelCaseMemberName = char.ToLowerInvariant(field.MemberName[0]) + field.MemberName.Substring(1);
                var titleCaseMemberName = CultureInfo.InvariantCulture.TextInfo.ToUpper(field.MemberName[0]) + field.MemberName.Substring(1);
                
                sw.WriteLine();

                if (config.ExamplesInDocumentation)
                {
                    sw.WriteLine(prefix + "/// <summary>");
                    sw.WriteLine(prefix + "/// Examples: " + field.GetExamplesText());
                    sw.WriteLine(prefix + "/// </summary>");
                }

                sw.Write(prefix + "[" + "DataMember" + "(\'{0}\')", field.JsonMemberName);
                if (field.Type.Type == JsonTypeEnum.Array)
                {
                    sw.Write(", DataCollectionAttribute(Types::Class, classStr({0}))", titleCaseMemberName);
                }
                sw.WriteLine("]");
                
                sw.WriteLine(prefix + "public {0} parm{1}({0} _{2} = {2}) ", 
                    field.Type.GetTypeName(),
                    titleCaseMemberName,
                    camelCaseMemberName);
                sw.WriteLine(prefix + "{");
                sw.WriteLine(prefix + indentSize + "{0} = _{0};", camelCaseMemberName);
                sw.WriteLine(prefix + indentSize + "return {0};", camelCaseMemberName);
                sw.WriteLine(prefix + "}");

            }

        }


        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            throw new NotImplementedException();
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            throw new NotImplementedException();
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            throw new NotImplementedException();
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            throw new NotImplementedException();
        }
    }
}
