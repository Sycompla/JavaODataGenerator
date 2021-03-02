set generatorDir=D:\Server\VisualStudio\ODataGenerator\JavaODataGenerator\bin\Debug\netcoreapp3.0\

cd %generatorDir%
JavaODataGenerator.exe "PlanObject"
JavaODataGenerator.exe "Pom"
JavaODataGenerator.exe "Hibernate"
JavaODataGenerator.exe "WebXml"
JavaODataGenerator.exe "ODataServlet"
JavaODataGenerator.exe "Util"
JavaODataGenerator.exe "Storage"
JavaODataGenerator.exe "HibernateCap"
JavaODataGenerator.exe "FilterExpressionVisitor"
JavaODataGenerator.exe "EdmProvider"
JavaODataGenerator.exe "EntityProcessor"
JavaODataGenerator.exe "EntityCollectionProcessor"
JavaODataGenerator.exe "PrimitiveProcessor"
JavaODataGenerator.exe "cors"
JavaODataGenerator.exe "OpenApi"
call npm install -g redoc-cli
call redoc-cli bundle -o index.html OpenApiDocument.json