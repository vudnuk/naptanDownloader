using System.Net;
using System.Text.RegularExpressions;

namespace NaptanDownloader;

public class FileParser
{
    private readonly HttpClient _httpClient = new HttpClient();
    public string RemoveSpecialCharactersExceptQuotes(string list)
    {
        list = Regex.Replace(list, "[^a-zA-Z0-9_.,]+", "");

        for (int i = 0; i < list.Length - 1; i++)
        {
            if (list[i].Equals(',') && list[i].Equals(list[i + 1]))
            {
                list = list.Remove(i, 1);
            }
        }
        return list;
    }

    public string RemoveDuplicates(string list)
    {
        return string.Join(",", list.Split(',').Distinct());
        
    }

    public List<string> SplitStringIntoList(string list)
    {
        return new List<string>(list.Split(','));
    }

    public async Task MakeAPICallToDownloadFile(string localAuthority)
    {
        var url = $"https://naptan.api.dft.gov.uk/v1/access-nodes?dataFormat=xml&atcoAreaCodes={localAuthority}";
        var filePath = $@"/Users/victoriakundu/RiderProjects/NaptanDownloader/NaptanDownloader/Files/{localAuthority}.xml";
        
        try
        {
            File.Delete(filePath);
            using (var stream = await _httpClient.GetStreamAsync(url))
            {
                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await stream.CopyToAsync(fileStream);
                }
                if (File.Exists(filePath))
                {
                    Console.WriteLine("The file downloaded is: " + filePath.Substring(filePath.Length - 7));
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Exception on local authority: " + localAuthority, e);
        }
    }
    public async Task DownloadMultipleFiles(List<string> localAuthorities)
    {
        foreach (var localAuthority in localAuthorities)
        {
            await MakeAPICallToDownloadFile(localAuthority);
        }
    }

    public List<string> MakeLocalAuthoritiesThreeCharacters(List<string> localAuthorities)
    {
        var threeCharacterLAs = new List<string>();
        foreach (var localAuthority in localAuthorities)
        {
            if (localAuthority.Length < 3)
            {
                var editedLA = localAuthority.Insert(0,"0");
                threeCharacterLAs.Add(editedLA);
                continue;
            }
            threeCharacterLAs.Add(localAuthority);
        }
        return threeCharacterLAs;
    }
}