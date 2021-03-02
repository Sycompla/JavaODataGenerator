using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class LinuxServiceFileGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string DLLName { get; set; }
        public string LinuxPath { get; set; }
        public string Description { get; set; }

        public Type Type { get; set; }

        private const string TemplateExtension = ".serviceT";

        private const string DescriptionMask = "#description#";

        private const string DLLNameMask = "#dllName#";
        private const string WorkingDirectoryMask = "#workingDirectory#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/LinuxServiceFile/" + fileName + TemplateExtension;

            return File.ReadAllText(textFile);

        } // ReadIntoString

        public void WriteOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".service", text);

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
                        .Replace(DescriptionMask, Description)
                        ;

        } //GetHead

        public string GetBody()
        {
            if (!LinuxPath.StartsWith("/"))
                LinuxPath = "/" + LinuxPath;

            if (!LinuxPath.EndsWith("/"))
                LinuxPath = LinuxPath + "/";

            return
                ReadIntoString("Body")
                        .Replace(WorkingDirectoryMask, LinuxPath + DLLName + "ODataServicePublish")
                        .Replace(DLLNameMask, DLLName + "ODataService");
        } //GetMethods

        public string GetFoot()
        {
            return
                ReadIntoString("Foot");

        } //GetFoot


        public LinuxServiceFileGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetBody();

            result += GetFoot();

            WriteOut(result, DLLName + "ODataService", OutputPath);

            return this;

        } // Generate

        public LinuxServiceFileGenerator Generate(Type type)
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