using Ac4yClassModule.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class FilterExpressionVisitorGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string Package { get; set; }

        public Ac4yClass Ac4yClass { get; set; }

        private const string TemplateExtension = ".txt";

        private const string PackageMask = "#package#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/FilterExpressionVisitor/" + fileName + TemplateExtension;

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

        } //GetHead

        public string GetBody()
        {

            return ReadIntoString("Body")
                ;

        }

        public string GetFoot()
        {
            return ReadIntoString("Foot");
        }

        public FilterExpressionVisitorGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetBody();

            result += GetFoot();

            WriteOut(result, "FilterExpressionVisitor", OutputPath);

            return this;

        } // Generate

        public FilterExpressionVisitorGenerator Generate(Ac4yClass type)
        {

            Ac4yClass = type;

            return Generate();

        } // Generate

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    } // EFServiceGenerator

} // EFGeneralas