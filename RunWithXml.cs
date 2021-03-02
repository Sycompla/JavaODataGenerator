using Ac4yClassModule.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaODataGenerator
{
    class RunWithXml
    {
        public string LibraryPath { get; set; }
        public string ParameterPath { get; set; }
        public string ParameterFileName { get; set; }
        public string RootDirectory { get; set; }
        public string Namespace { get; set; }
        public string PLanObjectNamespace { get; set; }
        public string ConnectionString { get; set; }
        public string PortNumber { get; set; }
        public string IPAddress { get; set; }
        public string ODataURL { get; set; }
        public string LinuxPath { get; set; }
        public string LinuxServiceFileDescription { get; set; }
        public string PlanObjectFolderName { get; set; }
        public string GroupId { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string JdbcString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PackageName { get; set; }
        public string DbName { get; set; }

        private string Argument { get; set; }
        private Ac4yModule Ac4yModule { get; set; }
        private string PackageRoute { get; set; }


        public RunWithXml(string args, Ac4yModule ac4yModule)
        {
            Argument = args;
            Ac4yModule = ac4yModule;
        }

        public RunWithXml() { }

        public void Run()
        {
            PackageRoute = "src/main/java/" + PackageName.Replace(".", "/");

            if (Argument.Equals("PlanObject"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/entity");
                foreach (Ac4yClass planObject in Ac4yModule.ClassList)
                {
                    new PlanObjectGenerator()
                    {
                        OutputPath = RootDirectory + PackageRoute + "/entity/"
                        ,
                        DbName = DbName
                        ,
                        Package = PackageName
                    }
                        .Generate(planObject);
                }
            }

            if(Argument.Equals("Pom"))
            {
                new PomXmlGenerator()
                {
                    OutputPath = RootDirectory
                    ,
                    ArtifactId = ArtifactId
                    ,
                    GroupId = GroupId
                    ,
                    Version = Version
                }.Generate();
            }

            if (Argument.Equals("Hibernate"))
            {
                Directory.CreateDirectory(RootDirectory + "/src/main/resources/");
                new HibernateCfgXmlGenerator()
                {
                    OutputPath = RootDirectory + "/src/main/resources/"
                    ,
                    JdbcString = JdbcString
                    ,
                    Username = Username
                    ,
                    Password = Password
                }.Generate();
            }

            if(Argument.Equals("WebXml"))
            {
                Directory.CreateDirectory(RootDirectory + "src/main/webapp/WEB-INF");
                new WebXmlGenerator()
                {
                    OutputPath = RootDirectory + "src/main/webapp/WEB-INF/"
                    ,
                    PackageName = PackageName
                }.Generate();
            }

            if(Argument.Equals("ODataServlet"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/web");
                new ODataServletGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/web/"
                    ,
                    Package = PackageName
                }.Generate();
            }

            if (Argument.Equals("cors"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/cors");
                new CORSFilterGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/cors/"
                    ,
                    PackageName = PackageName
                }.Generate();
            }

            if (Argument.Equals("Util"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/util");
                new UtilGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/util/"
                    ,
                    Package = PackageName
                }.Generate();
            }

            if (Argument.Equals("Storage"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/data");
                new StorageClassGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/data/"
                    ,
                    Package = PackageName
                }.Generate(Ac4yModule);
            }

            if(Argument.Equals("HibernateCap"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/connection");
                foreach (Ac4yClass ac4yClass in Ac4yModule.ClassList)
                {
                    new HibernateCapGenerator()
                    {
                        OutputPath = RootDirectory + PackageRoute + "/connection/"
                        ,
                        Package = PackageName
                    }.Generate(ac4yClass, Ac4yModule);
                }
            }

            if(Argument.Equals("FilterExpressionVisitor"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/service");
                new FilterExpressionVisitorGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/service/"
                    ,
                    Package = PackageName
                }.Generate();
            }

            if (Argument.Equals("EdmProvider"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/service");
                new EdmProviderGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/service/"
                    ,
                    Package = PackageName
                }.Generate(Ac4yModule);
            }

            if (Argument.Equals("EntityProcessor"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/service");
                new EntityProcessorGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/service/"
                    ,
                    Package = PackageName
                }.Generate();
            }

            if (Argument.Equals("EntityCollectionProcessor"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/service");
                new EntityCollectionProcessorGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/service/"
                    ,
                    Package = PackageName
                }.Generate();
            }

            if (Argument.Equals("PrimitiveProcessor"))
            {
                Directory.CreateDirectory(RootDirectory + PackageRoute + "/service");
                new PrimitiveProcessorGenerator()
                {
                    OutputPath = RootDirectory + PackageRoute + "/service/"
                    ,
                    Package = PackageName
                }.Generate();
            }

            if(Argument.Equals("OpenApi"))
            {
                new OpenApiDocumentGenerator()
                {
                    OutputPath = RootDirectory
                    ,
                    ODataUrl = ODataURL
                    ,
                    Parameter = Ac4yModule
                    ,
                    Version = Version
                }.Generate();
            }

        } // run
    }
}
