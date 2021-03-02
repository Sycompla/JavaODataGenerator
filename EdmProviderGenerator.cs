using Ac4yClassModule.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class EdmProviderGenerator
    {


        #region members

        public string OutputPath { get; set; }
        public string Package { get; set; }

        public Ac4yModule Module { get; set; }

        private const string TemplateExtension = ".txt";

        private const string PackageMask = "#package#";
        private const string FinalNamesMask = "#finalNames#";
        private const string ClassNameMask = "#className#";
        private const string CLASSNAMEMask = "#CLASSNAME#";
        private const string EntityTypesMask = "#entityTypes#";
        private const string AddPropertiesMask = "#addProperties#";
        private const string AddPropertyMask = "#addProperty#";
        private const string PropertyNameMask = "#propertyName#";
        private const string PropertyNameSmallMask = "#propertyNameSmall#";
        private const string TypeMask = "#type#";
        private const string AddEntityPropertiesMask = "#addEntityProperties#";
        private const string AddEntitySetsToContainerMask = "#addEntitySetsToContainer#";
        private const string AddEntitySetsMask = "#addEntitySets#";
        private const string PropertyNameListMask = "#propertyNameList#";
        #endregion members

        public string ReadIntoString(string fileName)
        {

            string textFile = "Templates/EdmProvider/" + fileName + TemplateExtension;

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
            string finalNamesText = ReadIntoString("FinalNames");
            string addEntityPropertiesText = ReadIntoString("AddEntityProperties");
            string addPropertyText = ReadIntoString("AddProperty");
            string addEntitySetsToContainerText = ReadIntoString("AddEntitySetsToContainer");
            string addEntitySetsText = ReadIntoString("AddEntitySets");

            string finalNamesTextEdited = "";
            string entityTypes = "";
            string addEntityPropertiesTextEdited = "";
            string addEntitySetsToContainerTextEdited = "";
            string addEntitySetsTextEdited = "";

            foreach (Ac4yClass ac4yClass in Module.ClassList)
            {

                string addPropertyTextEdited = "";
                string propertiyNameList = "";

                finalNamesTextEdited += finalNamesText
                    .Replace(ClassNameMask, ac4yClass.Name)
                    .Replace(CLASSNAMEMask, ac4yClass.Name.ToUpper());

                entityTypes += "entityTypes.add(getEntityType(ET_" + ac4yClass.Name.ToUpper() + "_FQN));\n";

                foreach(Ac4yProperty property in ac4yClass.PropertyList)
                {
                    propertiyNameList += GetNameWithLowerFirstLetter(property.Name) + ", ";
                    if (!property.Name.Equals("Id"))
                    {
                        if (property.Type.Equals("DateTime"))
                        {
                            addPropertyTextEdited += addPropertyText
                                .Replace(PropertyNameMask, property.Name)
                                .Replace(PropertyNameSmallMask, GetNameWithLowerFirstLetter(property.Name))
                                .Replace(TypeMask, "Date");

                        }
                        else
                        {
                            addPropertyTextEdited += addPropertyText
                                .Replace(PropertyNameMask, property.Name)
                                .Replace(PropertyNameSmallMask, GetNameWithLowerFirstLetter(property.Name))
                                .Replace(TypeMask, property.Type);
                        }
                    }
                }
                propertiyNameList = propertiyNameList.Substring(0, propertiyNameList.Length - 2);

                addEntityPropertiesTextEdited += addEntityPropertiesText
                    .Replace(CLASSNAMEMask, ac4yClass.Name.ToUpper())
                    .Replace(AddPropertyMask, addPropertyTextEdited)
                    .Replace(PropertyNameListMask, propertiyNameList);

                addEntitySetsToContainerTextEdited += addEntitySetsToContainerText
                    .Replace(CLASSNAMEMask, ac4yClass.Name.ToUpper());

                addEntitySetsTextEdited += addEntitySetsText
                    .Replace(CLASSNAMEMask, ac4yClass.Name.ToUpper());
            }

            return ReadIntoString("Body")
                .Replace(FinalNamesMask, finalNamesTextEdited)
                .Replace(EntityTypesMask, entityTypes)
                .Replace(AddEntityPropertiesMask, addEntityPropertiesTextEdited)
                .Replace(AddEntitySetsToContainerMask, addEntitySetsToContainerTextEdited)
                .Replace(AddEntitySetsMask, addEntitySetsTextEdited)
                ;

        }

        public string GetFoot()
        {
            return ReadIntoString("Foot");
        }

        public EdmProviderGenerator Generate()
        {

            string result = null;

            result += GetHead();

            result += GetBody();

            result += GetFoot();

            WriteOut(result, "ODataEdmProvider", OutputPath);

            return this;

        } // Generate

        public EdmProviderGenerator Generate(Ac4yModule module)
        {

            Module = module;

            return Generate();

        } // Generate

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    } // EFServiceGenerator

} // EFGeneralas