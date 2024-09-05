using KNX_Virtual_Integrator.Model.Tools;
using Xunit;
using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.UnitTests;

public class ConverterTests
{

    [Fact]
    public void ConvertBoolToValue()
    {
        //Arrange


        //Act
        var val1 = Converter.IntToGroupValue(1, 1);
        var val2 = Converter.IntToGroupValue(0, 1);


        //Assert
        Assert.Equal(new GroupValue(true), val1);
        Assert.Equal(new GroupValue(false), val2);

    }

    [Fact]
    public void ConvertLessThan8bits()
    {
        //Arrange


        //Act
        var val1 = Converter.IntToGroupValue(0, 7);
        var val2 = Converter.IntToGroupValue(127, 7);


        //Assert
        Assert.Equal(new GroupValue((byte)0, 7), val1);
        Assert.Equal(new GroupValue((byte)127, 7), val2);
    }

    [Fact]
    public void Convert8bits()
    {
        //Arrange


        //Act
        var val1 = Converter.IntToGroupValue(0, 8);
        var val2 = Converter.IntToGroupValue(255, 8);


        //Assert
        Assert.Equal(new GroupValue((byte)0, 8), val1);
        Assert.Equal(new GroupValue((byte)255, 8), val2);
    }

    [Fact]
    public void ConvertMoreThan8bits()
    {
        //Arrange


        //Act
        var val1 = Converter.IntToGroupValue(0, 16);
        var val2 = Converter.IntToGroupValue(255, 16);
        var val3 = Converter.IntToGroupValue(65535, 16);


        //Assert
        Assert.Equal(new GroupValue(new byte[] { 0, 0 }), val1);
        Assert.Equal(new GroupValue(new byte[] { 0, 255 }), val2);
        Assert.Equal(new GroupValue(new byte[] { 255, 255 }), val3);
    }

    [Fact]
    public void RaiseIncorrectParameters()
    {
        //Arrange


        //Act and Assert
        Assert.Throws<ArgumentException>(() => Converter.IntToGroupValue(1, 0));
        Assert.Throws<ArgumentException>(() => Converter.IntToGroupValue(256, 8));
        Assert.Throws<ArgumentException>(() => Converter.IntToGroupValue(256, 12));
    }
}