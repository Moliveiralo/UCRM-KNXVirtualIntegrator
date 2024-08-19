using Xunit;
using KNXIntegrator;

namespace KNXIntegrator.Models.UnitTests;

public class KeyPairDatabaseTests{

    [Fact]
    public void Can_Add_KeyPair(){
        //Arrange
        var database = new KeyPairDatabase();

        //Act
        database.Add("key1","key2");

        //Assert
        Assert.Equal(new List<(string,string)> {("key1","key2")},database.keys);
    }

    [Fact]
    public void Can_Remove_KeyPair(){
        //Arrange
        var database = new KeyPairDatabase();

        //Act
        database.Add("key1","key2");
        database.Remove("key1","key2");

        //Assert
        Assert.Empty(database.keys);
    }

    [Fact]
    public void Can_GetByKey1(){
        //Arrange
        var database = new KeyPairDatabase();

        //Act
        database.Add("key1","key2");
        database.Add("key3","key4");
        

        //Assert
        Assert.Equal(new List<string>{"key2"},database.GetByKey1("key1"));
    }


    [Fact]
    public void Can_GetByKey2(){
        //Arrange
        var database = new KeyPairDatabase();

        //Act
        database.Add("key1","key2");
        database.Add("key3","key4");
        

        //Assert
        Assert.Equal(new List<string>{"key1"},database.GetByKey2("key2"));
    }

    [Fact]
    public void Can_GetMultipleKey(){
        //Arrange
        var database = new KeyPairDatabase();

        //Act
        database.Add("key1","key2");
        database.Add("key1","key3");
        

        //Assert
        Assert.Equal(new List<string> {"key2","key3"},database.GetByKey1("key1"));
    }


    
}