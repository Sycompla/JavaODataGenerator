
using Ac4yClassModule.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class PlanObjectGenerator
    {

        #region members

        public string OutputPath { get; set; }

        public string Package { get; set; }

        public string DbName { get; set; }

        public Ac4yClass Type { get; set; }

        private const string TemplateExtension = ".txt";

        private const string ClassNameMask = "#className#";
        private const string TypeMask = "#type#";
        private const string PackageMask = "#package#";
        private const string DbNameMask = "#dbName#"; 
        private const string PropertyNameMask = "#propertyName#";
        private const string PropertyNameSmallMask = "#propertyNameSmall#";
        private const string PropertiesMask = "#properties#";
        private const string PropertiesSmallMask = "#propertiesSmall#";

        public Dictionary<string, string> CSharpTypeToJava = new Dictionary<string, string>()
        {
            { "Int32", "int" },
            { "Decimal", "int" },
            { "Money", "int" },
            { "Float", "float" },
            { "Int64", "int" },
            { "VarChar", "" },
            { "Char", "" },
            { "VarBinary", "byte[]" },
            { "DateTime", "Date" },
            { "Date", "Date" },
            { "TinyInt", "" },
            { "Bit", "" },
            { "String", "String" },
            { "Double", "Double" },
            { "Boolean", "Boolean" }
        };

        #endregion members

        public string GetConvertedType(string type, Dictionary<string, string> dictionary)
        {
            string result = null;
            try
            {
                result = dictionary[type];
            }
            catch (Exception exception)
            {
                result = "";
            }
            return result;
        } // GetConvertedType

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/PlanObject/" + fileName + TemplateExtension;

            return File.ReadAllText(textFile);

        } // ReadIntoString

        public void WriteOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".java", text);

        }

        public string GetNameWithLowerFirstLetter(String Code)
        {
            return
                char.ToLower(Code[0])
                + Code.Substring(1)
                ;

        } // GetNameWithLowerFirstLetter

        public string GetHead()
        {

            return ReadIntoString("Head")
                        .Replace(PackageMask, Package)
                        .Replace(ClassNameMask, Type.Name)
                        .Replace(DbNameMask, DbName)
                        ;

        }

        public string GetFoot()
        {
            return
                ReadIntoString("Foot")
                        ;

        }

        public string GetProperties()
        {
            string propertiesText = ReadIntoString("Properties");
            string propertiesTextEdited = "";
            foreach(Ac4yProperty property in Type.PropertyList)
            {
                if (!property.Name.Equals("Id"))
                {
                    if (isCollection(property))
                    {
                        propertiesTextEdited = propertiesTextEdited + propertiesText.Replace(TypeMask, "List<" + GetConvertedType(property.Type, CSharpTypeToJava) + ">")
                                                                                    .Replace(PropertyNameMask, property.Name);

                    }
                    else
                    {
                        propertiesTextEdited = propertiesTextEdited + propertiesText.Replace(TypeMask, GetConvertedType(property.Type, CSharpTypeToJava))
                                                                                    .Replace(PropertyNameMask, property.Name);

                    }
                }
            }

            return propertiesTextEdited;
        }

        public string GetGetterAndSetter()
        {
            string propertiesText = ReadIntoString("GetterAndSetter");
            string propertiesTextEdited = "";
            foreach (Ac4yProperty property in Type.PropertyList)
            {
                if (isCollection(property))
                {
                    propertiesTextEdited = propertiesTextEdited + propertiesText.Replace(TypeMask, "List<" + GetConvertedType(property.Type, CSharpTypeToJava) + ">")
                                                                                .Replace(PropertyNameMask, property.Name)
                                                                                .Replace(PropertyNameSmallMask, GetNameWithLowerFirstLetter(property.Name));

                }
                else
                {
                    propertiesTextEdited = propertiesTextEdited + propertiesText.Replace(TypeMask, GetConvertedType(property.Type, CSharpTypeToJava))
                                                                                .Replace(PropertyNameMask, property.Name)
                                                                                .Replace(PropertyNameSmallMask, GetNameWithLowerFirstLetter(property.Name));

                }
            }

            return propertiesTextEdited;
        }

        public string GetConstructors()
        {
            string propertiesSmall = "";
            string properties = "";

            foreach (Ac4yProperty property in Type.PropertyList)
            {
                if (!property.Name.Equals("Id"))
                {
                    propertiesSmall = propertiesSmall + GetConvertedType(property.Type, CSharpTypeToJava) + " " + GetNameWithLowerFirstLetter(property.Name) + ", ";
                    properties = properties + property.Name + " = " + GetNameWithLowerFirstLetter(property.Name) + ";\n";
                }
            }

            return ReadIntoString("Constructors")
                        .Replace(PropertiesSmallMask, propertiesSmall.Substring(0, propertiesSmall.Length - 2))
                        .Replace(PropertiesMask, properties)
                        .Replace(ClassNameMask, Type.Name)
                        ;

        }

        public bool isCollection(Ac4yProperty property)
        {
            return
                property.Cardinality.Equals("COLLECTION");
        }

        public PlanObjectGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetProperties();

            result += GetConstructors();

            result += GetGetterAndSetter();

            result += GetFoot();

            WriteOut(result, Type.Name, OutputPath);

            return this;

        } // Generate

        public PlanObjectGenerator Generate(Ac4yClass type)
        {

            Type = type;

            return Generate();

        } // Generate


        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    }

} // EFGenerala