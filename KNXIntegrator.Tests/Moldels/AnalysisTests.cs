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
        List<Analysis.RecordEntry> actual = analyzer.GetRecords(new GroupAddress(1), new GroupAddress(2),1);
        
        //Assert
        Assert.Equal(
            new List<Analysis.RecordEntry>
            {
                new Analysis.RecordEntry
                {
                    CmdAddr = new GroupAddress(1),
                    CmdVal =  new GroupValue(false),
                    StateAddr =   new GroupAddress(2),
                    StateVal = null,
                    TestOK = null
                },
                new Analysis.RecordEntry
                {
                    CmdAddr = new GroupAddress(1),
                    CmdVal =  new GroupValue(true),
                    StateAddr =   new GroupAddress(2),
                    StateVal = null,
                    TestOK = null 
                }
            },
            actual);

    }

    [Fact]
    public void checkOK()
    {
        //Arrange
        Analysis analyzer = new Analysis();
        //Act and Assert
        Assert.True(analyzer.Check(new GroupValue(true),new GroupValue(true)));
        Assert.False(analyzer.Check(new GroupValue(true),new GroupValue(false)));
        Assert.True(analyzer.Check(Converter.IntToGroupValue(65535,16),Converter.IntToGroupValue(65535,16)));
    }
}