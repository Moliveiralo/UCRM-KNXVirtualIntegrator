using Knx.Falcon;
using KNXIntegrator.Models;

namespace KNXIntegrator.Tests.Moldels;

public class AnalysisTests
{
    [Fact]
    public void GetRecordOK()
    {
        //Arrange
        Analysis analyzer = new Analysis();
        
        //Act
        List<(GroupAddress, GroupValue, GroupAddress, GroupValue?, bool?)> actual = analyzer.GetRecords(new GroupAddress(1), new GroupAddress(2),1);
        
        //Assert
        Assert.Equal(
            new List<(GroupAddress, GroupValue, GroupAddress, GroupValue?, bool?)>
            {
                (new GroupAddress(1), new GroupValue(false), new GroupAddress(2), null, null),
                (new GroupAddress(1), new GroupValue(true), new GroupAddress(2), null, null)
            },
            actual);

    }

    [Fact]
    public void checkOK()
    {
        //Arrange
        Analysis analyzer = new Analysis();
        //Act and Assert
        Assert.Equal(true,analyzer.Check(new GroupValue(true),new GroupValue(true)));
        Assert.Equal(false,analyzer.Check(new GroupValue(true),new GroupValue(false)));
        Assert.Equal(true,analyzer.Check(Converter.IntToGroupValue(65535,16),Converter.IntToGroupValue(65535,16)));
    }
}