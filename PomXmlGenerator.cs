using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class PomXmlGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string ArtifactId { get; set; }
        public string GroupId { get; set; }
        public string Version { get; set; }

        public Type Type { get; set; }

        private const string TemplateExtension = ".txt";

        private const string ArtifactIdMask = "#artifactId#";
        private const string GroupIdMask = "#groupId#";
        private const string VersionMask = "#version#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/Pom/" + fileName + TemplateExtension;

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
                        .Replace(VersionMask, Version)
                        .Replace(GroupIdMask, GroupId)
                        .Replace(ArtifactIdMask, ArtifactId)
                        ;

        } //GetHead

        public PomXmlGenerator Generate()
        {

            string result = null;

            result += GetHead();

            WriteOut(result, "pom", OutputPath);

            return this;

        } // Generate

        public PomXmlGenerator Generate(Type type)
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