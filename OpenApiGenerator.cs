using Ac4yClassModule.Class;
using Ac4yClassModule.Service;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class OpenApiGenerator
    {
        public string OutputPath { get; set; }
        public CSODataGeneratorParameter Parameter { get; set; }
        public string Version { get; set; }
        public string ODataUrl { get; set; }

        private Ac4yClassHandler Ac4yClassHandler = new Ac4yClassHandler();

        public Dictionary<string, string> FormaKonverziok = new Dictionary<string, string>()
        {
            { "Int32", "int32" },
            { "Decimal", "" },
            { "Money", "integer" },
            { "Float", "float" },
            { "Int64", "int64" },
            { "VarChar", "" },
            { "Char", "" },
            { "VarBinary", "byte[]" },
            { "DateTime", "date-time" },
            { "Date", "date" },
            { "TinyInt", "" },
            { "Bit", "" },
            { "String", "" },
            { "Double", "double" },
            { "Boolean", "" }
        };

        public Dictionary<string, string> TipusKonverziok = new Dictionary<string, string>()
        {
            { "Int32", "integer" },
            { "Money", "integer" },
            { "Float", "number" },
            { "Int64", "integer" },
            { "VarChar", "string" },
            { "Char", "string" },
            { "VarBinary", "byte[]" },
            { "DateTime", "string" },
            { "Date", "string" },
            { "TinyInt", "Byte?" },
            { "Bit", "bool" },
            { "String", "string" },
            { "Double", "number" },
            { "Boolean", "boolean" }
        };

        public void WriteOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".json", text);

        }

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

        public OpenApiObject GenerateExample(Ac4yClass ac4yClass)
        {
            OpenApiObject resultExample = new OpenApiObject();

            foreach (Ac4yProperty property in ac4yClass.PropertyList)
            {
                if (!property.Name.Equals("Id"))
                {
                    if (GetConvertedType(property.TypeName, FormaKonverziok).Equals("int32") || GetConvertedType(property.TypeName, FormaKonverziok).Equals("int64"))
                        resultExample.Add(property.Name, new OpenApiInteger(
                            Int32.Parse(Ac4yClassHandler.GetAc4yExampleValue(property.PropertyInfo))
                            ));

                    if (GetConvertedType(property.TypeName, FormaKonverziok).Equals("float"))
                        resultExample.Add(property.Name, new OpenApiFloat(
                            float.Parse(Ac4yClassHandler.GetAc4yExampleValue(property.PropertyInfo))
                            ));

                    if (GetConvertedType(property.TypeName, FormaKonverziok).Equals("double"))
                        resultExample.Add(property.Name, new OpenApiDouble(
                            Double.Parse(Ac4yClassHandler.GetAc4yExampleValue(property.PropertyInfo))
                            ));

                    if (GetConvertedType(property.TypeName, FormaKonverziok).Equals("date"))
                        resultExample.Add(property.Name, new OpenApiDate(DateTime.Now.Date));

                    if (GetConvertedType(property.TypeName, FormaKonverziok).Equals("date-time"))
                        resultExample.Add(property.Name, new OpenApiDateTime(DateTime.Now));

                    if (GetConvertedType(property.TypeName, TipusKonverziok).Equals("string") && GetConvertedType(property.TypeName, FormaKonverziok).Equals(""))
                        resultExample.Add(property.Name, new OpenApiString(Ac4yClassHandler.GetAc4yExampleValue(property.PropertyInfo)));

                    if (GetConvertedType(property.TypeName, TipusKonverziok).Equals("boolean") && GetConvertedType(property.TypeName, FormaKonverziok).Equals(""))
                        resultExample.Add(property.Name, new OpenApiBoolean(
                            Boolean.Parse(Ac4yClassHandler.GetAc4yExampleValue(property.PropertyInfo))
                            ));
                }
            };
            return resultExample;
        }

        public string GetConnectedPropertyNames(PlanObjectReference planObject)
        {
            string result = "";
            Ac4yClass ac4yClass = new Ac4yClassHandler().GetAc4yClassFromType(planObject.classType);

            foreach (Ac4yProperty property in ac4yClass.PropertyList)
            {
                if (property.Cardinality.Equals("COLLECTION"))
                {
                    result = result + property.Name + ", ";
                }
            }

            return result;

        }

        public Dictionary<string, OpenApiSchema> SchemaFromObject()
        {
            Dictionary<string, OpenApiSchema> Lista = new Dictionary<string, OpenApiSchema>();

            foreach (PlanObjectReference planObject in Parameter.PlanObjectReferenceList)
            {
                Ac4yClass ac4yClass = new Ac4yClassHandler().GetAc4yClassFromType(planObject.classType);

                Dictionary<string, OpenApiSchema> PropertyLista = new Dictionary<string, OpenApiSchema>();
                Dictionary<string, OpenApiSchema> PropertyListaWithoutID = new Dictionary<string, OpenApiSchema>();

                foreach (Ac4yProperty property in ac4yClass.PropertyList)
                {
                    if (property.Cardinality.Equals("COLLECTION"))
                    {
                        PropertyLista.Add(property.Name, new OpenApiSchema()
                        {
                            Type = "array",
                            Items = new OpenApiSchema()
                            {
                                Type = "object",
                                Title = property.TypeName
                            },
                            Nullable = true,
                            Description = Ac4yClassHandler.GetAc4yDescription(property.PropertyInfo)

                        });

                    }
                    else if (property.NavigationProperty == true)
                    {
                        PropertyLista.Add(property.Name, new OpenApiSchema()
                        {
                            Type = "object",
                            Description = Ac4yClassHandler.GetAc4yDescription(property.PropertyInfo),
                            Title = property.Name,
                            Nullable = true
                        });

                    }
                    else
                    {

                        PropertyLista.Add(property.Name, new OpenApiSchema()
                        {
                            Type = GetConvertedType(property.TypeName, TipusKonverziok),
                            Format = GetConvertedType(property.TypeName, FormaKonverziok),
                            Description = Ac4yClassHandler.GetAc4yDescription(property.PropertyInfo)

                        });


                        if (!property.Name.Equals("Id"))
                        {
                            PropertyListaWithoutID.Add(property.Name, new OpenApiSchema()
                            {
                                Type = GetConvertedType(property.TypeName, TipusKonverziok),
                                Format = GetConvertedType(property.TypeName, FormaKonverziok),
                                Description = Ac4yClassHandler.GetAc4yDescription(property.PropertyInfo)

                            });
                        }
                    }

                };

                OpenApiSchema Schema = new OpenApiSchema() { Properties = PropertyLista };
                OpenApiSchema SchemaWithoutID = new OpenApiSchema() { Properties = PropertyListaWithoutID };
                Lista.Add(ac4yClass.Name, Schema);
                Lista.Add(ac4yClass.Name + "WithoutID", SchemaWithoutID);
            }

            return Lista;
        }

        public OpenApiPaths GeneratePaths()
        {
            OpenApiPaths paths = new OpenApiPaths();

            foreach (PlanObjectReference planObject in Parameter.PlanObjectReferenceList)
            {
                paths
                    .Add(
                        "/" + planObject.className + "/{id}", new OpenApiPathItem
                        {
                            Operations = new Dictionary<OperationType, OpenApiOperation>
                            {
                                [OperationType.Get] = new OpenApiOperation
                                {
                                    Description = "Egy, az adott id-hez tartozó, " + planObject.className + " típusú rekord lekérdezése.",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new OpenApiParameter
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "A rekord is-ével lekérdezi a konkrét rekordot",
                                            Required = true,
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer"
                                            }
                                        }
                                    },
                                    Responses = new OpenApiResponses
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "A lekérdezés sikeres volt, a rekord adatait adja vissza",
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Title = "Vendor",
                                                        Reference = new OpenApiReference
                                                        {
                                                            Type = ReferenceType.Schema,
                                                            Id = planObject.className
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        ["400"] = new OpenApiResponse
                                        {
                                            Description = "A kérés hibára futott, a hibaüzenet bővebben tájékoztat",
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Title = "Hiba üzenet",
                                                        Properties = new Dictionary<string, OpenApiSchema>
                                                        {
                                                            ["Message"] = new OpenApiSchema
                                                            {
                                                                Type = "string",
                                                                Description = "A hibáról informáló üzenet"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                },
                                [OperationType.Patch] = new OpenApiOperation
                                {
                                    Description = "Update-el 1 adott rekordot az id alapján",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new OpenApiParameter
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "A rekord id-hez tartozó rekordot fogja frissíteni",
                                            Required = true,
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer"
                                            }
                                        }
                                    },
                                    RequestBody = new OpenApiRequestBody
                                    {
                                        Required = true,
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Example = GenerateExample(new Ac4yClassHandler().GetAc4yClassFromType(planObject.classType)),
                                                Schema = new OpenApiSchema
                                                {
                                                    Title = "Vendor",
                                                    Reference = new OpenApiReference
                                                    {
                                                        Type = ReferenceType.Schema,
                                                        Id = planObject.className + "WithoutID"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    Responses = new OpenApiResponses
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "Az adatok frissítése sikeres volt, a rekord a db-ben is frissült",
                                        },
                                        ["400"] = new OpenApiResponse
                                        {
                                            Description = "A kérés hibára futott, a hibaüzenet bővebben tájékoztat",
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Title = "Hiba üzenet",
                                                        Properties = new Dictionary<string, OpenApiSchema>
                                                        {
                                                            ["Message"] = new OpenApiSchema
                                                            {
                                                                Type = "string",
                                                                Description = "A hibáról informáló üzenet"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                },
                                [OperationType.Delete] = new OpenApiOperation
                                {
                                    Description = "Töröl 1, adott rekordot az id alapján és a rekordhoz tartozó összes kapcsolt rekordot " +
                                                    "\nKapcsolt rekordok: " + GetConnectedPropertyNames(planObject),
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new OpenApiParameter
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "A rekord id-ével törli a konkrét rekordot és a hozzá kapcsolódókat is. " +
                                                            "\nKapcsolt rekordok: " + GetConnectedPropertyNames(planObject),
                                            Required = true,
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer"
                                            }
                                        }
                                    },
                                    Responses = new OpenApiResponses
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "A törlés sikeres volt, a rekord a db-ből is törlődött",
                                        },
                                        ["400"] = new OpenApiResponse
                                        {
                                            Description = "A kérés hibára futott, a hibaüzenet bővebben tájékoztat",
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Title = "Hiba üzenet",
                                                        Properties = new Dictionary<string, OpenApiSchema>
                                                        {
                                                            ["Message"] = new OpenApiSchema
                                                            {
                                                                Type = "string",
                                                                Description = "A hibáról informáló üzenet"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    );

                paths.Add(
                    "/" + planObject.className, new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Description = planObject.className + "típusú rekordok lekérdezése",
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "A lekérdezés sikeres volt, a rekordok listáját adja vissza",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = "array",
                                                    Items = new OpenApiSchema
                                                    {
                                                        Reference = new OpenApiReference
                                                        {
                                                            Type = ReferenceType.Schema,
                                                            Id = planObject.className
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    ["400"] = new OpenApiResponse
                                    {
                                        Description = "A kérés hibára futott, a hibaüzenet bővebben tájékoztat",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Title = "Hiba üzenet",
                                                    Properties = new Dictionary<string, OpenApiSchema>
                                                    {
                                                        ["Message"] = new OpenApiSchema
                                                        {
                                                            Type = "string",
                                                            Description = "A hibáról informáló üzenet"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            },
                            [OperationType.Post] = new OpenApiOperation
                            {
                                Description = "Új rekord létrehozása",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Required = true,
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Example = GenerateExample(new Ac4yClassHandler().GetAc4yClassFromType(planObject.classType)),
                                            Schema = new OpenApiSchema
                                            {
                                                Title = "Vendor",
                                                Reference = new OpenApiReference
                                                {
                                                    Type = ReferenceType.Schema,
                                                    Id = planObject.className + "WithoutID"
                                                }
                                            }
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "A felvitel sikeres volt, a rekord a db-be is bekerült",
                                    },
                                    ["400"] = new OpenApiResponse
                                    {
                                        Description = "A kérés hibára futott, a hibaüzenet bővebben tájékoztat",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Title = "Hiba üzenet",
                                                    Properties = new Dictionary<string, OpenApiSchema>
                                                    {
                                                        ["Message"] = new OpenApiSchema
                                                        {
                                                            Type = "string",
                                                            Description = "A hibáról informáló üzenet"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                );
            }

            return paths;
        }

        public OpenApiDocument GenerateDocument()
        {

            OpenApiDocument document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = Version,
                    Title = "Próba odata service doksi",
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = ODataUrl }
                },
                Components = new OpenApiComponents
                {
                    Schemas = SchemaFromObject()
                },
                Paths = GeneratePaths()
            };

            return document;
        }

        public OpenApiGenerator Generate()
        {
            OpenApiDocument document = GenerateDocument();
            string result = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);

            WriteOut(result, "OpenApiDocument", OutputPath);

            return this;

        } // Generate

    }
}