using Xunit;
using Knx.Falcon;

namespace KNXIntegrator.Models.UnitTests;

public class ConverterTests
{

    [Fact]
    public void ConvertBoolToValue()
    {
        //Arrange
        Converter converter = new Converter();

        //Act
        var val1 = converter.IntToGroupValue(1, 1);
        var val2 = converter.IntToGroupValue(0, 1);


        //Assert
        Assert.Equal(new GroupValue(true), val1);
        Assert.Equal(new GroupValue(false), val2);

    }

    [Fact]
    public void ConvertLessThan8bits()
    {
        //Arrange
        Converter converter = new Converter();

        //Act
        var val1 = converter.IntToGroupValue(0, 7);
        var val2 = converter.IntToGroupValue(127, 7);


        //Assert
        Assert.Equal(new GroupValue((byte)0, 7), val1);
        Assert.Equal(new GroupValue((byte)127, 7), val2);
    }

    [Fact]
    public void Convert8bits()
    {
        //Arrange
        Converter converter = new Converter();

        //Act
        var val1 = converter.IntToGroupValue(0, 8);
        var val2 = converter.IntToGroupValue(255, 8);


        //Assert
        Assert.Equal(new GroupValue((byte)0, 8), val1);
        Assert.Equal(new GroupValue((byte)255, 8), val2);
    }

    [Fact]
    public void ConvertMoreThan8bits()
    {
        //Arrange
        Converter converter = new Converter();

        //Act
        var val1 = converter.IntToGroupValue(0, 16);
        var val2 = converter.IntToGroupValue(255, 16);
        var val3 = converter.IntToGroupValue(65535, 16);


        //Assert
        Assert.Equal(new GroupValue(new byte[] { 0, 0 }), val1);
        Assert.Equal(new GroupValue(new byte[] { 0, 255 }), val2);
        Assert.Equal(new GroupValue(new byte[] { 255, 255 }), val3);
    }

    [Fact]
    public void RaiseIncorrectParameters()
    {
        //Arrange
        Converter converter = new Converter();

        //Act and Assert
        Assert.Throws<ArgumentException>(() => converter.IntToGroupValue(1, 0));
        Assert.Throws<ArgumentException>(() => converter.IntToGroupValue(256, 8));
        Assert.Throws<ArgumentException>(() => converter.IntToGroupValue(256, 12));
    }
}