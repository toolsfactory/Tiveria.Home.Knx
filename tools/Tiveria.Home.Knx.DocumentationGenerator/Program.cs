using XmlDocMarkdown.Core;

var settings = new XmlDocMarkdownSettings()
{
    VisibilityLevel = XmlDocVisibilityLevel.Internal,
    TocPrefix = "toc_",
    SourceCodePath = "https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx",
    SkipUnbrowsable = true,
    ShouldClean = true,
    RootNamespace = "Tiveria.Home.Knx",
    NamespacePages = true,
    IncludeObsolete = true,
    GenerateToc = true,
}; 

XmlDocMarkdownGenerator.Generate("Tiveria.Home.Knx.dll", "..\\..\\..\\..\\..\\docs", settings);

settings = new XmlDocMarkdownSettings()
{
    VisibilityLevel = XmlDocVisibilityLevel.Internal,
    TocPrefix = "toc_",
    SourceCodePath = "https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP",
    SkipUnbrowsable = true,
    ShouldClean = true,
    RootNamespace = "Tiveria.Home.Knx.IP",
    NamespacePages = true,
    IncludeObsolete = true,
    GenerateToc = true,
};

XmlDocMarkdownGenerator.Generate("Tiveria.Home.Knx.IP.dll", "..\\..\\..\\..\\..\\docs", settings);



// https://github.com/toolsfactory/Tiveria.Home.Knx.wiki.git