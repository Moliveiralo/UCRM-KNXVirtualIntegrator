using Xunit;
using KNXIntegrator.Models;
using System.Linq;


namespace KNXIntegrator.Models.UnitTests;
public class FunctionalModelDictionaryTests
{


    [Fact]
    public void Can_Add_FunctionalModel()
    {
        //Arrange
        FunctionalModelDictionary dict = new FunctionalModelDictionary();
        var model = new FunctionalModel(1);


        //Act
        dict.Add_FunctionalModel("key",model);
        
        //Assert
        Assert.Equal(model,dict.Get_FunctionalModel("key"));
    }



    [Fact]
    public void Can_Remove_FunctionalModel()
    {
        //Arrange
        FunctionalModelDictionary dict = new FunctionalModelDictionary();
        dict.Add_FunctionalModel("classic", new FunctionalModel(1));

        //Act
        dict.Remove_FunctionalModel("classic");

        //Assert
        var result = dict.Get_FunctionalModel("classic");
        Assert.Null(result);
    }

    [Fact]
    public void Can_GetAllModels(){
        //Arrange
        var dict = new FunctionalModelDictionary();
        var model1 = new FunctionalModel(1);
        var model2 = new FunctionalModel(2);
        dict.Add_FunctionalModel("key1",model1);
        dict.Add_FunctionalModel("key2",model2);

        //Act
        var all = dict.GetAllModels();

        //Assert
        Assert.Contains(model1,all);
        Assert.Contains(model2,all);
        Assert.Equal(2,all.Count);
    }




}