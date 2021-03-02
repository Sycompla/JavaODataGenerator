using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class EntityProcessorGenerator
    {

        #region members

        public string OutputPath { get; set; }
        public string Package { get; set; }

        public Type Type { get; set; }

        private const string TemplateExtension = ".txt";

        private const string PackageMask = "#package#";

        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/EntityProcessor/" + fileName + TemplateExtension;

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
                        ;

        }

        public string GetFoot()
        {
            return
                ReadIntoString("Foot")
                        ;

        }

        public string GetMethods()
        {
            return
                ReadIntoString("Body")
                ;
        }

        public EntityProcessorGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetMethods();

            result += GetFoot();

            WriteOut(result, "ODataEntityProcessor", OutputPath);

            return this;

        } // Generate

        public EntityProcessorGenerator Generate(Type type)
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