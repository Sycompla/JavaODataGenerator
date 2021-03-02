using Ac4yClassModule.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class HibernateCapGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string Package { get; set; }

        public Ac4yClass Ac4yClass { get; set; }
        public Ac4yModule Module { get; set; }

        private const string TemplateExtension = ".txt";

        private const string PackageMask = "#package#";
        private const string EntityImportsMask = "#entityImports#";
        private const string ClassNameMask = "#className#";
        private const string AnnotatedClassesMask = "#annotatedClasses#";
        private const string ClassNameSmallMask = "#classNameSmall#";
        private const string AddPropertiesMask = "#addProperties#";
        private const string PropertyNameMask = "#propertyName#";
        private const string PropertiesInsertMask = "#propertiesInsert#";
        private const string SetPropertiesMask = "#setProperties#";

        private const string DateFormat = "YYYY-MM-dd";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/HibernateCap/" + fileName + TemplateExtension;

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

            string imports = "";

            foreach (Ac4yClass ac4yClass in Module.ClassList)
            {
                imports += "import " + Package + ".entity." + ac4yClass.Name + ";\n";
            }

            return ReadIntoString("Head")
                        .Replace(EntityImportsMask, imports)
                        .Replace(PackageMask, Package)
                        .Replace(ClassNameMask, Ac4yClass.Name)
                        ;

        } //GetHead

        public string GetBody()
        {
            string addPropertyText = ReadIntoString("AddProperty");
            string setPropertiesText = ReadIntoString("SetProperties");

            string annotatedClasses = "";
            string addPropertyTextEdited = "";
            string propertiesInsertText = "";
            string setPropertiesTextEdited = "";

            foreach (Ac4yProperty property in Ac4yClass.PropertyList)
            {
                addPropertyTextEdited += addPropertyText
                    .Replace(ClassNameSmallMask, GetNameWithLowerFirstLetter(Ac4yClass.Name))
                    .Replace(PropertyNameMask, property.Name)
                    .Replace(ClassNameMask, Ac4yClass.Name);

                if (!property.Name.Equals("Id"))
                {

                    if (property.Type.Equals("DateTime"))
                    {
                        propertiesInsertText += "getDateTime(properties.get(\"" + property.Name + "\")), ";

                        setPropertiesTextEdited += "temp" + Ac4yClass.Name + ".set" + property.Name + "("
                            + "getDateTime(properties.get(\"" + property.Name + "\")));\n";
                    }
                    else
                    {
                        propertiesInsertText += "properties.get(\"" + property.Name + "\"), ";

                        setPropertiesTextEdited += setPropertiesText
                            .Replace(PropertyNameMask, property.Name)
                            .Replace(ClassNameMask, Ac4yClass.Name);
                    }
                }
            }
            propertiesInsertText = propertiesInsertText.Substring(0, propertiesInsertText.Length - 2);

            foreach(Ac4yClass ac4yClass in Module.ClassList)
            {
                annotatedClasses += ".addAnnotatedClass(" + ac4yClass.Name + ".class)\n";
            }

            return ReadIntoString("Body")
                .Replace(AnnotatedClassesMask, annotatedClasses)
                .Replace(ClassNameMask, Ac4yClass.Name)
                .Replace(ClassNameSmallMask, GetNameWithLowerFirstLetter(Ac4yClass.Name))
                .Replace(AddPropertiesMask, addPropertyTextEdited)
                .Replace(PropertiesInsertMask, propertiesInsertText)
                .Replace(SetPropertiesMask, setPropertiesTextEdited)
                ;

        }

        public string GetFoot()
        {
            return ReadIntoString("Foot");
        }

        public HibernateCapGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetBody();

            result += GetFoot();

            WriteOut(result, Ac4yClass.Name + "HibernateCap", OutputPath);

            return this;

        } // Generate

        public HibernateCapGenerator Generate(Ac4yClass type, Ac4yModule module)
        {

            Ac4yClass = type;

            Module = module;

            return Generate();

        } // Generate

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    } // EFServiceGenerator

} // EFGeneralas