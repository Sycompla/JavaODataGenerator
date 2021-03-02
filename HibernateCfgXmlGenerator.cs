using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class HibernateCfgXmlGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string JdbcString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Type Type { get; set; }

        private const string TemplateExtension = ".txt";

        private const string JdbcStringMask = "#jdbcString#";
        private const string UsernameMask = "#username#";
        private const string PasswordMask = "#password#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/HibernateConfiguration/" + fileName + TemplateExtension;

            return File.ReadAllText(textFile);

        } // ReadIntoString

        public void WriteOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".cfg.xml", text);

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

            return ReadIntoString("SQLServer")
                        .Replace(JdbcStringMask, JdbcString)
                        .Replace(UsernameMask, Username)
                        .Replace(PasswordMask, Password)
                        ;

        } //GetHead

        public HibernateCfgXmlGenerator Generate()
        {

            string result = null;

            result += GetHead();

            WriteOut(result, "hibernate", OutputPath);

            return this;

        } // Generate

        public HibernateCfgXmlGenerator Generate(Type type)
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