using System.Collections;
using System.Net;
using Xunit;
using FluentAssertions;
using NaptanDownloader;

namespace NaptanDownloaderTests;

public class ProgramTest
{
    [Fact]
    public void ShouldRemoveSpecialCharactersExceptCommasFromString()
    {
        var countyList =
            "[\"490\",\"639\",\"21\",\"190\",\"649\",\"668\",\"40\",\"40\",\"\u00a0\",\"650\"";
        var fileParser = new FileParser();

        var result = fileParser.RemoveSpecialCharactersExceptQuotes(countyList);

        result.Should().Be("490,639,21,190,649,668,40,40,650");
    }

    [Fact]
    public void ShouldRemoveDuplicatesFromString()
    {
        var list = "490,639,21,190,649,668,40,40,650";
        var fileParser = new FileParser();

        var result = fileParser.RemoveDuplicates(list);

        result.Should().Be("490,639,21,190,649,668,40,650");
    }

    [Fact]
    public void ShouldSplitStringIntoList()
    {
        var list = "490,639,21,190,649,668,40,40,650";
        var fileParser = new FileParser();
        List<string> stringList = fileParser.SplitStringIntoList(list);

        stringList.Count.Should().Be(9);
        
    }
    
    [Fact]
    public void ShouldMakeLocalAuthoritiesThreeCharactersInLength()
    {
        List<string> localAuthorities = new List<string>(){"490","639","21","190","649","668","40","650"};
        var fileParser = new FileParser();

        var result = fileParser.MakeLocalAuthoritiesThreeCharacters(localAuthorities);
        result.Should().BeEquivalentTo(new List<string>(){ "490", "639", "021", "190", "649", "668", "040", "650" });
    }

    [Fact]
    public async void ShouldMakeSuccessfulAPICall()
    {
        var localAuthority = "460";
        var fileParser = new FileParser();
        
        var exception = await Record.ExceptionAsync (async () => await fileParser.MakeAPICallToDownloadFile(localAuthority));
        
        Assert.Null(exception);
        
        var filePath = $@"/Users/victoriakundu/RiderProjects/NaptanDownloader/NaptanDownloader/Files/{localAuthority}.xml";
        var fileContent = File.ReadAllLines(filePath);
        fileContent.Should().NotBeNull();
        File.Delete(filePath);
    }
    
    [Fact]
    public async void ShouldMakeUnsuccessfulAPICall()
    {
        var invalidLocalAuthority = "ghbjkl";
        var fileParser = new FileParser();
        
        var exception = await Record.ExceptionAsync (async () => await fileParser.MakeAPICallToDownloadFile(invalidLocalAuthority));
        
        Assert.NotNull(exception);
    }
    
    [Fact]
    public async void ShouldDownloadMultipleFiles()
    {
        var localAuthorities = new List<string>(){"190","010","410","639","668"};
        var fileParser = new FileParser();
        
        var exception = await Record.ExceptionAsync (async () => await fileParser.DownloadMultipleFiles(localAuthorities));
        
        Assert.Null(exception);

        foreach (var localAuthority in localAuthorities)
        {
            var filePath = $@"/Users/victoriakundu/RiderProjects/NaptanDownloader/NaptanDownloader/Files/{localAuthority}.xml";
            var fileContent = File.ReadAllLines(filePath);
            fileContent.Should().NotBeNull();
            File.Delete(filePath);
        }
    }
}