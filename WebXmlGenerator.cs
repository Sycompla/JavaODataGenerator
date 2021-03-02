using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class WebXmlGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string PackageName { get; set; }

        public Type Type { get; set; }

        private const string TemplateExtension = ".txt";

        private const string PackageNameMask = "#packageName#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/WebXml/" + fileName + TemplateExtension;

            return File.ReadAllText(textFile);

        } // ReadIntoString

        public void WriteOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".xml", text);

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
                        .Replace(PackageNameMask, PackageName)
                        ;

        } //GetHead

        public WebXmlGenerator Generate()
        {

            string result = null;

            result += GetHead();

            WriteOut(result, "web", OutputPath);

            return this;

        } // Generate

        public WebXmlGenerator Generate(Type type)
        {

            Type = type;

            return Generate();

        } // Generate

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    } // EFServiceGenerator

} // EFGeneralas