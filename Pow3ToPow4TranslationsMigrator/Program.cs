using System.Text.RegularExpressions;
using System.Xml;

var tsOutput = "ts.txt";
var fileContent = File.ReadAllText(tsOutput);

var regex = new Regex("Property \'(.*)\' does not exist on type \'IResources\'");
var regex2 = new Regex("Type '\"(.*)\"' is not assignable to type \'keyof IResources");

var matches = regex.Matches(fileContent);
var matches2 = regex2.Matches(fileContent);

var missingKeys = new HashSet<string>();

foreach (Match match in matches)
{
    if (match.Groups.Count > 1) // Group 0 is the full match, groups[1..] are captures
    {
        missingKeys.Add(match.Groups[1].Value);
    }
}

foreach (Match match in matches2)
{
    if (match.Groups.Count > 1) // Group 0 is the full match, groups[1..] are captures
    {
        missingKeys.Add(match.Groups[1].Value);
    }
}

Console.WriteLine("\nCaptured matches:");
if (missingKeys.Count == 0)
{
    Console.WriteLine("No matches found.");
    return;
}

foreach (var match in missingKeys)
{
    Console.WriteLine("Transferring the following keys to POW4 translation files:");
    Console.WriteLine(match);
}


var translationFiles = new Dictionary<string, string>
{
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.de.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.de.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.es.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.es.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.fr.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.fr.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.it.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.it.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.nl.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.nl.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.no.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.no.resx" },
    { @"D:\git\pow3\Dobco.PACSONWEB3.Core\Shared\Translations.pt-br.resx", @"D:\git\pow4\Dobco.POW4.Core.Translations\Resources\SharedResource.pt-br.resx" },
};

foreach (var (sourceXml, destinationXml) in translationFiles)
{
    try
    {
        var pow3TranslationsXmlDox = new XmlDocument();
        pow3TranslationsXmlDox.Load(sourceXml);

        var pow4TranslationsXmlDoc = new XmlDocument();
        pow4TranslationsXmlDoc.Load(destinationXml);
        var pow4TranslationsRoot = pow4TranslationsXmlDoc.SelectSingleNode("//root");

        foreach (XmlNode pow3DataNode in pow3TranslationsXmlDox.SelectNodes("//data")!)
        {
            if (missingKeys.Contains(pow3DataNode.Attributes!["name"]!.Value))
            {
                var pow4DataNode = pow4TranslationsXmlDoc.ImportNode(pow3DataNode, true);
                pow4TranslationsRoot!.AppendChild(pow4DataNode);
            }
        }
    
        pow4TranslationsXmlDoc.Save(destinationXml);
        
        Console.WriteLine($"Written {destinationXml}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing {sourceXml}: " + ex.Message);
    }
}