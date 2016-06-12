using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace NugetPackageInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            var csprojPath = @"C:\Users\aziz\Documents\Visual Studio 2015\Projects\Dummy\Dummy\Dummy.csproj";
            AddReferenceToCsproj(csprojPath);
            AddReferenceToPackagesConfig(csprojPath);
        }

        private static void AddReferenceToPackagesConfig(string csprojPath)
        {
            var xmlDoc = new XmlDocument();
            var parentDirectory = new FileInfo(csprojPath).Directory;
            var packageConfig = "packages.config";
            var packageConfigFile = new FileInfo(Path.Combine(parentDirectory.FullName, packageConfig));
            if (!packageConfigFile.Exists)
            {
                XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(docNode);
                XmlNode newPackagesNode = xmlDoc.CreateElement("packages");
                xmlDoc.AppendChild(newPackagesNode);
                xmlDoc.Save(packageConfigFile.FullName);
            }
            xmlDoc.Load(packageConfigFile.FullName);
            XmlNode nunitPackageNode = xmlDoc.CreateNode(XmlNodeType.Element, "package", null);
            XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
            idAttribute.Value = "NUnit";
            XmlAttribute versionAttribute = xmlDoc.CreateAttribute("version");
            versionAttribute.Value = "3.2.1";
            XmlAttribute targetFrameworkAttribute = xmlDoc.CreateAttribute("targetFramework");
            targetFrameworkAttribute.Value = "net45";
            nunitPackageNode.Attributes.Append(idAttribute);
            nunitPackageNode.Attributes.Append(versionAttribute);
            nunitPackageNode.Attributes.Append(targetFrameworkAttribute);
            var packagesNode = xmlDoc.ChildNodes.Cast<XmlNode>().FirstOrDefault(child => child.Name == "packages");
            packagesNode.AppendChild(nunitPackageNode);
            xmlDoc.Save(packageConfigFile.FullName);
        }

        private static void AddReferenceToCsproj(string csprojPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(csprojPath);
            XmlNode referenceNode = xmlDoc.CreateNode(XmlNodeType.Element, "Reference", null);
            XmlAttribute includeAttribute = xmlDoc.CreateAttribute("Include");
            includeAttribute.Value = "NUnit";
            referenceNode.Attributes.Append(includeAttribute);
            XmlNode hintPathNode = xmlDoc.CreateElement("HintPath");
            hintPathNode.InnerText = @"..\..\Nunit.framwork.dll";
            referenceNode.AppendChild(hintPathNode);
            var projectNode = xmlDoc.ChildNodes.Cast<XmlNode>().FirstOrDefault(child => child.Name == "Project");
            var itemGroups = projectNode.ChildNodes.Cast<XmlNode>().Where(child => child.Name == "ItemGroup");
            var referencesItemGroup = itemGroups.FirstOrDefault(itemGroup => itemGroup.FirstChild.Name == "Reference");
            referencesItemGroup.AppendChild(referenceNode);
            xmlDoc.Save(csprojPath);
        }
    }
}
