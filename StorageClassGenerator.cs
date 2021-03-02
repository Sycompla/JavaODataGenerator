using Ac4yClassModule.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class StorageClassGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string Package { get; set; }

        public Ac4yModule Module { get; set; }

        private const string TemplateExtension = ".txt";

        private const string PackageMask = "#package#";
        private const string CapImportsMask = "#capImports#";
        private const string ClassNameMask = "#className#";
        private const string CreateEntityMask = "#createEntity#";
        private const string GetEntitiesMask = "#getEntities#";
        private const string UpdateEntityMask = "#updateEntity#";
        private const string DeleteEntityMask = "#deleteEntity#";
        private const string FillListEntitiesMask = "#fillListEntities#";
        private const string FillListEntityMask = "#fillListEntity#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/Storage/" + fileName + TemplateExtension;

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

            foreach(Ac4yClass ac4yClass in Module.ClassList)
            {
                imports = imports + "import " + Package + ".connection." + ac4yClass.Name + "HibernateCap;\n";
            }

            return ReadIntoString("Head")
                        .Replace(CapImportsMask, imports)
                        .Replace(PackageMask, Package)
                        ;

        } //GetHead

        public string GetBody()
        {
            string createEntityText = ReadIntoString("CreateEntity");
            string getEntitiesText = ReadIntoString("GetEntities");
            string updateEntityText = ReadIntoString("UpdateEntity");
            string deleteEntityText = ReadIntoString("DeleteEntity");

            string createEntityTextEdited = "";
            string getEntitiesTextEdited = "";
            string updateEntityTextEdited = "";
            string deleteEntityTextEdited = "";
            string fillListEntities = "";
            string fillListEntity = "";

            foreach(Ac4yClass ac4yClass in Module.ClassList)
            {
                createEntityTextEdited += createEntityText.Replace(ClassNameMask, ac4yClass.Name);

                getEntitiesTextEdited += getEntitiesText.Replace(ClassNameMask, ac4yClass.Name);

                updateEntityTextEdited += updateEntityText.Replace(ClassNameMask, ac4yClass.Name);

                deleteEntityTextEdited += deleteEntityText.Replace(ClassNameMask, ac4yClass.Name);

                fillListEntities += "entitiesNames.add(\"" + ac4yClass.Name + "s\");\n";

                fillListEntity += "entityNames.add(\"" + ac4yClass.Name + "\");\n";
            }

            return ReadIntoString("Body")
                .Replace(FillListEntityMask, fillListEntity)
                .Replace(FillListEntitiesMask, fillListEntities)
                .Replace(CreateEntityMask, createEntityTextEdited)
                .Replace(GetEntitiesMask, getEntitiesTextEdited)
                .Replace(UpdateEntityMask, updateEntityTextEdited)
                .Replace(DeleteEntityMask, deleteEntityTextEdited)
                ;

        }

        public string GetFoot()
        {
            return ReadIntoString("Foot");
        }

        public StorageClassGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetBody();

            result += GetFoot();

            WriteOut(result, "Storage", OutputPath);

            return this;

        } // Generate

        public StorageClassGenerator Generate(Ac4yModule type)
        {

            Module = type;

            return Generate();

        } // Generate

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    } // EFServiceGenerator

} // EFGeneralas